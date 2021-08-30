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

            /*
             * Ketika Jalan
                % xt % uotls % -1 % username % sp:8,tx: 486,ty: 427,strFrame: Enter %

                0 =
                1 = xt
                2 = uotls
                3 = -1
                4 = username
                5 = sp:8,tx: 486,ty: 427,strFrame: Enter

            * Ketika awal masuk room
                % xt % uotls % -1 % username % strPad:Spawn,tx:0,strFrame:Enter,ty:0 %
                
                0 =
                1 = xt
                2 = uotls
                3 = -1
                4 = username
                5 = strPad:Spawn,tx:0,strFrame:Enter,ty:0
             */

            // strPad:Spawn (0), tx:0 (1), strFrame:Enter (2), ty:0 (3)
            string[] args5_decomp = message.Arguments[5].Split(',');

            // strPad:Spawn (0)
            string[] args5_strPad = args5_decomp[0].Split(':');
            string strPad = args5_strPad[0];

            // strFrame:Enter (2)
            string[] args5_strFrame = args5_decomp[2].Split(':');

            // current Username
            string currUsername = message.Arguments[4];

            string curr_pad = "Spawn";
            string curr_cell = "Enter";
            bool isStrPad = false;
            if (strPad == "strPad")
            {
                isStrPad = true;
                curr_pad = args5_strPad[1];
                curr_cell = args5_strFrame[1];
            }


            if (isStrPad && (currUsername == targetUsername) && !World.IsMapLoading)
            {
                Player.MoveToCell(curr_cell, curr_pad);
            }
        }
    }
}
