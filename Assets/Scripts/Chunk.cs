using UnityEngine;
using System.Collections;
using ChunkRendering;
using System;

public class Chunk
{
	public static readonly int SliceHeight = 16;
	
	public static readonly int NumSlices = 256 / SliceHeight;
	public static readonly int MaxSliceIndex = NumSlices - 1;
	public static readonly int SliceHeightLimit = SliceHeight - 1;
	public ChunkSlice[] Slices = new ChunkSlice[NumSlices];
	public GameObject ChunkObject;
	public WorldBehaviour World;
	public int X;
	public int Z;
	public Color ChunkColor;
	public int MinSliceIndex;
	
	private int lowestY;
	
	private byte [,] HeightMap;
	
	public Chunk(int chunkX, int chunkZ, WorldBehaviour world, Color color)
	{
		ChunkColor = color;
		ChunkObject = new GameObject(String.Format("X {0} Z {1}", chunkX, chunkZ));
		ChunkBehaviour behaviour = ChunkObject.AddComponent(typeof(ChunkBehaviour)) as ChunkBehaviour;
		behaviour.Parent = this;
		ChunkObject.transform.position = new Vector3(chunkX*16,0,chunkZ*16);
		X = chunkX;
		Z = chunkZ;
		World = world;
		
		for(int i = 0; i < NumSlices; ++i)
			Slices[i] = new ChunkSlice(i);
		
		/*System.Random rand = new System.Random();
		int lowestY = 255;
		for (int x=0; x < 16; x++)
            for (int z=0; z< 16; z++)
			{ bool lowestColumnYFound = false;
                for (int y = 255; y >= 0; --y)
				{
					byte block = (byte)rand.Next(2);
                	Slices[y/Chunk.SliceHeight][x,y & Chunk.SliceHeightLimit,z] = block;
				
					if(block == 1 && !lowestColumnYFound)
					{
						if(lowestY > y)
							lowestY = y;
					
						lowestColumnYFound = true;
					}
				}
			}
		
		MinSliceIndex = lowestY / Chunk.SliceHeight;*/
	}
	
	public BlockType GetType(int x, int y, int z)
	{
		ChunkSlice slice = Slices[y / Chunk.SliceHeight];
		return (BlockType)slice[x & 0xF, y & Chunk.SliceHeightLimit, z & 0xF];
	}
	
	public void SetType(int x, int y, int z, BlockType type, bool unused)
	{
		ChunkSlice slice = Slices[y / Chunk.SliceHeight];
		slice[x & 0xF, y & Chunk.SliceHeightLimit, z & 0xF] = (byte)type;
	}
	
	public void SetData(int x, int y, int z, byte data, bool unused)
	{
		
	}
	
	public void RecalculateHeight()
    {
		lowestY = 255;
        HeightMap = new byte[16, 16];
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
                RecalculateHeight(x, z);
        }
		
		MinSliceIndex = (lowestY / Chunk.SliceHeight) - 1;
    }

    public void RecalculateHeight(int x, int z)
    {
        int height;
        BlockType blockType;
        for (height = 127; height > 0 && (GetType(x, height - 1, z) == 0 || (blockType = GetType(x, height - 1, z)) == BlockType.Leaves || blockType == BlockType.Water || blockType == BlockType.Still_Water); height--) ;
        HeightMap[x, z] = (byte)height;

        if (height < lowestY)
            lowestY = height;
    }
}
