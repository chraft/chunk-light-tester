using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChunkRendering;
using CustomGenerator;
using System.Threading;
using System.Diagnostics;

public class WorldBehaviour : MonoBehaviour {

	public static Chunk[] ChunksMap = new Chunk[ushort.MaxValue];
	
	public static Queue<ChunkSliceBuildEntry> ChunkSlicesToBuild = new Queue<ChunkSliceBuildEntry>();
	private static Queue<ChunkSliceBuildEntry> ChunkSlicesWorkingQueue = new Queue<ChunkSliceBuildEntry>();
	
	public static Queue<ChunkSlicesDeleteEntry> SlicesToDelete = new Queue<ChunkSlicesDeleteEntry>();
	private static Queue<ChunkSlicesDeleteEntry> SlicesToDeleteWorking = new Queue<ChunkSlicesDeleteEntry>();
	
	public static object ChunkQueueLock = new object();
	public static object SliceLock = new object();
	
	public static Texture2D AtlasTexture;
	
	public static Material BlockMaterial;
	
	private int accumulator;
	
	private int ChunksNum = 26*26;
	
	private int chunkRendered;
	
	public static readonly int MapMinChunkX = -13;
	public static readonly int MapMaxChunkX = 12;
	
	public static readonly int MapMinCoords = MapMinChunkX * 16;
	public static readonly int MapMaxCoords = MapMaxChunkX * 16;
	
	// Use this for initialization
	void Start () {
		
		BlockMaterial = (Material)Resources.Load ("Materials/BlockVertex", typeof(Material));
		AtlasTexture = (Texture2D)Resources.Load("Textures/terrain");
		
		if(AtlasTexture == null)
			UnityEngine.Debug.Log("Terrain texture not loaded!!!");
		
		GameObject camera = GameObject.Find("Main Camera");
		camera.transform.position = new Vector3(0,160,0);
		
		ChunkMeshThreadEntry[] chunkEntries = new ChunkMeshThreadEntry[ChunksNum];

		ChunkGenManager chunkGenManager = new ChunkGenManager(MapMinChunkX, MapMaxChunkX + 1, 6, this, 13284938921, chunkEntries);
		
		chunkGenManager.Generate();
		
		CalculateStartLight();
		
		LightAlgorithmRecorder.InitPlayback();
		
		for(int x = 0; x < ChunksNum; ++x)
		{
			chunkEntries[x].Init();
			ThreadPool.QueueUserWorkItem(new WaitCallback(chunkEntries[x].ThreadCallback));
		}
	}
	
	public static ushort ChunkIndexFromCoords(int x, int z)
	{
		return (ushort)((x + 127) << 8 | (z + 127));
	}
	
	private void CalculateStartLight()
	{
		for(int x = MapMinChunkX; x < MapMaxChunkX + 1; ++x)
		{
			for(int z = MapMinChunkX; z < MapMaxChunkX + 1; ++z)
			{
				Chunk chunk = ChunksMap[ChunkIndexFromCoords(x,z)];
				chunk.RecalculateSkyLight();
			}
		}
	}
	
	private void CalculateTestLight()
	{
		for(int x = MapMinChunkX; x < MapMaxChunkX; ++x)
		{
			for(int z = MapMinChunkX; z < MapMaxChunkX; ++z)
			{
				Chunk chunk = ChunksMap[ChunkIndexFromCoords(x,z)];
				chunk.RecalculateTestSkyLight();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
			
		if (Input.GetKeyUp(KeyCode.Escape))
			Application.Quit();
		else if(Input.GetKeyUp(KeyCode.L))
			CalculateTestLight();
	}
	
	void FixedUpdate()
	{	
		++accumulator;
		
		if(accumulator == 2) // Each 100ms (1 FixedStep is 50ms)
		{
			lock(ChunkQueueLock)
			{
				Queue<ChunkSliceBuildEntry> temp = ChunkSlicesWorkingQueue;
				ChunkSlicesWorkingQueue = ChunkSlicesToBuild;
				ChunkSlicesToBuild = temp;
			}
			
			for(int i = 0; ChunkSlicesWorkingQueue.Count != 0 && i < 40; ++i)
			{
				ChunkSliceBuildEntry chunkEntry = ChunkSlicesWorkingQueue.Dequeue();
				BuildChunkSliceMesh(chunkEntry);
			}
			
			lock(SliceLock)
			{
				Queue<ChunkSlicesDeleteEntry> temp = SlicesToDeleteWorking;
				SlicesToDeleteWorking = SlicesToDelete;
				SlicesToDelete = temp;
			}
			
			for(int i = 0; SlicesToDeleteWorking.Count != 0; ++i)
			{
				ChunkSlicesDeleteEntry sliceToDelete = SlicesToDeleteWorking.Dequeue();
				
				
				/*for(int s = 0; s < sliceToDelete.indexesToDelete.Length; ++s)
				{
					int index = sliceToDelete.indexesToDelete[s];
					ChunkSlice slice = sliceToDelete.parentChunk.Slices[index];
					if(slice.IsEmpty)
						sliceToDelete.parentChunk.Slices[index] = null;
					
					UnityEngine.Object.Destroy(sliceToDelete.parentChunk.ChunkSliceObjects[index]);
					sliceToDelete.parentChunk.ChunkSliceObjects[index] = null;
				}*/
			}
			
			accumulator = 0;
		}
	}
	
	public void BuildChunkSliceMesh(ChunkSliceBuildEntry chunkEntry)
	{
		// Build the Mesh:
		Mesh mesh = new Mesh();
		mesh.vertices = chunkEntry.Vertices;
		//mesh.SetIndices(chunkEntry.Triangles, MeshTopology.Quads, 0);
		
		mesh.triangles = chunkEntry.Triangles;
		
		mesh.uv = chunkEntry.Uvs;
		
		mesh.colors = chunkEntry.Colors;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds ();
		
		GameObject chunkSliceObject = chunkEntry.ParentChunk.ChunkSliceObjects[chunkEntry.SliceIndex];
		MeshFilter filter = chunkSliceObject.GetComponent<MeshFilter>();
		filter.mesh = mesh;
		
		chunkEntry.ParentChunk.ClearDirtySlices();
	}
	
	public BlockType GetBlockType(int x, int y, int z)
	{	
		int chunkX = x >> 4;
		int chunkZ = z >> 4;
		
		Chunk chunk = GetChunk(chunkX, chunkZ);
		
		if(chunk == null)
			return BlockType.NULL; // We return NULL so that is different from air and we don't build side faces of the blocks
		
		return chunk.GetBlockType(x & 0xF, y, z&0xF);
	}
	
	public Chunk GetChunk(int x, int z)
	{
		return ChunksMap[ChunkIndexFromCoords(x,z)];
	}
}
