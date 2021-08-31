using Grimoire.Game;
using Grimoire.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidRemake
{
    public class CellJumperHandler : IXtMessageHandler
    {
        public string[] HandledCommands { get; } = { "uotls" };

        public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();

        public void Handle(XtMessage message)
        {
            /* When walk
                % xt % uotls % -1 % username % sp:8,tx: 486,ty: 427,strFrame: Enter %

                0 =
                1 = xt
                2 = uotls
                3 = -1
                4 = username
                5 = sp:8,tx: 486,ty: 427,strFrame: Enter
            * when jump into cell
                % xt % uotls % -1 % username % strPad:Spawn,tx:0,strFrame:Enter,ty:0 %
                
                0 =
                1 = xt
                2 = uotls
                3 = -1
                4 = username
                5 = strPad:Spawn,tx:0,strFrame:Enter,ty:0
            */

            try
            {
                // current Username
                string currUsername = message.Arguments[4].ToLower();

                if (message.Arguments[5].StartsWith("strPad:") && (currUsername == targetUsername) && !World.IsMapLoading)
                {
                    // strPad:Spawn (0), tx:0 (1), strFrame:Enter (2), ty:0 (3)
                    string targetPad = message.Arguments[5].Split(',')[0].Split(':')[1];

                    // strPad:Spawn (0), tx:0 (1), strFrame:Enter (2), ty:0 (3)
                    string targetFrame = message.Arguments[5].Split(',')[2].Split(':')[1];

                    Player.MoveToCell(targetFrame, targetPad);
                }
            }
            catch { }
        }
    }
}
