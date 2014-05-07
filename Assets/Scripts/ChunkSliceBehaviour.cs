using UnityEngine;
using System.Collections;
using ChunkRendering;

public class ChunkSliceBehaviour : MonoBehaviour {
	public ChunkSlice ParentSlice;
	public static ChunkRenderer chunkRenderer = new ChunkRenderer();
	// Use this for initialization
	void Start () { 
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(ParentSlice.IsDirtyLight() && !ParentSlice.IsProcessingLight())
			chunkRenderer.RenderSliceLight(ParentSlice);
	}
}
