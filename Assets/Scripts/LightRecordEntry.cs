using UnityEngine;
using System.Collections;

public class LightRecordEntry {
	
	public byte BlockX
	{
		get;
		private set;
	}
	
	public byte BlockY
	{
		get;
		private set;
	}
	
	public byte BlockZ
	{
		get;
		private set;
	}
	
	public short ChunkX
	{
		get;
		private set;
	}
	
	public short ChunkZ
	{
		get;
		private set;
	}
	
	public byte PrevLight
	{
		get;
		private set;
	}
	
	public byte NewLight
	{
		get;
		private set;
	}
	
	public LightRecordEntry(int blockX, int blockY, int blockz, int chunkX, int chunkZ, int prevLight, int newLight)
	{
		BlockX = (byte)blockX;
		BlockY = (byte)blockY;
		BlockZ = (byte)blockz;
		
		ChunkX = (short)chunkX;
		ChunkZ = (short)chunkZ;
		
		PrevLight = (byte)prevLight;
		NewLight = (byte)newLight;
	}
}
