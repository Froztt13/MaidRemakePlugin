using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grimoire.Game;
using Grimoire.Networking;
using DarkUI.Forms;
using Grimoire.Tools;
using MaidRemake.LockedMapHandle;
using MaidRemake.WhitelistMap;
using System.IO;
using Newtonsoft.Json;
using MaidRemake.Handlers;
using Newtonsoft.Json.Linq;
using Grimoire.UI;

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

		public JoinMapHandler JoinMapHandler { get; } = new JoinMapHandler();

		public WarningMsgHandler RedMsgHandler { get; } = new WarningMsgHandler();

		public CopyWalkHandler CopyWalkHandler { get; } = new CopyWalkHandler();

		public PartyChatHandler PartyChatHandler { get; } = new PartyChatHandler();

		public PartyInvitationHandler PartyInvitationHandler { get; } = new PartyInvitationHandler();

		private int healthPercent => (int)MaidRemake.Instance.numHealthPercent.Value;

		string[] buffSkill = null;
		int buffIndex = 0;

		string[] healSkill = null;
		int healIndex = 0;

		string[] monsterList = null;

		bool onPause = false;

		bool forceSkill = false;

		Stopwatch stopwatch = new Stopwatch();

		public MaidRemake()
		{
			InitializeComponent();

			KeyPreview = true;
			//this.KeyDown += new KeyEventHandler(this.hotkey);
			if (Player.IsLoggedIn) cmbGotoUsername.Text = Player.Username;
			this.Text = $"Maid Remake {Loader.Version}";
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

				Proxy.Instance.RegisterHandler(JoinMapHandler);

				if (!cbUnfollow.Checked)
					Proxy.Instance.RegisterHandler(CJHandler);

				if (cbCopyWalk.Checked)
					Proxy.Instance.RegisterHandler(CopyWalkHandler);

				Flash.FlashCall += AnimsMsgHandler;

				if (!cbUnfollow.Checked && Player.IsLoggedIn && !World.IsMapLoading && isPlayerInMyRoom && !isPlayerInMyCell)
					Player.GoToPlayer(targetUsername);

				if (cbAttackPriority.Checked)
					monsterList = tbAttPriority.Text.Split(',');

				if (cbUseHeal.Checked)
					healSkill = tbHealSkill.Text.Split(',');

				if (cbBuffIfStop.Checked)
				{
					buffSkill = tbBuffSkill.Text.Split(',');
					buffIndex = 0;
				}

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
						if ((isPlayerInMyRoom || cbUnfollow.Checked) && Player.IsLoggedIn && !World.IsMapLoading && !onPause)
						{
							gotoTry = 0;

							if (!Player.IsAlive)
							{
								skillIndex = 0;
								World.SetSpawnPoint();
								await Task.Delay(500);
								forceSkill = false;
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
								{
									Player.CancelAutoAttack();
									Player.CancelTarget();
								}

								if (cbBuffIfStop.Checked && tbBuffSkill.Text != String.Empty)
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

							if (Player.HasTarget)
							{
								if (forceSkill)
								{
									string skillAct = numSkillAct.Value.ToString();
									await Task.Delay(1000);
									await Task.Delay(Player.SkillAvailable(skillAct));
									Player.UseSkill(skillAct);
									await Task.Delay(500);
									Player.UseSkill(skillAct);
									forceSkill = false;
								} else
								{
									Player.UseSkill(skillList[skillIndex]);
								}
							}

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

		private Grimoire.Networking.Message CreateMessage(string raw)
		{
			if (raw != null && raw.Length > 0)
			{
				switch (raw[0])
				{
					case '%':
						return new XtMessage(raw);
					case '<':
						return new XmlMessage(raw);
					case '{':
						return new JsonMessage(raw);
				}
			}

			return null;
		}


		private void AnimsMsgHandler(AxShockwaveFlashObjects.AxShockwaveFlash flash, string function, params object[] args)
		{
			if (function != "packetFromServer") return;
			try
			{
				Grimoire.Networking.Message message = CreateMessage((string)args[0]);
				JsonMessage jsonMessage = message as JsonMessage;
				if (jsonMessage != null)
				{
					if (jsonMessage.DataObject["anims"] != null)
					{
						JArray anims = (JArray)jsonMessage.DataObject["anims"];
						if (anims != null)
						{
							foreach (JObject anim in anims)
							{
								string msg = anim?["msg"]?.ToString()?.ToLower();
								if (msg != null)
								{
									//debug($"msg: {msg}");
									if (tbSpecialMsg.Text.Contains(","))
									{
										foreach (string m in tbSpecialMsg.Text.Split(','))
										{
											if (msg.Contains(m))
											{
												forceSkill = true;
											}
										}
									} 
									else
									{
										if (msg.Contains(tbSpecialMsg.Text))
										{
											forceSkill = true;
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				debug($"[MAID] e: {e}");
			}
		}
		private void debug(string text)
		{
			LogForm.Instance.AppendDebug(text);
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
				Player.MoveToCell("Enter", "Spawn");
			await Task.Delay(500);
			Player.GoToPlayer(targetUsername);
			//await Proxy.Instance.SendToServer($"%xt%zm%cmd%1%goto%{targetUsername}%");
		}

		/* UI state */

		public void startUI()
		{
			tbSpecialMsg.Enabled = false;
			numSkillAct.Enabled = false;
			cmbGotoUsername.Enabled = false;
			tbSkillList.Enabled = false;
			gbOptions.Enabled = false;
			cbWaitSkill.Enabled = false;
			btnMe.Enabled = false;
			cbCopyWalk.Enabled = false;
			if (LockedMapForm.Instance.Visible)
			{
				if (LockedMapForm.Instance.WindowState == FormWindowState.Minimized)
					LockedMapForm.Instance.WindowState = FormWindowState.Normal;
				LockedMapForm.Instance.Hide();
			}
			if (WhitelistMapForm.Instance.Visible)
			{
				if (WhitelistMapForm.Instance.WindowState == FormWindowState.Minimized)
					WhitelistMapForm.Instance.WindowState = FormWindowState.Normal;
				WhitelistMapForm.Instance.Hide();
			}
		}

		public void stopMaid()
		{
			Proxy.Instance.UnregisterHandler(RedMsgHandler);
			Proxy.Instance.UnregisterHandler(CJHandler);
			Proxy.Instance.UnregisterHandler(JoinMapHandler);
			Proxy.Instance.UnregisterHandler(CopyWalkHandler);
			Flash.FlashCall -= AnimsMsgHandler;

			tbSpecialMsg.Enabled = true;
			numSkillAct.Enabled = true;
			cmbGotoUsername.Enabled = true;
			tbSkillList.Enabled = true;
			gbOptions.Enabled = true;
			cbWaitSkill.Enabled = true;
			btnMe.Enabled = true;
			cbCopyWalk.Enabled = true;
			cbEnablePlugin.Checked = false;
			onPause = false;
		}

		/* Hotkey */

		private void cbEnableGlobalHotkey_CheckedChanged(object sender, EventArgs e)
		{
			cbUnfollow.Enabled = cbEnableGlobalHotkey.Checked;
			cbStopAttack.Enabled = cbEnableGlobalHotkey.Checked;
			if (cbEnableGlobalHotkey.Checked)
			{
				kbh.OnKeyPressed += globalHotkey;
				kbh.OnKeyUnpressed += (s, ek) => { };
				//this.KeyDown -= hotkey;

				kbh.HookKeyboard();
			}
			else
			{
				kbh.OnKeyPressed -= globalHotkey;
				kbh.OnKeyUnpressed -= (s, ek) => { };
				//this.KeyDown += new KeyEventHandler(this.hotkey);

				kbh.UnHookKeyboard();
			}
		}

		private void hotkey(object sender, KeyEventArgs e)
		{
			if (cmbGotoUsername.Focused || tbAttPriority.Focused)
				return;

			switch (e.KeyCode)
			{
				case Keys.R:
					// LockCell: R
					e.SuppressKeyPress = true;
					cbUnfollow.Checked = cbUnfollow.Checked ? false : true;
					break;
				case Keys.T:
					// StopAttack: T
					e.SuppressKeyPress = true;
					cbStopAttack.Checked = cbStopAttack.Checked ? false : true;
					break;
			}
		}

		private void globalHotkey(object sender, Keys e)
		{
			if (cmbGotoUsername.Focused || tbAttPriority.Focused)
				return;

			switch(e)
			{
				case Keys.R:
					// LockCell: R
					cbUnfollow.Checked = cbUnfollow.Checked ? false : true;
					break;
				case Keys.T:
					// StopAttack: T
					cbStopAttack.Checked = cbStopAttack.Checked ? false : true;
					break;
			}
		}

		/* Other Control */

		public void pauseFollow()
		{
			if (onPause) return;
			if (cbCopyWalk.Checked) 
				Proxy.Instance.UnregisterHandler(CopyWalkHandler);
			onPause = true;
			logDebug("onPause: true");
		}

		public void resumeFollow()
		{
			if (!onPause) return;
			if (cbCopyWalk.Checked) 
				Proxy.Instance.RegisterHandler(CopyWalkHandler);
			onPause = false;
			logDebug("onPause: false");
		}

		private void cbLockCell_CheckedChanged(object sender, EventArgs e)
		{
			if (cbEnableGlobalHotkey.Checked == false) return;
			if(cbUnfollow.Checked)
			{
				Proxy.Instance.UnregisterHandler(CJHandler);
				if (cbCopyWalk.Checked) Proxy.Instance.UnregisterHandler(CopyWalkHandler);
			}
			else
			{
				Proxy.Instance.RegisterHandler(CJHandler);
				if (cbCopyWalk.Checked) Proxy.Instance.RegisterHandler(CopyWalkHandler);
			}
		}

		private void cbStopAttack_CheckedChanged(object sender, EventArgs e)
		{
			if (cbEnableGlobalHotkey.Checked == false) return;
			if (cbStopAttack.Checked)
			{
				lbStopAttackBg.BackColor = System.Drawing.Color.DeepPink;
				stopwatch.Reset();
				stopwatch.Start();
				timerStopAttack.Enabled = true;
				cbStopAttack.BackColor = System.Drawing.Color.Magenta;
				Player.CancelAutoAttack();
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
			tbHealSkill.Enabled = !cbUseHeal.Checked;
			numHealthPercent.Enabled = !cbUseHeal.Checked;
			if (cbUseHeal.Checked)
			{
				healSkill = tbHealSkill.Text.Split(',');
			}
		}

		private void cbBuffIfStop_CheckedChanged(object sender, EventArgs e)
		{
			cbBuffIfStop.Enabled = !cbBuffIfStop.Checked;
			if (cbBuffIfStop.Checked)
			{
				buffSkill = tbBuffSkill.Text.Split(',');
				buffIndex = 0;
			}
		}

		private void cbAttackPriority_CheckedChanged(object sender, EventArgs e)
		{
			tbAttPriority.Enabled = !cbAttackPriority.Checked;
			if (cbAttackPriority.Checked)
			{
				monsterList = tbAttPriority.Text.Split(',');
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

		private void btnSave_Click(object sender, EventArgs e)
		{
			MaidConfig maidConfig = new MaidConfig
			{
				Target = cmbGotoUsername.Text,
				SkillList = tbSkillList.Text,
				SkillDelay = (int)numSkillDelay.Value,
				WaitSkill = cbWaitSkill.Checked,
				StopFailedGoto = cbStopIf.Checked,
				LockedZoneHandler = cbHandleLockedMap.Checked,
				LockedZoneHandlerMaps = LockedMapForm.Instance.tbLockedMapAlternative.Text,
				WhitelistMap = cbWhitelistMap.Checked,
				WhitelistMapMaps = WhitelistMapForm.Instance.tbWhitelistMap.Text,
				RelogDelay = (int)numRelogDelay.Value,
				GlobalHotkey = cbEnableGlobalHotkey.Checked,
				SafeSkill = cbUseHeal.Checked,
				SafeSkillList = tbHealSkill.Text,
				SafeSkillHP = (int)numHealthPercent.Value,
				BuffStopAttack = cbBuffIfStop.Checked,
				BuffStopAttackList = tbBuffSkill.Text,
				AttackPriority = cbAttackPriority.Checked,
				AttackPriorityMonster = tbAttPriority.Text,
				CopyWalk = cbCopyWalk.Checked,
				SpecialMsg = tbSpecialMsg.Text,
				SpecialAct = (int)numSkillAct.Value,
			};
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Title = "Save config";
				saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Config");
				saveFileDialog.Filter = "Maid config|*.json";
				saveFileDialog.DefaultExt = ".json";
				saveFileDialog.CheckFileExists = false;
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(maidConfig, Formatting.Indented));
						string[] path = saveFileDialog.FileName.Split('\\');
						gbConfig.Text = $"Config : {path[path.Length-1]}";
					}
					catch (Exception ex)
					{
						MessageBox.Show("Unable to save config: " + ex.Message);
					}
				}
			}
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Title = "Load config";
				openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Config");
				openFileDialog.Filter = "Maid config|*.json";
				openFileDialog.DefaultExt = ".json";
				if (openFileDialog.ShowDialog() == DialogResult.OK && 
					TryDeserialize(File.ReadAllText(openFileDialog.FileName), out MaidConfig config))
				{
					gbConfig.Text = $"Config : {openFileDialog.SafeFileName}";
					cmbGotoUsername.Text = config.Target;
					tbSkillList.Text = config.SkillList;
					numSkillDelay.Value = config.SkillDelay;
					cbWaitSkill.Checked = config.WaitSkill;
					cbStopIf.Checked = config.StopFailedGoto;
					cbHandleLockedMap.Checked = config.LockedZoneHandler;
					LockedMapForm.Instance.tbLockedMapAlternative.Text = config.LockedZoneHandlerMaps;
					cbWhitelistMap.Checked = config.WhitelistMap;
					WhitelistMapForm.Instance.tbWhitelistMap.Text = config.WhitelistMapMaps;
					numRelogDelay.Value = config.RelogDelay;
					cbEnableGlobalHotkey.Checked = config.GlobalHotkey;
					cbUseHeal.Checked = config.SafeSkill;
					tbHealSkill.Text = config.SafeSkillList;
					numHealthPercent.Value = config.SafeSkillHP;
					cbBuffIfStop.Checked = config.BuffStopAttack;
					tbBuffSkill.Text = config.BuffStopAttackList;
					cbAttackPriority.Checked = config.AttackPriority;
					tbAttPriority.Text = config.AttackPriorityMonster;
					cbCopyWalk.Checked = config.CopyWalk;
					tbSpecialMsg.Text = config.SpecialMsg;
					numSkillAct.Value = config.SpecialAct;
				}
			}
		}

		private bool TryDeserialize(string json, out MaidConfig config)
		{
			try
			{
				config = JsonConvert.DeserializeObject<MaidConfig>(json);
				return true;
			}
			catch (Exception e) { MessageBox.Show(e.ToString()); }
			config = null;
			return false;
		}

		private void cbPartyCmd_CheckedChanged(object sender, EventArgs e)
		{
			if (cbPartyCmd.Checked)
			{
				Proxy.Instance.RegisterHandler(PartyInvitationHandler);
				Proxy.Instance.RegisterHandler(PartyChatHandler);
			} 
			else
			{
				Proxy.Instance.UnregisterHandler(PartyInvitationHandler);
				Proxy.Instance.UnregisterHandler(PartyChatHandler);
			}
		}

		private void lblWhitelistMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (WhitelistMapForm.Instance.Visible || WhitelistMapForm.Instance.WindowState == FormWindowState.Minimized)
			{
				WhitelistMapForm.Instance.WindowState = FormWindowState.Normal;
				WhitelistMapForm.Instance.Hide();
			}
			else if (!WhitelistMapForm.Instance.Visible)
			{
				WhitelistMapForm.Instance.Show(this);
			}
		}

		public void logDebug(string msg)
		{
			Grimoire.UI.LogForm.Instance.AppendDebug($"[MaidRemake] {msg}");
		}

		private void btnMe_Click(object sender, EventArgs e)
		{
			if (Player.IsLoggedIn) cmbGotoUsername.Text = Player.Username;
		}
	}
}
