using UnityEngine;


public class BlockUVs
{
	public static Rect GetUVFromTypeAndFace(BlockType type, BlockFace face)
	{
		Rect result;
		switch(type)
		{
			case BlockType.Grass:
			{
				switch(face)
				{
					case BlockFace.Top:
					{
						return new Rect(0, 0, 16, 16);
					}
					case BlockFace.Side:
					{
						return new Rect(48, 48, 16, 16);
					}
					case BlockFace.Bottom:
					{
						return new Rect(32, 32, 16, 16);
					}
				}
				break;
			}
			case BlockType.Stone:
			{
				switch(face)
				{
					case BlockFace.Top:
					{
						return new Rect(0, 0, 16, 16);
					}
					case BlockFace.Side:
					{
						return new Rect(48, 48, 16, 16);
					}
					case BlockFace.Bottom:
					{
						return new Rect(32, 32, 16, 16);
					}
				}
				break;
			}
			case BlockType.Log:
			{
				switch(face)
				{
					case BlockFace.Top:
					{
						return new Rect(0, 0, 16, 16);
					}
					case BlockFace.Side:
					{
						return new Rect(48, 48, 16, 16);
					}
					case BlockFace.Bottom:
					{
						return new Rect(32, 32, 16, 16);
					}
				}
				break;
			}
			case BlockType.Leaves:
			{
				switch(face)
				{
					case BlockFace.Top:
					{
						return new Rect(0, 0, 16, 16);
					}
					case BlockFace.Side:
					{
						return new Rect(48, 48, 16, 16);
					}
					case BlockFace.Bottom:
					{
						return new Rect(32, 32, 16, 16);
					}
				}
				break;
			}
		}
		
		return new Rect(0, 0, 16, 16);
	}
}	
