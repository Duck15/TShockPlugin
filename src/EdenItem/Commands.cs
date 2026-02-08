using System.Text;
using Terraria;
using Terraria.ID;
using TShockAPI;using Terraria;
using Terraria.GameContent.Creative;
using TShockAPI;

namespace EdenItem;

public class Commands
{
    /// <summary>
    /// 查找玩家身上是否有指定物品以及数量情况
    /// </summary>
    /// <param name="args">指令参数</param>
    public static void SearchItem(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage("用法: /si <物品名称/ID> <玩家/索引/all>");
            return;
        }

        // 解析物品
        string itemInput = args.Parameters[0];
        Item item;
        if (!TryGetItem(itemInput, out item))
        {
            args.Player.SendErrorMessage($"无法找到物品: {itemInput}");
            return;
        }

        // 解析玩家
        List<TSPlayer> players;
        if (!TryGetPlayers(args.Parameters[1], out players))
        {
            args.Player.SendErrorMessage($"无法找到玩家: {args.Parameters[1]}");
            return;
        }

        // 查找物品
        foreach (var plr in players)
        {
            if (plr == null || !plr.IsLoggedIn || !plr.Active)
                continue;

            int totalCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"玩家 {plr.Name} 的 {item.Name} 数量:");

            // 检查背包
            int inventoryCount = CountItemsInInventory(plr.TPlayer.inventory, item.type);
            if (inventoryCount > 0)
            {
                sb.AppendLine($"  背包: {inventoryCount}");
                totalCount += inventoryCount;
            }

            // 检查银行
            int bankCount = CountItemsInInventory(plr.TPlayer.bank.item, item.type);
            if (bankCount > 0)
            {
                sb.AppendLine($"  银行: {bankCount}");
                totalCount += bankCount;
            }

            // 检查银行2
            int bank2Count = CountItemsInInventory(plr.TPlayer.bank2.item, item.type);
            if (bank2Count > 0)
            {
                sb.AppendLine($"  银行2: {bank2Count}");
                totalCount += bank2Count;
            }

            // 检查银行3
            int bank3Count = CountItemsInInventory(plr.TPlayer.bank3.item, item.type);
            if (bank3Count > 0)
            {
                sb.AppendLine($"  银行3: {bank3Count}");
                totalCount += bank3Count;
            }

            // 检查银行4
            int bank4Count = CountItemsInInventory(plr.TPlayer.bank4.item, item.type);
            if (bank4Count > 0)
            {
                sb.AppendLine($"  银行4: {bank4Count}");
                totalCount += bank4Count;
            }

            // 检查虚空袋
            int voidBagCount = CountItemsInInventory(plr.TPlayer.bank4.item, item.type);
            if (voidBagCount > 0)
            {
                sb.AppendLine($"  虚空袋: {voidBagCount}");
                totalCount += voidBagCount;
            }

            // 检查垃圾桶
            if (plr.TPlayer.trashItem.type == item.type)
            {
                sb.AppendLine($"  垃圾桶: {plr.TPlayer.trashItem.stack}");
                totalCount += plr.TPlayer.trashItem.stack;
            }

