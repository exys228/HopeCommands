# HopeCommands
Making osu!hope plugins fully commandable.

Note that you will need to fix project's [HOPEless](https://github.com/HoLLy-HaCKeR/HOPEless) and [osu.Shared](https://github.com/HoLLy-HaCKeR/osu.Shared) references.

Example usage:
```csharp
using exys228.HopeCommands;

public class SomePlugin : CommandBase, IHopePlugin
{
	// ...
	
	private const string PluginName = "SomePlugin";
	private const string ChannelName = "#somechannel";
	
	public SomePlugin() : base(PluginName, ChannelName)
	{
		
	}
	
	public void Load()
	{
		AddCommand("test", "print some text", delegate (string[] args)
		{
			SendPlayerMessage(PluginName, "yo", ChannelName, 0);
		});
	}
	
	private void SendPlayerMessage(string sender, string message, string channel, int senderid)
	{
		// ...
	}
}
```
