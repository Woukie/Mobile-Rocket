﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Environment
{
    public class TerrainMeshGenerator : MonoBehaviour
    {
        public Transform CameraTransform;
        public Material TerrainMaterial;

        public float TerrainScale1;
        public float TerrainScale2;
        public int WidthResolution;
        public int DepthResolution;
        public int ChunkWidth;
        public int ChunkDepth;
        public float Amplitude1;
        public float Amplitude2;
        public int NumberOfChunks;

        private GameObject[] _chunks;
        private int[] _currentChunkPositions;
        private int _lastChunkPosition;

        private void Start()
        {
            _currentChunkPositions = new int[NumberOfChunks];
            _chunks = new GameObject[NumberOfChunks];

            var magicNumberWidth = (ChunkWidth + ChunkWidth / WidthResolution) * (1f / (WidthResolution + 1));
            var magicNumberDepth = (ChunkDepth + ChunkDepth / DepthResolution) * (1f / (DepthResolution + 1));

            for (var i = 0; i < NumberOfChunks; i++)
            {
                _chunks[i] = CreateEmptyChunk(i.ToString(), magicNumberWidth, magicNumberDepth);
                _chunks[i].transform.position = new Vector3(ChunkWidth * ((int)(CameraTransform.position.x / ChunkWidth) + i - NumberOfChunks / 2), 0, 0);
                UpdateGeometry(_chunks[i]);
            }
        }

        private void Update()
        {
            var currentChunkPosition = ChunkWidth * (int) (CameraTransform.position.x / ChunkWidth) / (2 * WidthResolution);
            if (currentChunkPosition == _lastChunkPosition)
            {
                return;
            }

            var chunksToBeMoved = new List<int>();
            var missingChunks = new List<int>();

            var targetChunkPositions = new int[NumberOfChunks];

            for (var i = 0; i < NumberOfChunks; i++)
            {
                _currentChunkPositions[i] = (int)_chunks[i].transform.position.x;
                targetChunkPositions[i] = ChunkWidth * ((int)(CameraTransform.position.x / ChunkWidth) + i - NumberOfChunks / 2);
            }

            // If one of the current chunk positions are not in the right place, mark them to be moved
            for (var i = 0; i < NumberOfChunks; i++)
            {
                // Check if a chunk exists when it shouldn't
                if (!targetChunkPositions.Contains(_currentChunkPositions[i]))
                {
                    chunksToBeMoved.Add(i);
                }

                // Check if the target chunk already exists
                if (!_currentChunkPositions.Contains(targetChunkPositions[i]))
                {
                    missingChunks.Add(i);
                }
            }

            foreach (var chunkToBeMoved in chunksToBeMoved)
            {
                _chunks[chunkToBeMoved].transform.position = new Vector3(targetChunkPositions[missingChunks[0]], 0, 0);
                UpdateGeometry(_chunks[chunkToBeMoved]);
                missingChunks.RemoveAt(0);
            }

            _lastChunkPosition = ChunkWidth * (int) (CameraTransform.position.x / ChunkWidth) / (2 * WidthResolution);
        }

        private void UpdateGeometry(GameObject chunk)
        {
            var chunkMesh = chunk.GetComponent<MeshFilter>().mesh;
            var newChunkMeshVertices = new List<Vector3>();

            foreach (var meshVertex in chunkMesh.vertices)
            {
                // For region specific terrain, optimize by considering it within (only) the area you want it
                var x = meshVertex.x + chunk.transform.position.x;
                var z = meshVertex.z + chunk.transform.position.z;

                var startHill = x < 50 && x > -50 ? 10 / ((x * x + z * z) / 100 + 1) : 0;
                var noise1 = Perlin.Noise(x * TerrainScale1, z * TerrainScale1);
                var noise2 = Perlin.Noise(x * TerrainScale2, z * TerrainScale2);

                var curvature = meshVertex.z * meshVertex.z / -100;

                var yVal = noise1 * Amplitude1 + noise2 * Amplitude2 + curvature + startHill;

                newChunkMeshVertices.Add(new Vector3(meshVertex.x, yVal, meshVertex.z));
            }

            chunkMesh.vertices = newChunkMeshVertices.ToArray();
            chunkMesh.RecalculateNormals();

            var chunkMeshCollider = chunk.GetComponent<MeshCollider>();
            chunkMeshCollider.sharedMesh = chunkMesh;
        }

        private GameObject CreateEmptyChunk(string chunkName, float a, float b)
        {
            var chunkGameObject = new GameObject(chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

            var vertices = new List<Vector3>();
            var triangles = new int[6 * WidthResolution * DepthResolution];

            for (var z = 0; z < DepthResolution + 1; z++)
            {
                for (var x = 0; x < WidthResolution + 1; x++)
                {
                    vertices.Add(new Vector3(x * a - (float)(ChunkWidth + ChunkWidth / WidthResolution) / 2, 0, z * b - (float)(ChunkDepth + ChunkDepth / DepthResolution) / 2));
                }
            }

            var triangleToCalculate = 0;

            for (var i = 0; i < WidthResolution * DepthResolution; i++)
            {
                var squareNumber = triangleToCalculate / 2;
                var cornerVertex = squareNumber + squareNumber / WidthResolution;

                triangles[3 * triangleToCalculate] = cornerVertex;
                triangles[3 * triangleToCalculate + 1] = cornerVertex + WidthResolution + 1;
                triangles[3 * triangleToCalculate + 2] = cornerVertex + WidthResolution + 2;

                triangleToCalculate += 1;

                triangles[3 * triangleToCalculate] = cornerVertex;
                triangles[3 * triangleToCalculate + 1] = cornerVertex + WidthResolution + 2;
                triangles[3 * triangleToCalculate + 2] = cornerVertex + 1;

                triangleToCalculate += 1;
            }

            var chunkMesh = chunkGameObject.GetComponent<MeshFilter>().mesh;

            chunkGameObject.GetComponent<MeshRenderer>().material = TerrainMaterial;
            chunkMesh.vertices = vertices.ToArray();
            chunkMesh.triangles = triangles;

            var uvs = new Vector2[vertices.Count];
            for (var i = 0; i < vertices.Count; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            chunkMesh.uv = uvs;

            chunkMesh.RecalculateNormals();

            return chunkGameObject;
        }
    }
}
