using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Gameplay.Level.LevelGenerator
{
    public class LevelGenerator : MonoBehaviour
    {
        [HideInInspector] public static LevelGenerator Main;
        
        
        [Header("General")] [SerializeField] [Tooltip("Size of a terrainUnit")]
        private int seed = 0;

        [Space] [SerializeField] [Tooltip("Size of a terrainUnit")]
        private float terrainUnitSize = 1f;

        [SerializeField] [Tooltip("Width and Height of the terrain in terrainUnits")]
        private int terrainSize = 100;

        [SerializeField] [Tooltip("maximum random offset of each vertex in terrainUnits")]
        private float maxRandomVertexOffset = 0f;

        [Space] [SerializeField] [Tooltip("texture showing the area being flattened for the base")]
        private Texture2D baseFlatteningMap;

        // [0] first black pixel in x axis
        // [1] last in x axis
        // [2] first in y axis
        // [3] last in y axis
        private int[] baseFlatteningMapBounds = new int[4];

        [SerializeField] [Tooltip("number of terrainUnits to smoothen between flattened base and terrain")]
        private int baseTerrainSmootheningDistance = 10;


        [Space] [Header("Terrain Generation")] [SerializeField]
        private float noiseScale = 1f;

        [SerializeField] private float noiseMultiplier = 1f;

        
        private NavMeshSurface navMesh;
        

        // debug
        [Space] [SerializeField] private bool generateRandomSeed = false;
        [SerializeField] private bool regenerate = false;
        private readonly Stopwatch terrainGenStopwatch = new Stopwatch();
        [SerializeField] private double generateMS = 0f;

        private void Awake()
        {
            Main = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            CalculateBaseFlatteningMapBounds();
        }

        // Update is called once per frame
        void Update()
        {
            if (regenerate)
            {
                regenerate = false;
                terrainGenStopwatch.Restart();
                GenerateLevel();
                terrainGenStopwatch.Stop();
                generateMS = terrainGenStopwatch.Elapsed.TotalMilliseconds;
            }
        }

        private void OnValidate()
        {
            if (regenerate)
            {
                regenerate = false;
                terrainGenStopwatch.Restart();
                GenerateLevel();
                terrainGenStopwatch.Stop();
                generateMS = terrainGenStopwatch.Elapsed.TotalMilliseconds;
            }
        }


        public void UpdateNavMesh()
        {
            navMesh.UpdateNavMesh(navMesh.navMeshData);
            // navMesh.BuildNavMesh();
        }

        // Vertex with FP32 position, FP16 2D normal and a 4-byte tangent.
        // In some cases StructLayout attribute needs
        // to be used, to get the data layout match exactly what it needs to be.
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct TerrainVertex
        {
            public Vector3 pos;
            //public ushort normalX, normalY;
            //public Color32 tangent;

            public TerrainVertex(Vector3 pos)
            {
                this.pos = pos;
                //this.normalX = normalX;
                //this.normalY = normalY;
            }
        }

        public void GenerateLevel()
        {
            if (generateRandomSeed)
                seed = new System.Random().Next();
            
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            int meshSplitCount = Mathf.CeilToInt(terrainSize / 100f);
            int singleMeshSize = terrainSize / meshSplitCount;
            terrainSize = singleMeshSize * meshSplitCount;

            Mesh[,] meshes = new Mesh[meshSplitCount, meshSplitCount];
            for (int ix = 0; ix < meshSplitCount; ix++)
            {
                for (int iy = 0; iy < meshSplitCount; iy++)
                {
                    meshes[ix, iy] = new Mesh();
                }
            }

            // specify vertex count and layout
            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                //new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
                //new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
            };

            int vertexCount = singleMeshSize * singleMeshSize + singleMeshSize * 2 + 1;
            int triangleIndexCount = singleMeshSize * singleMeshSize * 6;

            TerrainVertex[][][] verts = new TerrainVertex[meshSplitCount][][];
            for (int ix = 0; ix < meshSplitCount; ix++)
            {
                verts[ix] = new TerrainVertex[meshSplitCount][];
                for (int iy = 0; iy < meshSplitCount; iy++)
                {
                    verts[ix][iy] = new TerrainVertex[vertexCount];
                }
            }

            ushort[][][] triangles = new ushort[meshSplitCount][][];
            for (int ix = 0; ix < meshSplitCount; ix++)
            {
                triangles[ix] = new ushort[meshSplitCount][];
                for (int iy = 0; iy < meshSplitCount; iy++)
                {
                    triangles[ix][iy] = new ushort[triangleIndexCount];
                }
            }

            float terrainSizeUnityUnits = terrainSize * terrainUnitSize;
            float terrainSizeUnityUnitsHalf = terrainSizeUnityUnits / 2;
            float maxRandomVertexOffsetUnityUnits = maxRandomVertexOffset * terrainUnitSize;
            RandomEx rndmBool = new RandomEx(seed);


            int curMeshX;
            int curMeshZ;
            Mesh curMesh;

            int[,] vertIndexes = new int[meshSplitCount, meshSplitCount];
            ushort[,] triangleIndexes = new ushort[meshSplitCount, meshSplitCount];

            System.Random seedRandom = new System.Random(seed);
            int noiseOffsetX = Mathf.FloorToInt((float)seedRandom.NextDouble() * 1000);
            int noiseOffsetY = Mathf.FloorToInt((float)seedRandom.NextDouble() * 1000);

            for (float z = -terrainSizeUnityUnitsHalf;
                 z <= terrainSizeUnityUnitsHalf;
                 z += terrainUnitSize)
            {
                float positiveZ = z + terrainSizeUnityUnitsHalf;
                for (float x = -terrainSizeUnityUnitsHalf;
                     x <= terrainSizeUnityUnitsHalf;
                     x += terrainUnitSize)
                {
                    float positiveX = x + terrainSizeUnityUnitsHalf;

                    curMeshX = Mathf.FloorToInt(positiveX / (terrainSizeUnityUnits / meshSplitCount));
                    curMeshZ = Mathf.FloorToInt(positiveZ / (terrainSizeUnityUnits / meshSplitCount));
                    //curMesh = meshes[curMeshX, curMeshZ];

                    float terrainYOffsetMultiplier = SampleTerrainMultiplier(x, z);
                    float terrainYOffset = Mathf.PerlinNoise((positiveX + 0.1f) * noiseScale + noiseOffsetX, (positiveZ + 0.1f) * noiseScale + noiseOffsetY) * noiseMultiplier -
                                           noiseMultiplier / 2;
                    terrainYOffset *= terrainYOffsetMultiplier;

                    // x + random Number from -1 to (excluding)1 * max offset
                    float vertexXRandomized = x + (2 * (float)seedRandom.NextDouble() - 1) * maxRandomVertexOffsetUnityUnits;
                    float vertexZRandomized = z + (2 * (float)seedRandom.NextDouble() - 1) * maxRandomVertexOffsetUnityUnits;

                    if (x < terrainSizeUnityUnitsHalf - terrainUnitSize / 2 &&
                        z < terrainSizeUnityUnitsHalf - terrainUnitSize / 2)
                    {
                        triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)vertIndexes[curMeshX, curMeshZ];
                        triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 1);
                        if (rndmBool.NextBoolean())
                        {
                            triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + 1);
                            triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 1);
                        }
                        else
                        {
                            triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 2);
                            triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)vertIndexes[curMeshX, curMeshZ];
                        }

                        triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 2);
                        triangles[curMeshX][curMeshZ][triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + 1);

                        verts[curMeshX][curMeshZ][vertIndexes[curMeshX, curMeshZ]++] = new TerrainVertex(
                            new Vector3(vertexXRandomized, terrainYOffset, vertexZRandomized));
                    }

                    bool onNewMeshX = curMeshX > 0 && positiveX - terrainUnitSize / 2 < singleMeshSize * terrainUnitSize * curMeshX &&
                                      positiveX + terrainUnitSize / 2 > singleMeshSize * terrainUnitSize * curMeshX;
                    bool onNewMeshZ = curMeshZ > 0 && positiveZ - terrainUnitSize / 2 < singleMeshSize * terrainUnitSize * curMeshZ &&
                                      positiveZ + terrainUnitSize / 2 > singleMeshSize * terrainUnitSize * curMeshZ;

                    if (onNewMeshX && curMeshZ < meshSplitCount)
                    {
                        verts[curMeshX - 1][curMeshZ][vertIndexes[curMeshX - 1, curMeshZ]++] = new TerrainVertex(
                            new Vector3(vertexXRandomized, terrainYOffset, vertexZRandomized));
                    }

                    if (onNewMeshZ && curMeshX < meshSplitCount)
                    {
                        verts[curMeshX][curMeshZ - 1][vertIndexes[curMeshX, curMeshZ - 1]++] = new TerrainVertex(
                            new Vector3(vertexXRandomized, terrainYOffset, vertexZRandomized));
                    }

                    if (onNewMeshX && onNewMeshZ)
                    {
                        verts[curMeshX - 1][curMeshZ - 1][vertIndexes[curMeshX - 1, curMeshZ - 1]++] = new TerrainVertex(
                            new Vector3(vertexXRandomized, terrainYOffset, vertexZRandomized));
                    }
                }
            }

            GameObject go = null;

            for (int meshX = 0; meshX < meshSplitCount; meshX++)
            {
                for (int meshZ = 0; meshZ < meshSplitCount; meshZ++)
                {
                    curMesh = meshes[meshX, meshZ];

                    curMesh.SetVertexBufferParams(vertexCount, layout);
                    curMesh.SetVertexBufferData(verts[meshX][meshZ], 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
                    curMesh.SetIndices(triangles[meshX][meshZ], MeshTopology.Triangles, 0, false, 0);

                    //mesh.RecalculateNormals();
                    curMesh.RecalculateBounds();

                    go = new GameObject("levelMesh [" + meshX + ", " + meshZ + "]",
                        typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));
                    go.transform.SetParent(transform);
                    go.isStatic = true;

                    go.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
                    go.GetComponent<MeshFilter>().mesh = curMesh;
                    go.GetComponent<MeshCollider>().sharedMesh = curMesh;
                    go.layer = 11;
                }
            }

            if (go != null)
            {
                navMesh = go.AddComponent<NavMeshSurface>();
                navMesh.layerMask = LayerMask.GetMask(new string[] {"Terrain", "NavMeshObstacle"});
                navMesh.BuildNavMesh();
            }


            System.GC.Collect();
        }

        private float SampleTerrainMultiplier(float x, float y)
        {
            float texWidth = baseFlatteningMap.width;
            float texHeight = baseFlatteningMap.height;
            int textureX = Mathf.RoundToInt(x / terrainUnitSize + texWidth / 2);
            int textureY = Mathf.RoundToInt(y / terrainUnitSize + texHeight / 2);

            if (textureX < baseFlatteningMapBounds[0] - baseTerrainSmootheningDistance ||
                textureX > baseFlatteningMapBounds[1] + baseTerrainSmootheningDistance ||
                textureY < baseFlatteningMapBounds[2] - baseTerrainSmootheningDistance ||
                textureY > baseFlatteningMapBounds[3] + baseTerrainSmootheningDistance)
                return 1.0f;

            float sampledPixel = baseFlatteningMap.GetPixel(textureX, textureY).r;

            if (sampledPixel < .01 || baseTerrainSmootheningDistance == 0)
                return sampledPixel;

            float closest = baseTerrainSmootheningDistance;

            // checking bounds to see whether we can skip some checks in loop below
            float temp = textureX - baseTerrainSmootheningDistance;
            float lowestX = temp < baseFlatteningMapBounds[0] ? -(baseTerrainSmootheningDistance - (baseFlatteningMapBounds[0] - temp)) : -baseTerrainSmootheningDistance;
            temp = textureX + baseTerrainSmootheningDistance;
            float highestX = temp > baseFlatteningMapBounds[1] ? (baseTerrainSmootheningDistance - (temp - baseFlatteningMapBounds[1])) : baseTerrainSmootheningDistance;

            temp = textureY - baseTerrainSmootheningDistance;
            float lowestY = temp < baseFlatteningMapBounds[2] ? -(baseTerrainSmootheningDistance - (baseFlatteningMapBounds[2] - temp)) : -baseTerrainSmootheningDistance;
            temp = textureY + baseTerrainSmootheningDistance;
            float highestY = temp > baseFlatteningMapBounds[3] ? (baseTerrainSmootheningDistance - (temp - baseFlatteningMapBounds[3])) : baseTerrainSmootheningDistance;

            // checking all pixels around centerPixel to find closest black one
            for (float ix = lowestX; ix < highestX + 1; ix++)
            {
                int texX = Mathf.FloorToInt(textureX + ix);
                if (texX >= texWidth || texX < 0)
                    continue;

                for (float iy = lowestY; iy < highestY + 1; iy++)
                {
                    int texY = Mathf.FloorToInt(textureY + iy);

                    if (texY >= texHeight || texY < 0)
                        continue;

                    sampledPixel = baseFlatteningMap.GetPixel(texX, texY).r;
                    if (sampledPixel < .01)
                    {
                        float dist = new Vector2(ix, iy).magnitude;
                        closest = (dist < closest) ? dist : closest;
                    }
                }
            }

            return Mathf.SmoothStep(0, 1, closest / baseTerrainSmootheningDistance);
        }

        private void CalculateBaseFlatteningMapBounds()
        {
            int texWidth = baseFlatteningMap.width;
            int texHeight = baseFlatteningMap.height;

            baseFlatteningMapBounds[0] = texWidth;
            baseFlatteningMapBounds[1] = 0;
            baseFlatteningMapBounds[2] = texHeight;
            baseFlatteningMapBounds[3] = 0;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    if (baseFlatteningMap.GetPixel(x, y).r < .01)
                    {
                        baseFlatteningMapBounds[0] = (x < baseFlatteningMapBounds[0]) ? x : baseFlatteningMapBounds[0];
                        baseFlatteningMapBounds[1] = (x > baseFlatteningMapBounds[1]) ? x : baseFlatteningMapBounds[1];
                        baseFlatteningMapBounds[2] = (y < baseFlatteningMapBounds[2]) ? y : baseFlatteningMapBounds[2];
                        baseFlatteningMapBounds[3] = (y > baseFlatteningMapBounds[3]) ? y : baseFlatteningMapBounds[3];
                    }
                }
            }
        }
    }

    public class RandomEx : System.Random
    {
        private uint boolBits;

        public RandomEx() : base()
        {
        }

        public RandomEx(int seed) : base(seed)
        {
        }

        public bool NextBoolean()
        {
            boolBits >>= 1;
            if (boolBits <= 1) boolBits = (uint)~this.Next();
            return (boolBits & 1) == 0;
        }
    }
}