using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkSliceBuildEntry {
	public Vector3[] Vertices;
	public int[] Triangles;
	public Color[] Colors;
	public Vector2[] Uvs;
	public Chunk ParentChunk;
	public int SliceIndex;
}