            // 发送结果
            if (totalCount > 0)
            {
                sb.AppendLine($"  总计: {totalCount}");
                args.Player.SendMessage(sb.ToString(), 0, 255, 0); // 绿色
            }
            else
            {
                args.Player.SendMessage($"玩家 {plr.Name} 没有 {item.Name}", 255, 0, 0); // 红色
            }
        }
    }

    /// <summary>
    /// 移除玩家身上所有的指定物品
    /// </summary>
    /// <param name="args">指令参数</param>
    public static void RemoveItem(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage("用法: /ri <物品名称/ID> <玩家/索引/all>");
            return;
        }

        // 解析物品
        string itemInput = args.Parameters[0];
        Item item;
        if (!TryGetItem(itemInput, out item))
        {
            args.Player.SendErrorMessage($"无法找到物品: {itemInput}");
            return;
        }

        // 解析玩家
        List<TSPlayer> players;
        if (!TryGetPlayers(args.Parameters[1], out players))
        {
            args.Player.SendErrorMessage($"无法找到玩家: {args.Parameters[1]}");
            return;
        }

        // 移除物品
        foreach (var plr in players)
        {
            if (plr == null || !plr.IsLoggedIn || !plr.Active)
                continue;

            int totalRemoved = 0;

            // 移除背包中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.inventory, item.type);

            // 移除银行中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.bank.item, item.type);

            // 移除银行2中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.bank2.item, item.type);

            // 移除银行3中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.bank3.item, item.type);

            // 移除银行4中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.bank4.item, item.type);

            // 移除虚空袋中的物品
            totalRemoved += RemoveItemsFromInventory(plr, plr.TPlayer.bank4.item, item.type);

            // 移除垃圾桶中的物品
            if (plr.TPlayer.trashItem.type == item.type)
            {
                totalRemoved += plr.TPlayer.trashItem.stack;
                plr.TPlayer.trashItem.TurnToAir();
                plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, PlayerItemSlotID.TrashItem);
            }

            // 发送结果
            if (totalRemoved > 0)
            {
                args.Player.SendMessage($"已从玩家 {plr.Name} 身上移除 {totalRemoved} 个 {item.Name}", 0, 255, 0); // 绿色
                plr.SendMessage($"管理员 {args.Player.Name} 已从你身上移除 {totalRemoved} 个 {item.Name}", 255, 0, 0); // 红色
            }
            else
            {
                args.Player.SendMessage($"玩家 {plr.Name} 身上没有 {item.Name}", 255, 0, 0); // 红色
            }
        }
    }

    /// <summary>
    /// 尝试获取物品
    /// </summary>
    /// <param name="input">物品名称或ID</param>
    /// <param name="item">获取到的物品</param>
    /// <returns>是否成功获取</returns>
    private static bool TryGetItem(string input, out Item item)
    {
        item = new Item();

        // 尝试通过ID获取
        if (int.TryParse(input, out int itemId))
        {
            var foundItem = TShock.Utils.GetItemById(itemId);
            if (foundItem != null)
            {
                item = foundItem;
                return true;
            }
            return false;
        }

        // 尝试通过名称获取
        var items = TShock.Utils.GetItemByIdOrName(input);
        if (items.Count == 1)
        {
            item = items[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// 尝试获取玩家列表
    /// </summary>
    /// <param name="input">玩家名称、索引或all</param>
    /// <param name="players">获取到的玩家列表</param>
    /// <returns>是否成功获取</returns>
    private static bool TryGetPlayers(string input, out List<TSPlayer> players)
    {
        players = new List<TSPlayer>();

        // 检查是否为all
        if (input.Equals("*all*", StringComparison.OrdinalIgnoreCase) || input.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            players.AddRange(TShock.Players.Where(p => p != null && p.IsLoggedIn && p.Active));
            return players.Count > 0;
        }

        // 尝试通过索引获取
        if (int.TryParse(input, out int index))
        {
            var plr = TShock.Players[index];
            if (plr != null && plr.IsLoggedIn && plr.Active)
            {
                players.Add(plr);
                return true;
            }
            return false;
        }

        // 尝试通过名称获取
        var foundPlayer = TShock.Players.FirstOrDefault(p => p != null && p.IsLoggedIn && p.Active && p.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
        if (foundPlayer != null)
        {
            players.Add(foundPlayer);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 计算物品栏中指定物品的数量
    /// </summary>
    /// <param name="inventory">物品栏</param>
    /// <param name="itemType">物品类型</param>
    /// <returns>物品数量</returns>
    private static int CountItemsInInventory(Item[] inventory, int itemType)
    {
        int count = 0;
        foreach (var invItem in inventory)
        {
            if (invItem != null && invItem.type == itemType && invItem.stack > 0)
            {
                count += invItem.stack;
            }
        }
        return count;
    }

    /// <summary>
    /// 从物品栏中移除指定物品
    /// </summary>
    /// <param name="plr">玩家</param>
    /// <param name="inventory">物品栏</param>
    /// <param name="itemType">物品类型</param>
    /// <returns>移除的物品数量</returns>
    private static int RemoveItemsFromInventory(TSPlayer plr, Item[] inventory, int itemType)
    {
        int removed = 0;

        for (int i = 0; i < inventory.Length; i++)
        {
            var invItem = inventory[i];
            if (invItem.type == itemType && invItem.stack > 0)
            {
                removed += invItem.stack;
                invItem.TurnToAir();
                
                // 发送更新
                int slotId = i;
                if (inventory == plr.TPlayer.bank.item)
                    slotId += 58; // Bank1_1
                else if (inventory == plr.TPlayer.bank2.item)
                    slotId += 84; // Bank2_1
                else if (inventory == plr.TPlayer.bank3.item)
                    slotId += 110; // Bank3_1
                else if (inventory == plr.TPlayer.bank4.item)
                    slotId += 136; // Bank4_1 (VoidVault_1)

                plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, slotId);
            }
        }

        return removed;
    }
}