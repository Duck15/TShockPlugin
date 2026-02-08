using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace GodTime.TaskSystem;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
	public static ThreadStart threadStart = closeGod;

	private Thread thread = new Thread(threadStart);

	public Config config = new Config();

	public static List<TSPlayer> godTimePlayer = new List<TSPlayer>();

	public override string Author => "Cai";

	public override string Description => "复活为玩家添加一段无敌时间";

	public override string Name => "复活无敌时间";

	public override Version Version => new Version(1, 0, 0, 0);

	public string ConfigPath => Path.Combine(TShock.SavePath, "复活无敌时间.json");

	public static bool threadOpen { get; set; } = true;

	public MainPlugin(Main game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		ReadConfig();
		threadOpen = true;
		ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
		GeneralHooks.ReloadEvent += Reload;
		GetDataHandlers.KillMe += new EventHandler<GetDataHandlers.KillMeEventArgs>(OnPlayerKilled);
		GetDataHandlers.PlayerSpawn += new EventHandler<GetDataHandlers.SpawnEventArgs>(OnPlayerSpawn);
		ServerApi.Hooks.ServerLeave.Register(this, OnPlayerLeave);
	}

	private void OnInitialize(EventArgs args)
	{
		thread.IsBackground = true;
		thread.Start();
	}

	private void OnPlayerLeave(LeaveEventArgs args)
	{
		godTimePlayer.RemoveAll((TSPlayer x) => x.Index == args.Who);
	}

	public static void closeGod()
	{
		while (threadOpen)
		{
			try
			{
				TSPlayer[] players = TShock.Players;
				foreach (TSPlayer plr in players)
				{
					if (plr != null && !plr.HasPermission("godcheck.ignore") && !plr.GodMode && !godTimePlayer.Exists((TSPlayer x) => x.Index == plr.Index))
					{
						CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(plr.Index, state: false);
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.Message);
			}
			Thread.Sleep(5000);
		}
	}

	private void OnPlayerSpawn(object? sender, GetDataHandlers.SpawnEventArgs args)
	{
		Task.Run(async delegate
		{
			if (!args.Player.GodMode && godTimePlayer.Exists((TSPlayer x) => x.Index == args.Player.Index))
			{
				bool boss = false;
				NPC[] npc = Main.npc;
				foreach (NPC npc2 in npc)
				{
					Player plr = Main.player[args.PlayerId];
					if (npc2.active && (npc2.boss || npc2.type == 13 || npc2.type == 14 || npc2.type == 15) && Math.Abs(plr.Center.X - npc2.Center.X) + Math.Abs(plr.Center.Y - npc2.Center.Y) < 4000f)
					{
						boss = true;
						break;
					}
				}
				CreativePowers.GodmodePower godPower = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
				godPower.SetEnabledState(args.Player.Index, state: true);
				if (boss)
				{
					args.Player.SendWarningMessage("[i:29]你处于BOSS战斗中, 获得[c/32FF82:3]秒无敌时间!");
					await Task.Delay(3000);
				}
				else
				{
					args.Player.SendInfoMessage($"[i:29]你已复活, 获得[c/32FF82:{config.time}]秒无敌时间!");
					await Task.Delay(config.time * 1000);
				}
				if (godTimePlayer.Exists((TSPlayer x) => x.Index == args.Player.Index))
				{
					removeGodTime(args.Player);
					args.Player.SendInfoMessage("[i:29]无敌时间已结束!");
				}
			}
		});
	}

	public static void removeGodTime(TSPlayer player)
	{
		CreativePowers.GodmodePower power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
		power.SetEnabledState(player.Index, state: false);
		godTimePlayer.RemoveAll((TSPlayer x) => x.Index == player.Index);
	}

	public void OnPlayerKilled(object? sender, GetDataHandlers.KillMeEventArgs args)
	{
		if (!args.Player.GodMode)
		{
			godTimePlayer.Add(args.Player);
		}
	}

	public void Reload(ReloadEventArgs args)
	{
		ReadConfig();
		args.Player.SendSuccessMessage("[玩家复活无敌]配置文件已重载!");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
			GeneralHooks.ReloadEvent -= Reload;
			threadOpen = false;
		}
		base.Dispose(disposing);
	}

	public void ReadConfig()
	{
		try
		{
			config = Config.Read(ConfigPath).Write(ConfigPath);
		}
		catch (Exception ex)
		{
			config = new Config();
			TShock.Log.ConsoleError("[玩家复活无敌] 读取配置文件发生错误!\n{0}".SFormat(ex.ToString()));
		}
	}
}
