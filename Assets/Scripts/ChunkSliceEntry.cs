using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkSliceEntry {
	public List<Vector3> Vertices;
	public List<int> Triangles;
	public List<Color> Colors;
	public List<Vector2> Uvs;
	public Chunk ParentChunk;
	public int SliceIndex;
}
