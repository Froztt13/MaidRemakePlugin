using Grimoire.Game;
using Grimoire.Networking;
using MaidRemake.WhitelistMap;
using System.Collections.Generic;
using System.Linq;

namespace MaidRemake.Handlers
{
	public class CellJumperHandler : IXtMessageHandler
	{
		public string[] HandledCommands { get; } = { "uotls" };

		public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();

		private List<string> maps => WhitelistMapForm.Instance.getAlternativeMap;

		public void Handle(XtMessage message)
		{
			/*
				when jump into cell
				% xt % uotls % -1 % username % mvts:-1,px:0,strPad:Spawn,py:0,mvtd:0,tx:0,strFrame:Enter,ty:0 %
				
				0 =
				1 = xt
				2 = uotls
				3 = -1
				4 = username
				5 = mvts:-1,px:0,strPad:Spawn,py:0,mvtd:0,tx:0,strFrame:Enter,ty:0
			*/

			try
			{
				// current Username
				string currUsername = message.Arguments[4].ToLower();

				if (currUsername == targetUsername && !World.IsMapLoading)
				{
					string movement = message.Arguments[5].ToString();
					string cell = null;
					string pad = null;
					foreach (string m in movement.Split(','))
					{
						if (m.Split(':')[0] == "strFrame")
							cell = m.Split(':')[1];
						if (m.Split(':')[0] == "strPad")
							pad = m.Split(':')[1];
					}
					if (cell != null && pad != null)
					{
						Player.MoveToCell(cell, pad);
						MaidRemake.Instance.resetSpecials();
					}

					// on whitelisted map
					if (MaidRemake.Instance.cbWhitelistMap.Checked && cell != null)
					{
						string map = Player.Map.ToLower();
						string configMap = maps.Find(i => i.Split(';')[0].ToLower() == map);
						if (configMap != null)
						{
							string configCell = configMap.Split(';')[1].ToLower();
							//MaidRemake.Instance.logDebug($"config=>{configCell} || actual=>{cell.ToLower()}");
							if (configCell == cell.ToLower() || configCell == "all")
							{
								MaidRemake.Instance.pauseFollow();
							} 
							else
							{
								MaidRemake.Instance.resumeFollow();
							}
						}
						else
						{
							MaidRemake.Instance.resumeFollow();
						}
					}
				}
			}
			catch { }
		}
	}
}
