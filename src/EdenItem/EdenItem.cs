using LazyAPI;
using TerrariaApi.Server;using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace EdenItem;

[ApiVersion(2, 1)]
public class EdenItem : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "非小酋";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "瞎写的，不保证能用";
    #endregion

    #region 注册与释放
    public EdenItem(Main game) : base(game) { }

    public override void Initialize()
    {
        // 注册指令
        TShockAPI.Commands.ChatCommands.Add(new Command("eden.si", Commands.SearchItem, "si", "searchitem")
        {
            HelpText = "查找玩家身上是否有指定物品以及数量情况",
            AllowServer = true
        });
        TShockAPI.Commands.ChatCommands.Add(new Command("eden.ri", Commands.RemoveItem, "ri", "removeitem")
        {
            HelpText = "移除玩家身上所有的指定物品",
            AllowServer = true
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 移除指令
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.SearchItem);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.RemoveItem);
        }
        base.Dispose(disposing);
    }
    #endregion
}