using Terraria;

namespace VeinMiner;

static class Utils
{
public static Item GetItemFromTile(int x, int y)
	{
		ITile tile = Main.tile[x, y];
		Item item = new Item();
		int itemIdFromTileId = GetItemIdFromTileId(tile.type);
		item.SetDefaults(itemIdFromTileId);
		item.stack = 1;
		return item;
	}

	private static int GetItemIdFromTileId(int tileId)
	{
		return tileId switch
		{
			1 => 3, 
			0 => 2, 
			40 => 133, 
			53 => 169, 
			25 => 61, 
			203 => 836, 
			117 => 409, 
			147 => 593, 
			161 => 664, 
			59 => 176, 
			60 => 176, 
			7 => 12, 
			166 => 699, 
			6 => 11, 
			167 => 700, 
			9 => 14, 
			168 => 701, 
			8 => 13, 
			169 => 702, 
			22 => 56, 
			204 => 880, 
			37 => 116, 
			56 => 173, 
			58 => 174, 
			107 => 364, 
			221 => 1104, 
			108 => 365, 
			222 => 1105, 
			111 => 366, 
			223 => 1106, 
			211 => 947, 
			408 => 3460, 
			67 => 181, 
			66 => 180, 
			63 => 177, 
			65 => 179, 
			64 => 178, 
			68 => 182, 
			5 => 9, 
			323 => 2504, 
			_ => tileId, 
		};
	}
}