using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxelCube
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        private static readonly Vector3 P0 = new Vector3(0, 0, 0);
        private static readonly Vector3 P1 = new Vector3(1, 0, 0);
        private static readonly Vector3 P2 = new Vector3(1, 1, 0);
        private static readonly Vector3 P3 = new Vector3(0, 1, 0);
        private static readonly Vector3 P4 = new Vector3(0, 1, 1);
        private static readonly Vector3 P5 = new Vector3(1, 1, 1);
        private static readonly Vector3 P6 = new Vector3(1, 0, 1);
        private static readonly Vector3 P7 = new Vector3(0, 0, 1);

        private readonly int[] _cubeTriangles =
        {
            // Bottom
            0, 1, 2,
            0, 2, 3,

            // Left
            0 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
            0 + 4 * 1, 3 + 4 * 1, 2 + 4 * 1,

            // Front
            0 + 4 * 2, 1 + 4 * 2, 2 + 4 * 2,
            0 + 4 * 2, 2 + 4 * 2, 3 + 4 * 2,

            // Back
            0 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
            0 + 4 * 3, 3 + 4 * 3, 2 + 4 * 3,

            // Right
            0 + 4 * 4, 1 + 4 * 4, 2 + 4 * 4,
            0 + 4 * 4, 2 + 4 * 4, 3 + 4 * 4,

            // Top
            0 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            0 + 4 * 5, 3 + 4 * 5, 2 + 4 * 5
        };

        private readonly Vector3[] _cubeVertices =
        {
            // Bottom
            P0, P1, P6, P7,

            // Left
            P0, P1, P2, P3,

            // Front
            P0, P7, P4, P3,

            // Back
            P1, P6, P5, P2,

            // Right
            P7, P6, P5, P4,

            // Top
            P3, P2, P5, P4
        };

        private readonly ushort[] _voxels = new ushort[16 * 16 * 16];
        private MeshFilter _meshFilter;

        public ushort this[int x, int y, int z]
        {
            get => _voxels[x * 16 * 16 + y * 16 + z];
            set => _voxels[x * 16 * 16 + y * 16 + z] = value;
        }

        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            RenderToMesh();
        }

        private void RenderToMesh()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            for (var x = 0; x < 16; x++)
            for (var y = 0; y < 16; y++)
            for (var z = 0; z < 16; z++)
            {
                var voxelType = this[x, y, z];
                if (voxelType == 0)
                    continue;
                var pos = new Vector3(x, y, z);
                var verticesPos = vertices.Count;
                vertices.AddRange(_cubeVertices.Select(vert => pos + vert));
                triangles.AddRange(_cubeTriangles.Select(tri => verticesPos + tri));
            }

            if (!_meshFilter.sharedMesh) _meshFilter.mesh = new Mesh();
            var mesh = _meshFilter.sharedMesh;
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.RecalculateNormals();
        }
    }
}