using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

namespace ChunkRendering
{
	class ChunkRenderer
	{
		private static Color firstSideColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
		private static Color secondSideColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
		private static Color topColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		private static Color bottomColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	    public static void render(Chunk chunk)
	    {
			WorldBehaviour world = chunk.World;
			
/*			Stopwatch watch = new Stopwatch();
			watch.Start();*/
			
			int minSliceIndex = chunk.MinSliceIndex;
				
			Chunk frontChunk = world.GetChunk(chunk.X, chunk.Z - 1);
			Chunk backChunk = world.GetChunk(chunk.X, chunk.Z + 1);
			Chunk leftChunk = world.GetChunk(chunk.X - 1, chunk.Z);
			Chunk rightChunk = world.GetChunk(chunk.X + 1, chunk.Z);
			
			if(frontChunk != null && frontChunk.MinSliceIndex < minSliceIndex)
				minSliceIndex = frontChunk.MinSliceIndex;
			
			if(backChunk != null && backChunk.MinSliceIndex < minSliceIndex)
				minSliceIndex = backChunk.MinSliceIndex;
			
			if(leftChunk != null && leftChunk.MinSliceIndex < minSliceIndex)
				minSliceIndex = leftChunk.MinSliceIndex;
			
			if(rightChunk != null && rightChunk.MinSliceIndex < minSliceIndex)
				minSliceIndex = rightChunk.MinSliceIndex;
			
			
			for(int i = Chunk.NumSlices - 1; i >= 0; --i)
			{
				List<Vector3> vertices = new List<Vector3>();
	        	List<int> triangles = new List<int>();
				List<Color> colors = new List<Color>();
				List<Vector2> uvs = new List<Vector2>();
				ChunkSlice chunkSlice = chunk.Slices[i];
				
				
				
				if(i < minSliceIndex)
					break;
				
				float epsilon = 0.00f;
				
				/*watch.Reset();
				watch.Start();*/
				
		        for (int x = 0; x < 16; x++)
				{
		            for (int z = 0; z < 16; z++)
					{
		                for (int y = 0; y < Chunk.SliceHeight; y++)
		                {								
		                    byte block = chunkSlice[x, y, z];
							byte top;
							
							if(block == 0)
								continue;
							
							if(y + 1 > Chunk.SliceHeightLimit)
							{
								if(i + 1 > Chunk.MaxSliceIndex)
									top = 0;
								else
								{
									ChunkSlice topSlice = chunk.Slices[i + 1];
									top = topSlice[x, (y + 1) & Chunk.SliceHeightLimit, z];
								}
							}
		                    else
								top = chunkSlice[x, y + 1, z];
		
		                    // we are checking the top face of the block, so see if the top is exposed
		                    if (top == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x, y + 1, z));
		                        vertices.Add(new Vector3(x, y + 1, z + 1));
		                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
								vertices.Add(new Vector3(x + 1, y + 1, z));

		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(topColor);
								colors.Add(topColor);
								colors.Add(topColor);
								colors.Add(topColor);
							
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Top);
								
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMax, yMin));							
								uvs.Add(new Vector2(xMax, yMax));
		                    }
						
							int front;
							if(z - 1 < 0)
							{
								int worldX = (chunk.X << 4) + x;
								int worldZ = (chunk.Z << 4) - 1;
								int worldY = (chunkSlice.Index * ChunkSlice.SizeY) + y;
								
								front = world.GetBlockType(worldX, worldY, worldZ);
							}
							else
								front = chunkSlice[x, y, z - 1];
						
