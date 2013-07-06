using UnityEngine;
using System.Collections;
using System.Threading;
using ChunkRendering;

public class ChunkThreadEntry {

	private Chunk chunkToBeRendered;
	
	public ChunkThreadEntry(Chunk chunk)
	{
		chunkToBeRendered = chunk;
	}
	
	public void ThreadCallback(object threadContext)
	{
		ChunkRenderer.render(chunkToBeRendered);
	}
}
