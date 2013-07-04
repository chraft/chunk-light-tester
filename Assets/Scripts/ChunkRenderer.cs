using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace ChunkRendering
{
	class ChunkRenderer
	{
	
	    public static void render(Chunk chunk)
	    {
			WorldBehaviour world = chunk.World;
			
			GameObject camera = GameObject.Find("Main Camera");
			Vector3 pos = camera.transform.position;
			
			Stopwatch watch = new Stopwatch();
			watch.Start();
			
			for(int i = Chunk.NumSlices - 1; i >= 0; --i)
			{
				List<Vector3> vertices = new List<Vector3>();
	        	List<int> triangles = new List<int>();
				List<Color> colors = new List<Color>();
				ChunkSlice chunkSlice = chunk.Slices[i];
				int height = chunkSlice.Index * 16;
				
				if(i < chunk.MinSliceIndex)
					break;
				
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
							
								BlockUVs.GetUVFromTypeAndFace((BlockType)block, BlockFace.Top);
					
		                    }
						
							byte front;
							if(z - 1 < 0)
							{
								int worldX = chunk.X << 4;
								int worldZ = ((chunk.Z - 1) << 4);
								
								front = world.GetBlockType(worldX, y, worldZ);
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
		                    }
						
							byte right;
							
							if(x + 1 > 15)
							{	
								int worldX = ((chunk.X+1) << 4);
								int worldZ = chunk.Z << 4;
								right = world.GetBlockType(worldX, y, worldZ);
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
		                    }
						
							byte back;
							
							if(z + 1 > 15)
							{
								int worldX = chunk.X << 4;
								int worldZ = ((chunk.Z + 1) << 4);
								back = world.GetBlockType(worldX, y, worldZ);
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
		                    }
						
							byte left;
							
							if(x - 1 < 0)
							{
								int worldX = ((chunk.X - 1) << 4);
								int worldZ = chunk.Z << 4;
								left = world.GetBlockType(worldX, y, worldZ);
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
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
		
		                        // first triangle for the block top
		                        triangles.Add(vertexIndex);
		                        triangles.Add(vertexIndex+1);
		                        triangles.Add(vertexIndex+2);
		                        
		                        // second triangle for the block top
		                        triangles.Add(vertexIndex+2);
		                        triangles.Add(vertexIndex+3);
		                        triangles.Add(vertexIndex);
							
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
								colors.Add(Color.gray);
		                    }
		                }
					}
				}
				/*watch.Stop();
				long elapsedPreMesh = watch.ElapsedMilliseconds;
				watch.Start();*/
				
				// Build the Mesh:
		        Mesh mesh = new Mesh();
		        mesh.vertices = vertices.ToArray();
		        mesh.triangles = triangles.ToArray();
				
				Vector2[] uvs = new Vector2[vertices.Count];
        		for (int k = 0; k < uvs.Length; k++)
            		uvs[k] = new Vector2 (vertices[k].x, vertices[k].z);
				
				mesh.uv = uvs;
				
				mesh.colors = colors.ToArray();
				
				mesh.RecalculateNormals();
				mesh.RecalculateBounds ();
				
				/*watch.Stop();
				long elapsedPostMesh = watch.ElapsedMilliseconds;
				watch.Start();*/
				
				ChunkBehaviour behaviour = chunk.ChunkObject.GetComponent<ChunkBehaviour>();
				GameObject chunkSliceObject = behaviour.ChunkSliceObjects[i];
				MeshFilter filter = chunkSliceObject.GetComponent<MeshFilter>();
				filter.mesh = mesh;
				
				//watch.Stop();
				//elapsedPostSet = watch.ElapsedMilliseconds;
				
				/*UnityEngine.Debug.Log("Elapsed Mesh Prepare " + elapsedPreMesh + "ms");
				UnityEngine.Debug.Log("Elapsed Mesh Build " + (elapsedPostMesh - elapsedPreMesh) + "ms");
				UnityEngine.Debug.Log("Elapsed Mesh Set " + (elapsedPostSet - elapsedPostMesh) + "ms");
				UnityEngine.Debug.Log("Total Elapsed " + (elapsedPostSet));*/
			}
			watch.Stop();
			//UnityEngine.Debug.Log ("Total Elapsed " + ((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) + "ms");
	    }
	}
}
