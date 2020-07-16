using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using DInterests;

// ThoughtDefs are in 1.1/Defs/ThoughtDefs/Thoughts_Situation_Special.xml

namespace DInterestsCallings
{
	class ThoughtWorker_InterestingWork : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			SkillDef active = InterestBase.GetActiveSkill(p);
			if (active == null)
				return ThoughtState.Inactive;
			
			SkillRecord skill = p.skills.GetSkill(active);
			if (skill == null)
			{
				return ThoughtState.Inactive;
			}
			int passion = (int)skill.passion;

			// these could return -1 but passion won't be -1
			var minorAversion = InterestBase.interestList["DMinorAversion"];
			if (passion == minorAversion)
				return ThoughtState.ActiveAtStage(0);
			var majorAversion = InterestBase.interestList["DMajorAversion"];
			if (passion == majorAversion)
				return ThoughtState.ActiveAtStage(1);

			return ThoughtState.Inactive;
		}
	}
}
