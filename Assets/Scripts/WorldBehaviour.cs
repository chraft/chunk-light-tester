using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChunkRendering;
using CustomGenerator;
using System.Threading;

public class WorldBehaviour : MonoBehaviour {

	public Chunk[] ChunksMap = new Chunk[ushort.MaxValue];
	
	public static Queue<ChunkSliceEntry> ChunkSlicesToBuild = new Queue<ChunkSliceEntry>();
	private static Queue<ChunkSliceEntry> ChunkSlicesWorkingQueue = new Queue<ChunkSliceEntry>();
	
	public static object ChunkQueueLock = new object();
	
	public static Texture2D AtlasTexture;
	
	public static Material BlockMaterial;
	
	private float accumulator;
	
	private int ChunksNum = 21*21;
	
	// Use this for initialization
	void Start () {
		
		BlockMaterial = (Material)Resources.Load ("Materials/BlockVertex", typeof(Material));
		AtlasTexture = (Texture2D)Resources.Load("Textures/terrain");
		
		if(AtlasTexture == null)
			Debug.Log("Terrain texture not loaded!!!");
		
		CustomChunkGenerator chunkGenerator = new CustomChunkGenerator();
		
		chunkGenerator.Init(13284938921);
		
		GameObject camera = GameObject.Find("Main Camera");
		camera.transform.position = new Vector3(0,257,0);
		
		ChunkThreadEntry[] chunkEntries = new ChunkThreadEntry[ChunksNum];
		int i = 0;
		
		ThreadPool.SetMaxThreads(12,12);	
		
		for(int x = -10; x < 11; ++x)
		{
			for(int z = -10; z < 11; ++z)
			{
				ushort index = (ushort)((x + 127) << 8 | (z + 127));
				Chunk chunk = new Chunk(x,z, this, (z + x) % 2 == 0? Color.black : Color.gray);
				ChunksMap[index] = chunk;
				
				chunkGenerator.GenerateChunk(chunk, chunk.X, chunk.Z);
				
				chunkEntries[i] = new ChunkThreadEntry(chunk);
				++i;
			}
		}
		
		
		for(int x = 0; x < ChunksNum; ++x)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(chunkEntries[x].ThreadCallback));
		}
		
		/*for(int x = 0; x < 4; ++x)
			chunkEntries[x].ThreadCallback(null);*/
	}
	
	// Update is called once per frame
	void Update () {
		float deltaTime = Time.deltaTime;
		
		accumulator += deltaTime;
		
		if(accumulator >= 0.1f)
		{
			lock(ChunkQueueLock)
			{
				Queue<ChunkSliceEntry> temp = ChunkSlicesWorkingQueue;
				ChunkSlicesWorkingQueue = ChunkSlicesToBuild;
				ChunkSlicesToBuild = temp;
			}
			
			for(int i = 0; ChunkSlicesWorkingQueue.Count != 0 && i < 40; ++i)
			{
				ChunkSliceEntry chunkEntry = ChunkSlicesWorkingQueue.Dequeue();
				BuildChunkSliceMesh(chunkEntry);
			}
			
			accumulator -= 0.1f;
		}
		
		if (Input.GetKey(KeyCode.Escape))
        	Application.Quit();
	}
	
	public void BuildChunkSliceMesh(ChunkSliceEntry chunkEntry)
	{
		// Build the Mesh:
        Mesh mesh = new Mesh();
        mesh.vertices = chunkEntry.Vertices.ToArray();
        mesh.triangles = chunkEntry.Triangles.ToArray();
		
		/*Vector2[] uvs = new Vector2[chunkEntry.Vertices.Count];
		for (int k = 0; k < uvs.Length; k++)
    		uvs[k] = new Vector2 (chunkEntry.Vertices[k].x, chunkEntry.Vertices[k].z);*/
		
		mesh.uv = chunkEntry.Uvs.ToArray();
		
		mesh.colors = chunkEntry.Colors.ToArray();
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds ();
		
		ChunkBehaviour behaviour = chunkEntry.ParentChunk.ChunkObject.GetComponent<ChunkBehaviour>();
		GameObject chunkSliceObject = behaviour.ChunkSliceObjects[chunkEntry.SliceIndex];
		MeshFilter filter = chunkSliceObject.GetComponent<MeshFilter>();
		filter.mesh = mesh;
	}
	
	public int GetBlockType(int x, int y, int z)
	{	
		int chunkX = x >> 4;
		int chunkZ = z >> 4;
		
		Chunk chunk = GetChunk(chunkX, chunkZ);
		
		if(chunk == null)
			return -1; // We return -1 so that is different from air and we don't build side faces of the blocks
		
		ChunkSlice slice = chunk.Slices[y / Chunk.SliceHeight];
		return slice[x & 0xF, y & Chunk.SliceHeightLimit, z & 0xF];
	}
	
	public Chunk GetChunk(int x, int z)
	{
		return ChunksMap[(x + 127) << 8 | (z + 127)];
	}
}
