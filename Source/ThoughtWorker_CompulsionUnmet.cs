using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using DInterests;

namespace DInterestsCallings
{
    class ThoughtWorker_CompulsionUnmet : ThoughtWorker
    {

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			NeedDef compNeedDef = DefDatabase<NeedDef>.GetNamed("DCompulsionNeed", errorOnFail: true);
			Need_Compulsion compNeed = p.needs.TryGetNeed(compNeedDef) as Need_Compulsion;
			if (compNeed == null)
				return ThoughtState.Inactive;

			int moodBuffForCurrentLevel = (int)compNeed.MoodBuffForCurrentLevel;
			if (moodBuffForCurrentLevel == (int)Need_Compulsion.MoodBuff.Neutral)
				return ThoughtState.Inactive;

			return ThoughtState.ActiveAtStage(2-moodBuffForCurrentLevel);
		}

    }
}
