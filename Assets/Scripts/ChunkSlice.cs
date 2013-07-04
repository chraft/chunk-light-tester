using UnityEngine;
	
namespace ChunkRendering
{
	public class ChunkSlice
	{
		public int Index;
		public static readonly int SizeX = 16;
		public static readonly int SizeY = Chunk.SliceHeight;
		public static readonly int SizeZ = 16;
		
		private byte[] _Types = new byte[SizeX*SizeY*SizeZ];
		private byte[] _Skylight = new byte[SizeX*SizeY*SizeZ];
		
		public ChunkSlice(int index)
		{
			Index = index;
		}
		
		public byte this[int x, int y, int z]
		{
			get{
				if(x > 15 || x < 0 || y > Chunk.SliceHeightLimit || y < 0 || z > 15 || z < 0)
					return 0;
				
				return _Types[y << 8 | z << 4 | x];
			}
			set{_Types[y << 8 | z << 4 | x] = value;}
		}
	}
}
