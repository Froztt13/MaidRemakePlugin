using Grimoire.Game;
using Grimoire.Networking;

namespace MaidRemake.Handlers
{
	public class PartyChatHandler : IXtMessageHandler
	{
		public string[] HandledCommands { get; } = { "chatm" };

		public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();

		public void Handle(XtMessage message)
		{
			/*  % xt % chatm % 32123 % party~asdasdasdsa % player % 2049 % 32123 % 0 %
				0 =
				1 = xt
				2 = chatm
				3 = 32123
				4 = party~asdasdasdsa
				5 = player
			*/

			try
			{
				string[] chat = message.Arguments[4].ToString().Split('~');
				string type = chat[0];
				string msg = chat[1];

				if (type.Equals("party") && msg.StartsWith("."))
				{
					string[] command = msg.Split(' ');
					switch (command[0])
					{
						case ".tp":
							string map = command[1];
							if (map.Contains("tercessuinotlim"))
								Player.MoveToCell("m22", "Left");
							Player.JoinMap(map, "Enter", "Spawn");
							break;
						case ".target":
							MaidRemake.Instance.cmbGotoUsername.Text = msg.Remove(0, 8);
							break;
						case ".stop":
							MaidRemake.Instance.cbEnablePlugin.Checked = false;
							break;
						case ".start":
							MaidRemake.Instance.cbEnablePlugin.Checked = true;
							break;
					}
				}
			}
			catch { }
		}
	}
}
