using Grimoire.Game;
using Grimoire.Networking;

namespace MaidRemake
{
    public class CellJumperHandler : IXtMessageHandler
    {
        public string[] HandledCommands { get; } = { "uotls" };

        public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();

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
                        System.Console.WriteLine($"Jump = cell:{cell} pad:{pad}");
                        Player.MoveToCell(cell, pad);
                    }
                }
            }
            catch { }
        }
    }
}
