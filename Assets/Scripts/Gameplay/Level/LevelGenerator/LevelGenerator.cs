using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Level.LevelGenerator
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] [Tooltip("Width and Height of the terrain in units specified below")]
        private int terrainSize = 100;

        [SerializeField] [Tooltip("Size of a single unit if the terrain")]
        private float terrainUnitSize = 1f;

        [SerializeField] [Tooltip("Width and Height of the flattened base area")]
        private int maxBaseSize = 10;

        [SerializeField] private float noiseScale = 1f;
        [SerializeField] private float noiseMultiplier = 1f;

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

            public TerrainVertex(Vector3 pos, ushort normalX, ushort normalY)
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
            int vertIndex = 0;
            ushort triangleIndex = 0;

            for (float z = 0; z <= terrainSize; z++)
            {
                for (float x = 0; x <= terrainSize; x++)
                {
                    if (x < terrainSize && z < terrainSize)
                    {
                        triangles[triangleIndex++] = (ushort)vertIndex;
                        triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 1);
                        triangles[triangleIndex++] = (ushort)(vertIndex + 1);
                        triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 1);
                        triangles[triangleIndex++] = (ushort)(vertIndex + terrainSize + 2);
                        triangles[triangleIndex++] = (ushort)(vertIndex + 1);
                    }

                    float terrainYOffset = Mathf.PerlinNoise((x + 0.1f) * noiseScale, (z + 0.1f) * noiseScale) * noiseMultiplier;

                    verts[vertIndex++] = new TerrainVertex(new Vector3(x, 10f + terrainYOffset, z), 0, 1);
                }
            }

            mesh.SetVertexBufferParams(vertexCount, layout);
            mesh.SetVertexBufferData(verts, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0, false, 0);

            //mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GetComponent<MeshFilter>().mesh = mesh;


            // var mesh = new Mesh();
            // // specify vertex count and layout
            // var layout = new[]
            // {
            //     new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            //     new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
            //     //new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
            // };
            // var vertexCount = 4;
            // mesh.SetVertexBufferParams(vertexCount, layout);
            //
            // // set vertex data
            // var verts = new NativeArray<TerrainVertex>(vertexCount, Allocator.Temp,
            //     NativeArrayOptions.UninitializedMemory);
            //
            // verts[0] = new TerrainVertex(new Vector3(0f, 10f, 0f), 0, 1);
            // verts[1] = new TerrainVertex(new Vector3(0f, 10f, 10f), 0, 1);
            // verts[2] = new TerrainVertex(new Vector3(10f, 10f, 0f), 0, 1);
            // verts[3] = new TerrainVertex(new Vector3(10f, 10f, 10f), 0, 1);
            //
            //
            // var triangles = new NativeArray<ushort>(6, Allocator.Temp,
            //     NativeArrayOptions.UninitializedMemory);
            //
            // triangles[0] = 0;
            // triangles[1] = 1;
            // triangles[2] = 2;
            // triangles[3] = 3;
            // triangles[4] = 2;
            // triangles[5] = 1;
            //
            // //mesh.indexFormat = IndexFormat.UInt16;
            //
            // mesh.SetVertexBufferData(verts, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
            //
            // //mesh.SetIndexBufferParams(6, IndexFormat.UInt16);
            // //mesh.SetIndexBufferData(triangles, 0, 0, 6, MeshUpdateFlags.Default);
            // mesh.SetIndices(triangles, MeshTopology.Triangles, 0, false, 0);
            // //mesh.SetTriangles(triangles.ToArray(), 0, false);
            //
            //
            // //mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            //
            //
            // // Debug.Log("Indices: " + mesh.GetIndices(0));
            // // foreach (var index in mesh.GetIndices(0))
            // // {
            // //     Debug.Log(index);
            // // }
            // //
            // // Debug.Log("Triangles: " + mesh.GetTriangles(0));
            // // foreach (var index in mesh.GetTriangles(0))
            // // {
            // //     Debug.Log(index);
            // // }
            //
            //
            // GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}