using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HOPEless.Bancho;
using HOPEless.Bancho.Objects;

namespace exys228.HopeCommands
{
	public class CommandBase
	{
		private string PluginName = "";
		private string ChannelName = "";
		private char CommandChar = '\0';

		private int? UserID = null;

		private List<Command> CommandList = new List<Command>();

		private Queue<BanchoPacket> ServerReplies = new Queue<BanchoPacket>();

		/// <summary>
		/// Base for IHopePlugin with commands.
		/// </summary>
		/// <param name="pluginName">Plugin's name</param>
		/// <param name="channel">Channel name (including "#" at the start)</param>
		/// <param name="commandChar">Start char for commands (ex. "!" or "/")</param>
		public CommandBase(string pluginName, string channel, char commandChar = '!')
		{
			PluginName = pluginName;
			ChannelName = channel;
			CommandChar = commandChar;

			CommandList.Add(new Command("help", "display help", delegate(string[] args)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("List of available commands:");

				foreach (Command c in CommandList)
					sb.AppendLine($"{CommandChar}{c.CommandText} - {c.Description}");

				SendPlayerMessage(PluginName, sb.ToString(), ChannelName, UserID.Value);
			}));
		}

		/// <summary>
		/// Add command to plugin's command list.
		/// </summary>
		/// <param name="command">Command's name</param>
		/// <param name="description">Description of command for help menu.</param>
		/// <param name="callback">Command's function</param>
		public void AddCommand(string command, string description, Command.DCallback callback)
		{
			AddCommand(new Command(command, description, callback));
		}

		/// <summary>
		/// Add command to plugin's command list.
		/// </summary>
		public void AddCommand(Command command)
		{
			if (CommandList.Where(a => a.CommandText == command.CommandText).Count() > 0)
				throw new ArgumentException("Command already exists!");

			CommandList.Add(command);
		}

		public virtual void OnBanchoRequest(ref List<BanchoPacket> plist)
		{
			for (int i = 0; i < plist.Count; i++)
			{
				switch (plist[i].Type)
				{
					case PacketType.ClientChatChannelJoin: // if player tries to join, then let him do so.
					{
						if (new BanchoString(plist[i].Data).Value == ChannelName)
						{
							ServerReplies.Enqueue
							(
								new BanchoPacket
								(
									PacketType.ServerChatChannelJoinSuccess,
									new BanchoString(ChannelName)
								)
							);

							plist.RemoveAt(i); // don't send to bancho
						}

						break;
					}

					case PacketType.ClientChatChannelLeave: // when player leaves nospec channel don't send data about channel's existence to bancho.
					{
						if (new BanchoString(plist[i].Data).Value == ChannelName)
							plist.RemoveAt(i);

						break;
					}

					case PacketType.ClientChatMessagePublic:
					{
						BanchoChatMessage msg = new BanchoChatMessage(plist[i].Data);

						if (msg.Channel != ChannelName) break;

						string tmsg = msg.Message.Trim();

						if (tmsg.First() == CommandChar)
						{
							string[] splitMessage = tmsg.Split(' ');

							string cmd = splitMessage.First().Substring(1); // remove CommandChar

							var command = CommandList.Where(a => a.CommandText == cmd);

							if (command.Count() <= 0)
								SendPlayerMessage(PluginName, $"Unknown command. Enter {CommandChar}help for list of available commands.", ChannelName, UserID.Value);
							else
								command.First().Execute(splitMessage.Skip(1).ToArray()); // Skip(1): remove cmd, leave args
						}

						plist.RemoveAt(i); // don't send to bancho
						break;
					}
				}
			}
		}

		public virtual void OnBanchoResponse(ref List<BanchoPacket> plist)
		{
			for (int i = 0; i < plist.Count; i++)
			{
				switch (plist[i].Type)
				{
					case PacketType.ServerLoginReply:
					{
						BanchoInt userid = new BanchoInt(plist[i].Data);
						
						if (userid.Value >= 0)
						{
							UserID = userid.Value;

							ServerReplies.Enqueue
							(
								new BanchoPacket
								(
									PacketType.ServerChatChannelAvailableAutojoin,
									new BanchoChatChannel
									(
										ChannelName,
										$"{PluginName}'s command menu",
										-1 // lul
									)
								)
							);
						}
						else UserID = null;
						
						break;
					}
				}
			}

			if (ServerReplies.Count > 0)
				plist.Add(ServerReplies.Dequeue());
		}

		internal void SendPlayerMessage(string sender, string message, string channel, int senderid)
		{
			ServerReplies.Enqueue
			(
				new BanchoPacket(PacketType.ServerChatMessage,
				new BanchoChatMessage(sender, message, channel, senderid))
			);
		}
	}
}
