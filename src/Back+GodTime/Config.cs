using System.IO;
using Newtonsoft.Json;

namespace GodTime;

public class Config
{
	[JsonProperty("无敌时间")]
	public int time;

	public Config Write(string file)
	{
		File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
		return this;
	}

	public static Config Read(string file)
	{
		if (!File.Exists(file))
		{
			WriteExample(file);
		}
		return JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
	}

	public static void WriteExample(string file)
	{
		_Config config = new _Config
		{
			time = 10
		};
		Config config2 = new Config
		{
			time = config.time
		};
		config2.Write(file);
	}
}
