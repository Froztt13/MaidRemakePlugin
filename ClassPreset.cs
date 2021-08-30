using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidRemake
{
    public static class ClassPreset
    {
        public static void LR()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "3,1,2,4";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = "3";

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = false;
        }

        public static void LC()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "1,2,4";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = "3";
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = false;
        }

        public static void LOO()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "2,1,3,4";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = "1,2,3";

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = false;
        }

        public static void SC()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "3,1,2,4";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = "2,3";

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = false;
        }

        public static void AP()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "1,2,3";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = false;
        }

        public static void CCMD()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "1,1,1,1,3";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = "2";
            MaidRemake.Instance.numHealthPercent.Value = 75;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = true;
        }

        public static void SSOT()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "4,2,3,1,2";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = true;
        }

        public static void NCM()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "1,2,4,1,2,1,3";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = true;
        }

        public static void TK()
        {
            // skill list
            MaidRemake.Instance.tbSkillList.Text = "2,1,1,1,2,1,4,3,1";

            // heal skill
            MaidRemake.Instance.tbHealSkill.Text = String.Empty;
            MaidRemake.Instance.numHealthPercent.Value = 60;

            // buff skill
            MaidRemake.Instance.tbBuffSkill.Text = String.Empty;

            // additional settings
            MaidRemake.Instance.cbWaitSkill.Checked = true;
        }

        // for making sure the setting is applied

        public static void cbClear()
        {
            MaidRemake.Instance.cbUseHeal.Checked = false;
            MaidRemake.Instance.cbBuffIfStop.Checked = false;
        }

        public static void cbSet()
        {
            MaidRemake.Instance.cbUseHeal.Checked = true;
            MaidRemake.Instance.cbBuffIfStop.Checked = true;
        } 
    }
}
