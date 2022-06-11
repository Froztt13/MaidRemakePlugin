using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidRemake
{
	internal class MaidConfig
	{
		public string SkillList;

		public int SkillDelay;

		public bool WaitSkill;

		public bool StopFailedGoto;

		public bool LockedZoneHandler;

		public string LockedZoneHandlerMaps;

		public int RelogDelay;

		public bool GlobalHotkey;

		public bool Unfollow;

		public bool StopAttack;

		public string SafeSkillList;

		public int SafeSkillHP;

		public string BuffStopAttack;

		public bool AttackPriority;

		public string AttackPriorityMonster;

		public bool CopyWalk;
	}
}
