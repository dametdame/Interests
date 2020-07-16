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
    class CompulsionDef : InterestDef
    {

        public override void HandleLearn(ref float xp, SkillRecord sk, Pawn pawn, ref bool direct)
        {
            if (!pawn.IsColonist)
                return;

            if (xp < 0 || direct)
                return;
            NeedDef compNeedDef = DefDatabase<NeedDef>.GetNamed("DCompulsionNeed", errorOnFail: true);

            Need_Compulsion compNeed = pawn.needs.TryGetNeed(compNeedDef) as Need_Compulsion;
            if (compNeed == null)
            {
                Log.Error("Failed to find need associated with DCompulsionNeed");
            }

            compNeed.Notify_Worked(xp);
        }

    }
}
