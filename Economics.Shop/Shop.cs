﻿using Economics.RPG.Extensions;
using Economics.Shop.Model;
using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using EconomicsAPI.Extensions;
using Org.BouncyCastle.Bcpg;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Shop;

[ApiVersion(2, 1)]
public class Shop : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Shop.json");

    internal static Config Config { get; set; }
    public Shop(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        GeneralHooks.ReloadEvent += (_) => Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    public static bool HasItem(TSPlayer player, List<ItemTerm> itemTerms)
    {
        foreach (ItemTerm term in itemTerms)
        {
            var count = 0;
            CheckBanksForItem(player, term.netID, ref count);
            if (count < term.Stack)
                return false;

        }
        ConsumeItem(player, itemTerms);
        return true;
    }

    private static void ConsumeItem(TSPlayer player, List<ItemTerm> terms)
    {
        foreach (var term in terms)
        {
            var stack = term.Stack;
            for (int j = 0; j < player.TPlayer.inventory.Length; j++)
            {
                var item = player.TPlayer.inventory[j];
                if (item.netID == term.netID)
                {
                    if (item.stack >= stack)
                    {
                        item.stack -= stack;
                        TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", player.Index, j);
                    }
                    else
                    {
                        stack -= item.stack;
                    }
                }
            }
        }
    }

    private static void CheckBanksForItem(TSPlayer player, int itemId, ref int itemCount)
    {
        for (int j = 0; j < player.TPlayer.inventory.Length; j++)
        {
            if (player.TPlayer.inventory[j].type == itemId)// 检查猪猪储钱罐
            {
                itemCount += player.TPlayer.bank.item[j].stack;
            }
        }
    }

    [CommandMap("shop", "economics.shop")]
    public static void CShop(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                return;

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Config.PageMax,
                        NothingToDisplayString = "当前商店没有商品",
                        HeaderFormat = "商品列表 ({0}/{1})：",
                        FooterFormat = "输入 {0}shop list {{0}} 查看更多".SFormat(Commands.Specifier)
                    }
                );
        }
        if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var lines = new List<string>();
            var index = 1;
            foreach (var product in Config.Products)
            {
                lines.Add($"{index}:[{product.Name}]  {string.Join(" ", product.Items.Select(x => x.ToString()))} 价格:{product.Cost}");
                index++;
            }
            Show(lines);
        }
        else if (args.Parameters.Count >= 2 && args.Parameters[0].ToLower() == "buy")
        {
            if (!int.TryParse(args.Parameters[1], out var index))
            {
                args.Player.SendErrorMessage("请输入正确的序号!");
                return;
            }
            var count = 1;
            if (!int.TryParse(args.Parameters[2], out count))
            {
                args.Player.SendErrorMessage("你输入的购买数量不正确!");
                return;
            }
            var product = Config.GetProduct(index);
            if (product == null)
            {
                args.Player.SendErrorMessage("此商品不存在，请检查序号后重新输入!");
                return;
            }
            if (!args.Player.InProgress(product.ProgressLimit))
            {
                args.Player.SendErrorMessage($"购买此商品需满足进度条件: {string.Join(",", product.ProgressLimit)}");
                return;
            }
            if (!args.Player.InLevel(product.LevelLimit))
            {
                args.Player.SendErrorMessage($"购买此商品需达到以下等级之一: {string.Join(",", product.LevelLimit)}");
                return;
            }
            if (!HasItem(args.Player, product.ItemTerm))
            {
                args.Player.SendErrorMessage($"请满足物品条件: {string.Join(",", product.ItemTerm.Select(x => x.ToString()))}");
                return;
            }
            if (!EconomicsAPI.Economics.CurrencyManager.DelUserCurrency(args.Player.Name, product.Cost))
            {
                args.Player.SendErrorMessage($"你的{EconomicsAPI.Economics.Setting.CurrencyName}不足!");
                return;
            }
            for (int i = 0; i < count; i++)
            { 
                args.Player.GiveItems(product.Items);
            }
            
            args.Player.ExecCommand(product.Commamds);
            args.Player.SendSuccessMessage("购买成功!");
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage("/shop buy [序号]");
            args.Player.SendInfoMessage("/shop list [序号]");
        }
        else
        {
            args.Player.SendInfoMessage("输入/shop help 查看指令帮助");
        }
    }
}
