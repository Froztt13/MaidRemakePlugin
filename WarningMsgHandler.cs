using Grimoire.Game;
using Grimoire.Networking;
using MaidRemake.LockedMapHandle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidRemake
{
    public class WarningMsgHandler : IXtMessageHandler
    {
        public string[] HandledCommands { get; } = { "warning" };
        public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();
        public bool isLockedMapHandlerEnabled => MaidRemake.Instance.cbHandleLockedMap.Checked;

        public void Handle(XtMessage message)
        {

            /*
               %xt%warning%-1%Cannot goto to player in a Locked zone.%

                0 =
                1 = xt
                2 = warning
                3 = -1
                4 = Cannot goto to player in a Locked zone.
             */

            if(isLockedMapHandlerEnabled && message.Arguments[4].Contains("Cannot goto to player in a Locked zone.") && Player.IsLoggedIn)
            {
                string[] mapInfo = AlternativeMap.GetNext().Split(';');
                Player.JoinMap(mapInfo[0], mapInfo[1], mapInfo[2]);
            }
            else if (Player.IsLoggedIn && Player.Map != "whitemap")
            {
                if (message.Arguments[4].Contains("Cannot goto to player in a Locked zone.") && Player.IsLoggedIn)
                {
                    Player.JoinMap($"whitemap-{new Random().Next(9999, 999999)}");
                }
                else if (message.Arguments[4].Contains("Room join failed, destination room is full.") && Player.IsLoggedIn)
                {
                    Player.JoinMap($"whitemap-{new Random().Next(9999, 999999)}");
                }
                else if (message.Arguments[4].Contains($"Player '{targetUsername}' could not be found.") && Player.IsLoggedIn)
                {
                    Player.JoinMap($"whitemap-{new Random().Next(9999, 999999)}");
                }
            }
        }
    }
}
