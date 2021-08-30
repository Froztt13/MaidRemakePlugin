using Grimoire.Game;
using Grimoire.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidRemake.LockedMapHandle
{
    public class LockedMapHandler : IXtMessageHandler
    {
        public string[] HandledCommands { get; } = { "warning" };

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

            if(message.Arguments[4].Contains("Cannot goto to player in a Locked zone.") && Player.IsLoggedIn)
            {
                string[] mapInfo = AlternativeMap.GetNext().Split(';');
                Player.JoinMap(mapInfo[0], mapInfo[1], mapInfo[2]);
            }
        }
    }
}
