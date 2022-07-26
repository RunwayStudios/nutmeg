using System.Diagnostics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
            GenerateLevel();
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
            var mesh = new Mesh();

            // specify vertex count and layout
            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                //new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
                //new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
            };

            int vertexCount = terrainSize * terrainSize + terrainSize * 2 + 1;

            var verts = new NativeArray<TerrainVertex>(vertexCount, Allocator.Temp,
                NativeArrayOptions.ClearMemory);

            int triangleIndexCount = terrainSize * terrainSize * 6;

            var triangles = new NativeArray<ushort>(triangleIndexCount, Allocator.Temp,
                NativeArrayOptions.ClearMemory);


            // ... fill in vertex array data here...

            float terrainSizeUnityUnits = terrainSize * terrainUnitSize;
            float maxRandomVertexOffsetUnityUnits = maxRandomVertexOffset * terrainUnitSize;
            RandomEx rndmBool = new RandomEx();

            int vertIndex = 0;
            ushort triangleIndex = 0;

            for (float z = 0; z <= terrainSizeUnityUnits; z += terrainUnitSize)
            {
                for (float x = 0; x <= terrainSizeUnityUnits; x += terrainUnitSize)
                {
                    if (x < terrainSizeUnityUnits && z < terrainSizeUnityUnits)
                    {
                        triangles[triangleIndex++] = (ushort)vertIndex;
                        triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 1);
                        if (rndmBool.NextBoolean())
                        {
                            triangles[triangleIndex++] = (ushort)(vertIndex + 1);
                            triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 1);
                        }
                        else
                        {
                            triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 2);
                            triangles[triangleIndex++] = (ushort)vertIndex;
                        }

                        triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 2);
                        triangles[triangleIndex++] = (ushort)(vertIndex + 1);
                    }

                    float terrainYOffsetMultiplier = SampleTerrainMultiplier(x, z);

                    float terrainYOffset = Mathf.PerlinNoise((x + 0.1f) * noiseScale, (z + 0.1f) * noiseScale) * noiseMultiplier -
                                           noiseMultiplier / 2;
                    terrainYOffset *= terrainYOffsetMultiplier;

                    verts[vertIndex++] = new TerrainVertex(new Vector3(
                        x + Random.Range(-1, 1) * maxRandomVertexOffsetUnityUnits,
                        terrainYOffset,
                        z + Random.Range(-1, 1) * maxRandomVertexOffsetUnityUnits));
                }
            }

            mesh.SetVertexBufferParams(vertexCount, layout);
            mesh.SetVertexBufferData(verts, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0, false, 0);

            //mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
            
            System.GC.Collect();
        }

        private float SampleTerrainMultiplier(float x, float y)
        {
            int centreTextureX = Mathf.RoundToInt(x / terrainSize * baseFlatteningMap.width);
            int centreTextureY = Mathf.RoundToInt(y / terrainSize * baseFlatteningMap.height);
            float sampledPixel = baseFlatteningMap.GetPixel(centreTextureX, centreTextureY).r;

            if (sampledPixel < .01)
                return sampledPixel;

            float closest = baseTerrainSmootheningDistance;
            float incrementX = (float)terrainSize / baseFlatteningMap.width;
            float incrementY = (float)terrainSize / baseFlatteningMap.height;
            float texWidth = baseFlatteningMap.width;
            float texHeight = baseFlatteningMap.height;

            for (float ix = -baseTerrainSmootheningDistance; ix < baseTerrainSmootheningDistance + 1; ix += incrementX)
            {
                if (ix == 0)
                    continue;
                
                int texX = Mathf.FloorToInt(centreTextureX + ix);
                if (texX >= texWidth || texX < 0)
                    continue;
                
                for (float iy = -baseTerrainSmootheningDistance; iy < baseTerrainSmootheningDistance + 1; iy += incrementY)
                {
                    if (iy == 0)
                        continue;
                    
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

            return Mathf.Clamp(closest / baseTerrainSmootheningDistance, 0, 1);
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