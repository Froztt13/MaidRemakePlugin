using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplePacketPlugin
{
    public static class ClassPreset
    {
        public static void LR()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "3,1,2,4";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = "3";

            // additional settings
            Main.Instance.cbWaitSkill.Checked = false;
        }

        public static void LC()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "1,2,4";

            // heal skill
            Main.Instance.tbHealSkill.Text = "3";
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = false;
        }

        public static void LOO()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "2,1,3,4";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = "1,2,3";

            // additional settings
            Main.Instance.cbWaitSkill.Checked = false;
        }

        public static void SC()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "3,1,2,4";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = "2,3";

            // additional settings
            Main.Instance.cbWaitSkill.Checked = false;
        }

        public static void AP()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "1,2,3";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = false;
        }

        public static void CCMD()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "1,1,1,1,3";

            // heal skill
            Main.Instance.tbHealSkill.Text = "2";
            Main.Instance.numHealthPercent.Value = 75;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = true;
        }

        public static void SSOT()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "4,2,3,1,2";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = true;
        }

        public static void NCM()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "1,2,4,1,2,1,3";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = true;
        }

        public static void TK()
        {
            // skill list
            Main.Instance.tbSkillList.Text = "2,1,1,1,2,1,4,3,1";

            // heal skill
            Main.Instance.tbHealSkill.Text = String.Empty;
            Main.Instance.numHealthPercent.Value = 60;

            // buff skill
            Main.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            Main.Instance.cbWaitSkill.Checked = true;
        }

        // for making sure the setting is applied

        public static void cbClear()
        {
            Main.Instance.cbUseHeal.Checked = false;
            Main.Instance.cbBuffIfStop.Checked = false;
        }

        public static void cbSet()
        {
            Main.Instance.cbUseHeal.Checked = true;
            Main.Instance.cbBuffIfStop.Checked = true;
        } 
    }
}
