using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grimoire.Game;
using Grimoire.Networking;

namespace ExamplePacketPlugin
{
    public partial class Main : Form
    {
        public static Main Instance { get; } = new Main();

        public Main()
        {
            InitializeComponent();

            KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.hotkey);
        }

        LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();

        public CellJumperHandler CJHandler { get; } = new CellJumperHandler();
        public loginHandler loggedInHandler { get; } = new loginHandler();

        string[] buffSkill = null;
        int buffIndex = 0;

        string[] monsterList = null;

        Stopwatch stopwatch = new Stopwatch();

        private async void cbEnablePlugin_CheckedChanged(object sender, EventArgs e)
        {
            bool isStopIfEnabled = cbStopIf.Checked;

            if (cbEnablePlugin.Checked)
            {
                tbGotoUsername.Enabled = false;
                tbSkillList.Enabled = false;
                cbStopIf.Enabled = false;
                numRelogDelay.Enabled = false;

                int gotoTry = 0;
                int relogDelay = (int)numRelogDelay.Value;

                CJHandler.targetUsername = tbGotoUsername.Text.ToLower();
                string targetUsername = tbGotoUsername.Text.ToLower();

                string[] skillList = tbSkillList.Text.Split(',');
                int skillIndex = 0;

                Proxy.Instance.RegisterHandler(loggedInHandler);
                if(!cbLockCell.Checked) Proxy.Instance.RegisterHandler(CJHandler);

                while(cbEnablePlugin.Checked)
                {
                    try
                    {
                        // while player is logout & no firstLogin state -> do delay (2s)
                        while(!Player.IsLoggedIn && !await loggedInHandler.getStateAsync()) 
                            await Task.Delay(2000);

                        // if first join state occur -> set firstjoin to false and do first join delay (5s)
                        if (await loggedInHandler.getSetStateAsync()) 
                            await Task.Delay(relogDelay);

                        // starting the plugin
                        if(IsPlayerInMap(targetUsername) || cbLockCell.Checked)
                        {
                            gotoTry = 0;

                            if(!Player.IsAlive)
                            {
                                World.SetSpawnPoint();

                                await Task.Delay(500);
                                continue;
                            }

                            if(cbStopAttack.Checked)
                            {
                                if(Player.HasTarget)
                                    Player.CancelTarget();

                                if(cbBuffIfStop.Checked && isPlayerInCombat())
                                {
                                    Player.UseSkill(buffSkill[buffIndex]);
                                    buffIndex++;

                                    if (buffIndex == buffSkill.Length)
                                        buffIndex = 0;
                                }

                                await Task.Delay(500);
                                continue;
                            }

                            if(cbAttackPriority.Checked)
                                doPriorityAttack();

                            if (!Player.HasTarget)
                                Player.AttackMonster("*");

                            Player.UseSkill(skillList[skillIndex]);
                            skillIndex++;

                            if (skillIndex == skillList.Length) 
                                skillIndex = 0;
                        } 
                        else
                        {
                            gotoTarget(targetUsername);
                            if(isStopIfEnabled)
                            {
                                gotoTry++;
                                if(gotoTry >= 5)
                                {
                                    gotoTry = 0;
                                    stopBot();
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
                stopBot();
            }
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

        private void gotoTarget(string targetUsername)
        {
            Player.MoveToCell("Blank", "Spawn");
            Proxy.Instance.SendToServer($"%xt%zm%cmd%1%goto%{targetUsername}%");
        }

        public void stopBot()
        {
            Proxy.Instance.UnregisterHandler(CJHandler);
            Proxy.Instance.UnregisterHandler(loggedInHandler);
            tbGotoUsername.Enabled = true;
            tbSkillList.Enabled = true;
            cbStopIf.Enabled = true;
            numRelogDelay.Enabled = true;
            cbEnablePlugin.Checked = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
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
            if (tbGotoUsername.Focused || tbAttPriority.Focused)
                return;

            if (e.KeyCode == Keys.R)
            {
                // LockCell: R
                e.SuppressKeyPress = true;
                cbLockCell.Checked = cbLockCell.Checked ? false : true;
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
            if (tbGotoUsername.Focused || tbAttPriority.Focused)
                return;

            if (e == Keys.R)
            {
                // LockCell: R
                cbLockCell.Checked = cbLockCell.Checked ? false : true;
            }
            else if (e == Keys.T)
            {
                // StopAttack: T
                cbStopAttack.Checked = cbStopAttack.Checked ? false : true;
            }
        }

        private void cbLockCell_CheckedChanged(object sender, EventArgs e)
        {
            if(cbLockCell.Checked)
                Proxy.Instance.UnregisterHandler(CJHandler);
            else
                Proxy.Instance.RegisterHandler(CJHandler);
        }

        private void cbStopAttack_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStopAttack.Checked)
            {
                stopwatch.Reset();
                stopwatch.Start();
                timerStopAttack.Enabled = true;
                if(cbChangeColor.Checked)
                    cbStopAttack.BackColor = System.Drawing.Color.Magenta;
                if (Player.HasTarget)
                    Player.CancelTarget();
                Player.Rest();
            }
            else
            {
                stopwatch.Stop();
                this.Text = "Maid Remake";
                timerStopAttack.Enabled = false;
                cbStopAttack.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void cbBuffIfStop_CheckedChanged(object sender, EventArgs e)
        {
            if(cbBuffIfStop.Checked)
            {
                buffSkill = Main.Instance.tbBuffSkill.Text.Split(',');
                buffIndex = 0;
                tbBuffSkill.Enabled = false;
            }
            else
                tbBuffSkill.Enabled = true;
        }

        private void cbAttackPriority_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAttackPriority.Checked)
            {
                monsterList = Main.Instance.tbAttPriority.Text.Split(',');
                tbAttPriority.Enabled = false;
            }
            else
                tbAttPriority.Enabled = true;
        }

        private void timerStopAttack_Tick(object sender, EventArgs e)
        {
            this.Text = $"Maid Remake ({string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed)})";
        }
    }
}
