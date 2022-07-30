using System;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Gameplay.Level.LevelGenerator
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("General")] [SerializeField] [Tooltip("Size of a terrainUnit")]
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


        [Space] [SerializeField] private bool regenerate = false;
        private readonly Stopwatch terrainGenStopwatch = new Stopwatch();
        [SerializeField] private double generateMS = 0f;

        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            CalculateBaseFlatteningMapBounds();
            //GenerateLevel();
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
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            int meshSplitCount = Mathf.CeilToInt(terrainSize / 100f);
            int singleMeshSize = terrainSize / meshSplitCount;

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

            NativeArray<TerrainVertex>[,] verts = new NativeArray<TerrainVertex>(vertexCount, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory)[meshSplitCount,meshSplitCount];

            var triangles = new NativeArray<ushort>(triangleIndexCount, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);

            float terrainSizeUnityUnits = terrainSize * terrainUnitSize;
            float terrainSizeUnityUnitsHalf = terrainSizeUnityUnits / 2;
            float maxRandomVertexOffsetUnityUnits = maxRandomVertexOffset * terrainUnitSize;
            RandomEx rndmBool = new RandomEx();


            int curMeshX;
            int curMeshZ;
            Mesh curMesh;

            int[,] vertIndexes = new int[meshSplitCount,meshSplitCount];
            ushort[,] triangleIndexes = new ushort[meshSplitCount,meshSplitCount];

            for (float z = -terrainSizeUnityUnitsHalf;
                 z <= terrainSizeUnityUnitsHalf;
                 z += terrainUnitSize)
            {
                for (float x = -terrainSizeUnityUnitsHalf;
                     x <= terrainSizeUnityUnitsHalf;
                     x += terrainUnitSize)
                {
                    curMeshX = Mathf.FloorToInt(x / singleMeshSize);
                    curMeshZ = Mathf.FloorToInt(z / singleMeshSize);
                    curMesh = meshes[curMeshX, curMeshZ];

                    if (x < terrainSizeUnityUnitsHalf &&
                        z < terrainSizeUnityUnitsHalf)
                    {
                        triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)vertIndexes[curMeshX, curMeshZ];
                        triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 1);
                        if (rndmBool.NextBoolean())
                        {
                            triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + 1);
                            triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 1);
                        }
                        else
                        {
                            triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 2);
                            triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)vertIndexes[curMeshX, curMeshZ];
                        }

                        triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + singleMeshSize + 2);
                        triangles[triangleIndexes[curMeshX, curMeshZ]++] = (ushort)(vertIndexes[curMeshX, curMeshZ] + 1);
                    }

                    float terrainYOffsetMultiplier = SampleTerrainMultiplier(x + terrainSizeUnityUnitsHalf, z + terrainSizeUnityUnitsHalf);

                    float terrainYOffset = Mathf.PerlinNoise((x + 0.1f) * noiseScale, (z + 0.1f) * noiseScale) * noiseMultiplier -
                                           noiseMultiplier / 2;
                    terrainYOffset *= terrainYOffsetMultiplier;

                    verts[vertIndexes[curMeshX, curMeshZ]++] = new TerrainVertex(new Vector3(
                        x + Random.Range(-1, 1) * maxRandomVertexOffsetUnityUnits,
                        terrainYOffset,
                        z + Random.Range(-1, 1) * maxRandomVertexOffsetUnityUnits));
                }
            }

            for (int meshX = 0; meshX < meshSplitCount; meshX++)
            {
                for (int meshZ = 0; meshZ < meshSplitCount; meshZ++)
                {
                    curMesh = meshes[meshX, meshZ];
                    
                    curMesh.SetVertexBufferParams(vertexCount, layout);
                    curMesh.SetVertexBufferData(verts, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
                    curMesh.SetIndices(triangles, MeshTopology.Triangles, 0, false, 0);

                    //mesh.RecalculateNormals();
                    curMesh.RecalculateBounds();

                    GameObject go = new GameObject("levelMesh [" + meshX + ", " + meshZ + "]",
                        typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer));
                    go.transform.SetParent(transform);

                    go.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
                    go.GetComponent<MeshFilter>().mesh = curMesh;
                    go.GetComponent<MeshCollider>().sharedMesh = curMesh;
                }
            }


            System.GC.Collect();
        }

        private float SampleTerrainMultiplier(float x, float y)
        {
            float texWidth = baseFlatteningMap.width;
            float texHeight = baseFlatteningMap.height;
            int centreTextureX = Mathf.RoundToInt(x / terrainSize * texWidth);
            int centreTextureY = Mathf.RoundToInt(y / terrainSize * texHeight);

            if (centreTextureX < baseFlatteningMapBounds[0] - baseTerrainSmootheningDistance ||
                centreTextureX > baseFlatteningMapBounds[1] + baseTerrainSmootheningDistance ||
                centreTextureY < baseFlatteningMapBounds[2] - baseTerrainSmootheningDistance ||
                centreTextureY > baseFlatteningMapBounds[3] + baseTerrainSmootheningDistance)
                return 1.0f;

            float sampledPixel = baseFlatteningMap.GetPixel(centreTextureX, centreTextureY).r;

            if (sampledPixel < .01 || baseTerrainSmootheningDistance == 0)
                return sampledPixel;

            float closest = baseTerrainSmootheningDistance;
            float incrementX = terrainSize / texWidth;
            float incrementY = terrainSize / texHeight;

            // checking bounds to see whether we can skip some checks in loop below
            float temp = centreTextureX - baseTerrainSmootheningDistance;
            float lowestX = temp < baseFlatteningMapBounds[0] ? -(baseTerrainSmootheningDistance - (baseFlatteningMapBounds[0] - temp)) : -baseTerrainSmootheningDistance;
            temp = centreTextureX + baseTerrainSmootheningDistance;
            float highestX = temp > baseFlatteningMapBounds[1] ? (baseTerrainSmootheningDistance - (temp - baseFlatteningMapBounds[1])) : baseTerrainSmootheningDistance;

            temp = centreTextureY - baseTerrainSmootheningDistance;
            float lowestY = temp < baseFlatteningMapBounds[2] ? -(baseTerrainSmootheningDistance - (baseFlatteningMapBounds[2] - temp)) : -baseTerrainSmootheningDistance;
            temp = centreTextureY + baseTerrainSmootheningDistance;
            float highestY = temp > baseFlatteningMapBounds[3] ? (baseTerrainSmootheningDistance - (temp - baseFlatteningMapBounds[3])) : baseTerrainSmootheningDistance;

            // checking all pixels around centerPixel to find closest black one
            for (float ix = lowestX; ix < highestX + 1; ix += incrementX)
            {
                int texX = Mathf.FloorToInt(centreTextureX + ix);
                if (texX >= texWidth || texX < 0)
                    continue;

                for (float iy = lowestY; iy < highestY + 1; iy += incrementY)
                {
                    int texY = Mathf.FloorToInt(centreTextureY + iy);

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