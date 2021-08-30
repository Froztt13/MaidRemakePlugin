using System;
using System.Windows.Forms;
using Grimoire.Networking;
using Grimoire.Tools.Plugins;
using MaidRemake.LockedMapHandle;

namespace MaidRemake
{
    [GrimoirePluginEntry]
    public class Loader : IGrimoirePlugin
    {
        public string Author => "Afif_Sapi, Froztt13";

        public string Description => "Battle maid to help your battle!\r\n" +
            "This plugin will auto follow 'Goto Username' then attack and kill any monster.";

        private ToolStripItem menuItem;

        public void Load()
        {
            // Add an item to the main menu in Grimoire.
            menuItem = Grimoire.UI.Root.Instance.MenuMain.Items.Add("Maid(R)");
            menuItem.Click += MenuStripItem_Click;
        }

        public void Unload() // In this method you need to clean everything up
        {
            Proxy.Instance.UnregisterHandler(MaidRemake.Instance.MapLockHandler);
            Proxy.Instance.UnregisterHandler(MaidRemake.Instance.CJHandler);
            menuItem.Click -= MenuStripItem_Click;
            Grimoire.UI.Root.Instance.MenuMain.Items.Remove(menuItem);
            LockedMapForm.Instance.Dispose();
            MaidRemake.Instance.Dispose();
        }

        private void MenuStripItem_Click(object sender, EventArgs e)
        {
            if (MaidRemake.Instance.Visible)
            {
                if (MaidRemake.Instance.WindowState == FormWindowState.Minimized)
                    MaidRemake.Instance.WindowState = FormWindowState.Normal;
                MaidRemake.Instance.Hide();
            }
            else
            {
                MaidRemake.Instance.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                MaidRemake.Instance.Location = new System.Drawing.Point((Grimoire.UI.Root.Instance.Location.X + Grimoire.UI.Root.Instance.Width / 2) - 
                    (MaidRemake.Instance.Width / 2), (Grimoire.UI.Root.Instance.Location.Y + Grimoire.UI.Root.Instance.Height / 2) - (MaidRemake.Instance.Height / 2));
                MaidRemake.Instance.Show(Grimoire.UI.Root.Instance);
                MaidRemake.Instance.BringToFront();
            }
        }
    }
}
