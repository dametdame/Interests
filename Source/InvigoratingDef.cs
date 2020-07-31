using DInterests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace DInterestsCallings
{
    class InvigoratingDef : InterestDef
    {
        public override void HandleTick(SkillRecord sk, Pawn pawn)
        {  
            SkillDef sd = InterestBase.GetActiveSkill(pawn);
            if (sd == null)
                return;
            SkillRecord skill = pawn.skills.GetSkill(sd);
            if (skill != sk)
                return;

            Need_Rest restNeed = pawn.needs.TryGetNeed(NeedDefOf.Rest) as Need_Rest;
            if (restNeed == null)
            {
                //Log.Error("Got null rest need, wat");
                return;
            }

            // Rest fall per 150 ticks is 150f*1.58333332E-05f * RestFallFactor * (modifier based on tiredness level);
            // Perfect equilibrium is 200f* 1.58333332E-05f
            RestCategory rc = restNeed.CurCategory;
            float factor;
            switch (rc)
            {
                case RestCategory.Rested:
                    factor = 1.0f;
                    break;
                case RestCategory.Tired:
                    factor = 0.7f;
                    break;
                case RestCategory.VeryTired:
                    factor = 0.3f;
                    break;
                case RestCategory.Exhausted:
                    factor = 0.6f;
                    break;
                default:
                    factor = 999f;
                    break;
            }
            float restGain = (200f / 2.0f) * 1.58333332E-05f * factor;

            restNeed.CurLevel += restGain;
        }
        
    }
}
