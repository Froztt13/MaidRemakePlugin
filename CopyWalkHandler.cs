using Grimoire.Game;
using Grimoire.Networking;

namespace MaidRemake
{
    public class CopyWalkHandler : IXtMessageHandler
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
            */

            try
            {
                // current Username
                string currUsername = message.Arguments[4].ToLower();

                if (message.Arguments[5].StartsWith("sp:") && (currUsername == targetUsername) && !World.IsMapLoading)
                {
                    string movement = message.Arguments[5].ToString();
                    string cell = null;
                    string pad = null;
                    float tx = 0f;
                    float ty = 0f;
                    foreach (string m in movement.Split(','))
                    {
                        if (m.Split(':')[0] == "strFrame")
                            cell = m.Split(':')[1];
                        if (m.Split(':')[0] == "strPad")
                            pad = m.Split(':')[1];
                        if (m.Split(':')[0] == "tx")
                            tx = float.Parse(m.Split(':')[1]);
                        if (m.Split(':')[0] == "ty")
                            ty = float.Parse(m.Split(':')[1]);
                    }
                    if (tx != 0f && ty != 0f)
                    {
                        Player.WalkToPoint(tx.ToString(), ty.ToString());
                    }
                }
            }
            catch { }
        }
    }
}
