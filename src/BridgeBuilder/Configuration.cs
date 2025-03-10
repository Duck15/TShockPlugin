﻿using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace BridgeBuilder;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    [LocalizedPropertyName(CultureType.Chinese, "允许快速铺路方块id")]
    [LocalizedPropertyName(CultureType.English, "items")]
    public int[] AllowedTileIDs { get; set; } = { 19, 380, 427, 435, 436, 437, 438, 439 };

    [LocalizedPropertyName(CultureType.Chinese, "允许快速铺路墙壁id")]
    [LocalizedPropertyName(CultureType.English, "walls")]
    public int[] AllowedwallIDs { get; set; } = Array.Empty<int>();

    [LocalizedPropertyName(CultureType.Chinese, "一次性最长铺多少格")]
    [LocalizedPropertyName(CultureType.English, "maxTile")]
    public int MaxPlaceLength { get; set; } = 256;

    protected override string Filename => "BridgeBuilder";
}