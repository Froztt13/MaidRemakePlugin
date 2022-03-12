using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grimoire.Game;
using Grimoire.Networking;
using DarkUI.Forms;
using Grimoire.Tools;
using MaidRemake.LockedMapHandle;

namespace MaidRemake
{
	public partial class MaidRemake : DarkForm
	{
		public static MaidRemake Instance { get; } = new MaidRemake();

		public string targetUsername => MaidRemake.Instance.cmbGotoUsername.Text.ToLower();

		public bool isPlayerInMyCell => bool.Parse(Flash.Call<string>("GetCellPlayers", new string[] { targetUsername }) ?? "False");

		public bool isPlayerInMyRoom => IsPlayerInMap(targetUsername);

		public int skillDelay => (int)MaidRemake.Instance.numSkillDelay.Value;

		LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();

		public CellJumperHandler CJHandler { get; } = new CellJumperHandler();

		public WarningMsgHandler RedMsgHandler { get; } = new WarningMsgHandler();

		public CopyWalkHandler CopyWalkHandler { get; } = new CopyWalkHandler();

		private int healthPercent => (int)MaidRemake.Instance.numHealthPercent.Value;

		string[] buffSkill = null;
		int buffIndex = 0;

		string[] healSkill = null;
		int healIndex = 0;

		string[] monsterList = null;

		Stopwatch stopwatch = new Stopwatch();

		public MaidRemake()
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
				startUI();

				int gotoTry = 0;

				string[] skillList = tbSkillList.Text.Split(',');
				int skillIndex = 0;

				if (cbHandleLockedMap.Checked && AlternativeMap.Count() > 0)
					AlternativeMap.Init();
				else if (cbHandleLockedMap.Checked)
					cbHandleLockedMap.Checked = false;

				Proxy.Instance.RegisterHandler(RedMsgHandler);

				if (!cbUnfollow.Checked)
					Proxy.Instance.RegisterHandler(CJHandler);

				if (cbCopyWalk.Checked)
					Proxy.Instance.RegisterHandler(CopyWalkHandler);

				if (!cbUnfollow.Checked && Player.IsLoggedIn && !World.IsMapLoading && isPlayerInMyRoom && !isPlayerInMyCell)
					Player.GoToPlayer(targetUsername);

				while (cbEnablePlugin.Checked)
				{
					try
					{
						// while player is logout -> do delay (2s), wait first join, do first join delay
						if (cbEnablePlugin.Checked && !Player.IsLoggedIn)
							await waitForFirstJoin();

						// plugin disabled
						if (!cbEnablePlugin.Checked)
							return;

						// starting the plugin
						if ((isPlayerInMyRoom || cbUnfollow.Checked) && Player.IsLoggedIn && !World.IsMapLoading)
						{
							gotoTry = 0;

							if (!Player.IsAlive)
							{
								skillIndex = 0;
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

								await Task.Delay(skillDelay);
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

								await Task.Delay(skillDelay);
								continue;
							}

							if (cbAttackPriority.Checked)
								doPriorityAttack();

							if (World.IsMonsterAvailable("*") && !Player.HasTarget)
								Player.AttackMonster("*");

							if (cbWaitSkill.Checked && (Player.SkillAvailable(skillList[skillIndex]) > 0 || !Player.HasTarget))
							{
								await Task.Delay(150);
								continue;
							}

							if (Player.SkillAvailable(skillList[skillIndex]) == 0 && Player.HasTarget)
								Player.UseSkill(skillList[skillIndex]);

							skillIndex++;

							if (skillIndex >= skillList.Length)
								skillIndex = 0;
						}
						else if (Player.IsLoggedIn && !World.IsMapLoading)
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

							// wait loading screen before try to goto again (max: 5100 ms)
							for (int i = 0; i < 36 && cbEnablePlugin.Checked && Player.IsLoggedIn && !World.IsMapLoading; i++)
								await Task.Delay(150);

							// wait map loading end
							while (cbEnablePlugin.Checked && Player.IsLoggedIn && World.IsMapLoading)
								await Task.Delay(500);

							// wait 2 second before try to goto or join to different map (when locked map handler is enabled)
							for (int i = 0; i < 8 && cbEnablePlugin.Checked && cbHandleLockedMap.Checked && Player.IsLoggedIn && !World.IsMapLoading; i++)
								await Task.Delay(250);

							// goto target current cell when in the same room
							while (cbEnablePlugin.Checked && Player.IsLoggedIn && isPlayerInMyRoom && !isPlayerInMyCell)
							{
								Player.GoToPlayer(targetUsername);
								if (cbEnablePlugin.Checked && Player.IsLoggedIn && isPlayerInMyRoom && !isPlayerInMyCell)
									await Task.Delay(1000);
								else break;
							}
						}

						await Task.Delay(skillDelay);
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
			while (cbEnablePlugin.Checked && World.IsMapLoading)
				await Task.Delay(2000);

			// do first join delay
			if (cbEnablePlugin.Checked)
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
		}

		private async void gotoTarget(string targetUsername)
		{
			if (Player.CurrentState != Player.State.Idle)
				Player.MoveToCell("Blank", "Spawn");
			await Proxy.Instance.SendToServer($"%xt%zm%cmd%1%goto%{targetUsername}%");
		}

		/* UI state */

		public void startUI()
		{
			cmbGotoUsername.Enabled = false;
			tbSkillList.Enabled = false;
			gbOptions.Enabled = false;
			if (LockedMapForm.Instance.Visible)
			{
				if (LockedMapForm.Instance.WindowState == FormWindowState.Minimized)
					LockedMapForm.Instance.WindowState = FormWindowState.Normal;
				LockedMapForm.Instance.Hide();
			}
		}

		public void stopMaid()
		{
			Proxy.Instance.UnregisterHandler(RedMsgHandler);
			Proxy.Instance.UnregisterHandler(CJHandler);
			Proxy.Instance.UnregisterHandler(CopyWalkHandler);
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
				buffSkill = MaidRemake.Instance.tbBuffSkill.Text.Split(',');
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
				monsterList = MaidRemake.Instance.tbAttPriority.Text.Split(',');
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
				case "CCMD":
					ClassPreset.CCMD();
					break;
				case "SSOT":
					ClassPreset.SSOT();
					break;
				case "NCM":
					ClassPreset.NCM();
					break;
				case "TK":
					ClassPreset.TK();
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

		private void lblLockedMapSetting_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (LockedMapForm.Instance.Visible || LockedMapForm.Instance.WindowState == FormWindowState.Minimized)
			{
				LockedMapForm.Instance.WindowState = FormWindowState.Normal;
				LockedMapForm.Instance.Hide();
			}
			else if (!LockedMapForm.Instance.Visible)
			{
				LockedMapForm.Instance.Show(this);
			}
		}
	}
}