							if (front == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x, y, z));
		                        vertices.Add(new Vector3(x, y + 1, z));
		                        vertices.Add(new Vector3(x + 1, y + 1, z));
		                        vertices.Add(new Vector3(x + 1, y, z));
								
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                     
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);

								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Side);
								/*uvs.Add(new Vector2(coords.x + epsilon, coords.y + epsilon));
								uvs.Add(new Vector2(coords.x + epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + epsilon));*/
								
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMax, yMax));							
								uvs.Add(new Vector2(xMax, yMin));
		                    }
						
							int right;
							
							if(x + 1 > 15)
							{	
								int worldX = (chunk.X << 4) + 16;
								int worldZ = (chunk.Z << 4) + z;
								int worldY = (chunkSlice.Index * ChunkSlice.SizeY) + y;
								
								right = world.GetBlockType(worldX, worldY, worldZ);
							}
							else
								right = chunkSlice[x + 1, y, z];
		
		                    if (right == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x + 1, y, z));
		                        vertices.Add(new Vector3(x + 1, y + 1, z));
		                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
		                        vertices.Add(new Vector3(x + 1, y, z + 1));
		
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Side);
								
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMax, yMax));							
								uvs.Add(new Vector2(xMax, yMin));
								/*uvs.Add(new Vector2(coords.x + epsilon, coords.y + epsilon));
								uvs.Add(new Vector2(coords.x + epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + epsilon));*/
		                    }
						
							int back;
							
							if(z + 1 > 15)
							{
								int worldX = (chunk.X << 4) + x;
								int worldZ = (chunk.Z << 4) + 16;
								int worldY = (chunkSlice.Index * ChunkSlice.SizeY) + y;
								
								back = world.GetBlockType(worldX, worldY, worldZ);
							}
							else
								back = chunkSlice[x, y, z + 1];
						
							if (back == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x + 1, y, z + 1));
		                        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
		                        vertices.Add(new Vector3(x, y + 1, z + 1));
		                        vertices.Add(new Vector3(x, y, z + 1));
		
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								colors.Add(firstSideColor);
								
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Side);
								
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMax, yMax));							
								uvs.Add(new Vector2(xMax, yMin));
								
								/*uvs.Add(new Vector2(coords.x + epsilon, coords.y + epsilon));
								uvs.Add(new Vector2(coords.x + epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + epsilon));*/
		                    }
						
							int left;
							
							if(x - 1 < 0)
							{
								int worldX = (chunk.X << 4) - 1;
								int worldZ = (chunk.Z << 4) + z;
								int worldY = (chunkSlice.Index * ChunkSlice.SizeY) + y;
								
								left = world.GetBlockType(worldX, worldY, worldZ);
							}
							else
								left = chunkSlice[x - 1, y, z];
						
							if (left == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x, y, z + 1));
		                        vertices.Add(new Vector3(x, y + 1, z + 1));
		                        vertices.Add(new Vector3(x, y + 1, z));
		                        vertices.Add(new Vector3(x, y, z));
		
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								colors.Add(secondSideColor);
								
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Side);
								
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMax, yMax));							
								uvs.Add(new Vector2(xMax, yMin));
								/*uvs.Add(new Vector2(coords.x + epsilon, coords.y + epsilon));
								uvs.Add(new Vector2(coords.x + epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + epsilon));*/
		                    }
						
							byte bottom;
							
							if(y - 1 < 0)
								bottom = 1;
							else
								bottom = chunkSlice[x, y - 1, z];
						
							if (bottom == 0)
		                    {
		                        int vertexIndex = vertices.Count;
		                        vertices.Add(new Vector3(x, y, z + 1));
		                        vertices.Add(new Vector3(x, y, z));
		                        vertices.Add(new Vector3(x + 1, y, z));
		                        vertices.Add(new Vector3(x + 1, y, z + 1));
		
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(bottomColor);
								colors.Add(bottomColor);
								colors.Add(bottomColor);
								colors.Add(bottomColor);
								
								Rect coords = BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Bottom);
								float yMax = coords.y + coords.height - epsilon;
								float xMax = coords.x + coords.width - epsilon;
								float xMin = coords.x + epsilon;
								float yMin = coords.y + epsilon;
								
								uvs.Add(new Vector2(xMin, yMin));
								uvs.Add(new Vector2(xMin, yMax));
								uvs.Add(new Vector2(xMax, yMax));							
								uvs.Add(new Vector2(xMax, yMin));
								
								/*uvs.Add(new Vector2(coords.x + epsilon, coords.y + epsilon));
								uvs.Add(new Vector2(coords.x + epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + coords.height - epsilon));
								uvs.Add(new Vector2(coords.x + coords.width - epsilon, coords.y + epsilon));*/
		                    }
		                }
					}
				}
				/*watch.Stop();
				long elapsedPreMesh = watch.ElapsedMilliseconds;
				watch.Start();*/
				
				ChunkSliceEntry chunkEntry = new ChunkSliceEntry();
				chunkEntry.Vertices = vertices;
				chunkEntry.Triangles = triangles;
				chunkEntry.Colors = colors;
				chunkEntry.Uvs = uvs;
				chunkEntry.ParentChunk = chunk;
				chunkEntry.SliceIndex = i;
				
				lock(WorldBehaviour.ChunkQueueLock)
					WorldBehaviour.ChunkSlicesToBuild.Enqueue(chunkEntry);
				
				//watch.Stop();
				//elapsedPostSet = watch.ElapsedMilliseconds;
				
				/*UnityEngine.Debug.Log("Elapsed Mesh Prepare " + elapsedPreMesh + "ms");
				UnityEngine.Debug.Log("Elapsed Mesh Build " + (elapsedPostMesh - elapsedPreMesh) + "ms");
				UnityEngine.Debug.Log("Elapsed Mesh Set " + (elapsedPostSet - elapsedPostMesh) + "ms");
				UnityEngine.Debug.Log("Total Elapsed " + (elapsedPostSet));*/
			}
			//watch.Stop();
			//UnityEngine.Debug.Log ("Total Elapsed " + ((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) + "ms");
	    }
	}
}
