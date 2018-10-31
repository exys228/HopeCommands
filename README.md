# HopeCommands
Making osu!hope plugins fully commandable.

Note that you will need to fix project's HOPEless and osu.Shared references.

Example usage:
```csharp
using exys228.HopeCommands;

public class SomePlugin : CommandBase, IHopePlugin
{
	// ...
	
	public void Load()
	{
		AddCommand("test", "print some text", delegate (string[] args)
		{
			SendPlayerMessage("SomePlugin", "yo", "#somechannel", 0);
		});
	}
	
	private void SendPlayerMessage(string sender, string message, string channel, int senderid)
	{
		// ...
	}
}
```