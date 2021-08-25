using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grimoire.Game;
using Grimoire.Networking;
using DarkUI.Forms;

namespace ExamplePacketPlugin
{
    public partial class Main : DarkForm
    {
        public static Main Instance { get; } = new Main();

        public string targetUsername => Main.Instance.cmbGotoUsername.Text.ToLower();

        LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();

        public CellJumperHandler CJHandler { get; } = new CellJumperHandler();

        private int healthPercent => (int)Main.Instance.numHealthPercent.Value;

        string[] buffSkill = null;
        int buffIndex = 0;

        string[] healSkill = null;
        int healIndex = 0;

        string[] monsterList = null;

        Stopwatch stopwatch = new Stopwatch();

        public Main()
        {
            InitializeComponent();

            KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.hotkey);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private async void cbEnablePlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnablePlugin.Checked)
            {
                cmbGotoUsername.Enabled = false;
                tbSkillList.Enabled = false;
                gbOptions.Enabled = false;

                int gotoTry = 0;

                string[] skillList = tbSkillList.Text.Split(',');
                int skillIndex = 0;

                if (!cbUnfollow.Checked)
                    Proxy.Instance.RegisterHandler(CJHandler);

                while(cbEnablePlugin.Checked)
                {
                    try
                    {
                        // while player is logout -> do delay (2s), wait first join, do first join delay
                        if (!Player.IsLoggedIn)
                            await waitForFirstJoin();

                        // plugin disabled
                        if (!cbEnablePlugin.Checked)
                            return;

                        // starting the plugin
                        if (IsPlayerInMap(targetUsername) || cbUnfollow.Checked)
                        {
                            gotoTry = 0;

                            if (!Player.IsAlive)
                            {
                                World.SetSpawnPoint();
                                await Task.Delay(500);
                                continue;
                            }

                            if (cbUseHeal.Checked && tbHealSkill.Text != String.Empty && isHealthUnder(healthPercent))
                            {
                                Player.UseSkill(healSkill[healIndex]);
                                healIndex++;

                                if (healIndex >= healSkill.Length)
                                    healIndex = 0;

                                await Task.Delay(150);
                                continue;
                            }

                            if (cbStopAttack.Checked)
                            {
                                if (Player.HasTarget)
                                    Player.CancelTarget();

                                if (cbBuffIfStop.Checked && tbBuffSkill.Text != String.Empty && isPlayerInCombat())
                                {
                                    Player.UseSkill(buffSkill[buffIndex]);
                                    buffIndex++;

                                    if (buffIndex >= buffSkill.Length)
                                        buffIndex = 0;
                                }

                                await Task.Delay(150);
                                continue;
                            }

                            if (cbAttackPriority.Checked)
                                doPriorityAttack();

                            if (!Player.HasTarget)
                                Player.AttackMonster("*");

                            Player.UseSkill(skillList[skillIndex]);
                            skillIndex++;

                            if (skillIndex >= skillList.Length)
                                skillIndex = 0;
                        }
                        else
                        {
                            gotoTarget(targetUsername);
                            if (cbStopIf.Checked)
                            {
                                gotoTry++;
                                if (gotoTry >= 5)
                                {
                                    gotoTry = 0;
                                    stopMaid();
                                }
                            }
                            await Task.Delay(2000);
                        }

                        await Task.Delay(150);
                    }
                    catch { }
                }
            }
            else
            {
                stopMaid();
            }
        }

        private async Task waitForFirstJoin()
        {
            // wait player to join the map
            while (World.IsMapLoading)
                await Task.Delay(2000);

            // do first join delay
            await Task.Delay((int)numRelogDelay.Value);
        }

        private void doPriorityAttack()
        {
            for(int i = 0; i < monsterList.Length; i++)
            {
                if(World.IsMonsterAvailable(monsterList[i]))
                {
                    Player.AttackMonster(monsterList[i]);
                    return;
                }
            }
        }

        private bool isPlayerInCombat()
        {
            return (Player.CurrentState == Player.State.InCombat ? true : false);
        }

        private bool IsPlayerInMap (string targetUsername)
        {
            foreach (string player in World.PlayersInMap)
            {
                if (player.ToLower() == targetUsername)
                    return true;
            }
            return false;
        }

        private bool isHealthUnder(int percentage)
        {
            int healthBoundary = Player.HealthMax * percentage / 100;
            return Player.Health <= healthBoundary ? true : false;

            // if any party healt bellow setup (flash.call still buggy)
            /*try
            {
                World.RefreshDictionary();
                foreach (PlayerInfo player in World.Players)
                {
                    int healthBoundary = player.MaxHP * percentage / 100;
                    if (player.HP <= healthBoundary)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }*/
        }

        private void gotoTarget(string targetUsername)
        {
            Player.MoveToCell("Blank", "Spawn");
            Proxy.Instance.SendToServer($"%xt%zm%cmd%1%goto%{targetUsername}%");
        }

        public void stopMaid()
        {
            Proxy.Instance.UnregisterHandler(CJHandler);
            cmbGotoUsername.Enabled = true;
            tbSkillList.Enabled = true;
            gbOptions.Enabled = true;
            cbEnablePlugin.Checked = false;
        }

        /* Hotkey */

        private void cbEnableGlobalHotkey_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableGlobalHotkey.Checked)
            {
                kbh.OnKeyPressed += globalHotkey;
                kbh.OnKeyUnpressed += (s, ek) => { };
                this.KeyDown -= hotkey;

                kbh.HookKeyboard();
            }
            else
            {
                kbh.OnKeyPressed -= globalHotkey;
                kbh.OnKeyUnpressed -= (s, ek) => { };
                this.KeyDown += new KeyEventHandler(this.hotkey);

                kbh.UnHookKeyboard();
            }
        }

        private void hotkey(object sender, KeyEventArgs e)
        {
            if (cmbGotoUsername.Focused || tbAttPriority.Focused)
                return;

            if (e.KeyCode == Keys.R)
            {
                // LockCell: R
                e.SuppressKeyPress = true;
                cbUnfollow.Checked = cbUnfollow.Checked ? false : true;
            }
            else if (e.KeyCode == Keys.T)
            {
                // StopAttack: T
                e.SuppressKeyPress = true;
                cbStopAttack.Checked = cbStopAttack.Checked ? false : true;
            }
        }
        private void globalHotkey(object sender, Keys e)
        {
            if (cmbGotoUsername.Focused || tbAttPriority.Focused)
                return;

            if (e == Keys.R)
            {
                // LockCell: R
                cbUnfollow.Checked = cbUnfollow.Checked ? false : true;
            }
            else if (e == Keys.T)
            {
                // StopAttack: T
                cbStopAttack.Checked = cbStopAttack.Checked ? false : true;
            }
        }

        /* Other Control */

        private void cbLockCell_CheckedChanged(object sender, EventArgs e)
        {
            if(cbUnfollow.Checked)
                Proxy.Instance.UnregisterHandler(CJHandler);
            else
                Proxy.Instance.RegisterHandler(CJHandler);
        }

        private void cbStopAttack_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStopAttack.Checked)
            {
                lbStopAttackBg.BackColor = System.Drawing.Color.DeepPink;
                stopwatch.Reset();
                stopwatch.Start();
                timerStopAttack.Enabled = true;
                cbStopAttack.BackColor = System.Drawing.Color.Magenta;
                if (Player.HasTarget)
                    Player.CancelTarget();
                Player.Rest();
            }
            else
            {
                lbStopAttackBg.BackColor = System.Drawing.Color.Transparent;
                stopwatch.Stop();
                this.Text = "Maid Remake";
                timerStopAttack.Enabled = false;
                cbStopAttack.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void cbUseHeal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseHeal.Checked)
            {
                tbHealSkill.Enabled = false;
                numHealthPercent.Enabled = false;
                healSkill = tbHealSkill.Text.Split(',');
            }
            else
            {
                tbHealSkill.Enabled = true;
                numHealthPercent.Enabled = true;
            }
        }

        private void cbBuffIfStop_CheckedChanged(object sender, EventArgs e)
        {
            if(cbBuffIfStop.Checked)
            {
                tbBuffSkill.Enabled = false;
                buffSkill = Main.Instance.tbBuffSkill.Text.Split(',');
                buffIndex = 0;
            }
            else
            {
                tbBuffSkill.Enabled = true;
            }
        }

        private void cbAttackPriority_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAttackPriority.Checked)
            {
                monsterList = Main.Instance.tbAttPriority.Text.Split(',');
                tbAttPriority.Enabled = false;
            }
            else
            {
                tbAttPriority.Enabled = true;
            }
        }

        private void timerStopAttack_Tick(object sender, EventArgs e)
        {
            this.Text = $"Maid Remake ({string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed)})";
        }

        private void cmbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassPreset.cbClear();
            switch (cmbPreset.SelectedItem.ToString())
            {
                case "LR":
                    ClassPreset.LR();
                    break;
                case "LC":
                    ClassPreset.LC();
                    break;
                case "LOO":
                    ClassPreset.LOO();
                    break;
                case "SC":
                    ClassPreset.SC();
                    break;
                case "AP":
                    ClassPreset.AP();
                    break;
                case "CCM":
                    ClassPreset.CCM();
                    break;
            }
            ClassPreset.cbSet();
        }

        // get username in cell
        private void cmbGotoUsername_Clicked(object sender, EventArgs e)
        {
            if (World.IsMapLoading)
                return;
            cmbGotoUsername.Items.Clear();
            foreach(string player in World.PlayersInMap)
                cmbGotoUsername.Items.Add(player);
        }
    }
}
