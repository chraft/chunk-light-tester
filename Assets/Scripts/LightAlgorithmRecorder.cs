using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightAlgorithmRecorder : MonoBehaviour{
	private static LinkedList<LightRecordEntry> recordQueue = new LinkedList<LightRecordEntry>();
	private static LinkedListNode<LightRecordEntry> currNode;
	private static BlockHighliter currentBlock;
	
	public static void InitPlayback()
	{
		currentBlock = new BlockHighliter(0.0f,0.0f, 0.0f);
		currNode = recordQueue.First;
		
		LightRecordEntry startEntry = currNode.Value;
		int worldX = (startEntry.ChunkX << 4) + startEntry.BlockX;
		int worldZ = (startEntry.ChunkZ << 4) + startEntry.BlockZ;
		int worldY = startEntry.BlockY;
		
		currentBlock.SetPosition(worldX, worldY, worldZ);
	}
	
	public static void RecordBlock(int blockX, int blockY, int blockZ, int chunkX, int chunkZ, int prevLight, int newLight)
	{
		LightRecordEntry entry = new LightRecordEntry(blockX, blockY, blockZ, chunkX, chunkZ, prevLight, newLight);
		
		recordQueue.AddLast(entry);
	}
	
	public static void PlayForwardOneStep()
	{
		if(currNode != null)
		{
			if(currNode.Next != null)
				currNode = currNode.Next;
			
			LightRecordEntry currEntry = currNode.Value;
			int worldX = (currEntry.ChunkX << 4) + currEntry.BlockX;
			int worldZ = (currEntry.ChunkZ << 4) + + currEntry.BlockZ;
			int worldY = + currEntry.BlockY;
			
			currentBlock.SetPosition(worldX, worldY, worldZ);
		}
		
	}
	public static void PlayBackOneStep()
	{
		if(currNode != null)
		{
			if(currNode.Previous != null)
				currNode = currNode.Previous;
			
			LightRecordEntry currEntry = currNode.Value;
			int worldX = (currEntry.ChunkX << 4) + currEntry.BlockX;
			int worldZ = (currEntry.ChunkZ << 4) + + currEntry.BlockZ;
			int worldY = + currEntry.BlockY;
			
			currentBlock.SetPosition(worldX, worldY, worldZ);
		}
	}
	
	public void Start()
	{
	}
	
	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.W))
			PlayForwardOneStep();
		else if(Input.GetKeyDown(KeyCode.S))
			PlayBackOneStep();
	}
}
