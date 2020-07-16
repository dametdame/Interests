using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DInterests;
using RimWorld;
using Verse;
using Verse.Noise;

namespace DInterestsCallings
{
    public class HediffGiver_Allergic : HediffGiver
    {

        public float gainRate = 60.0f/2500.0f/10.0f; // 10 hours to 100%, 7 hours to anaphylaxis

        public HediffGiver_Allergic() { }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (!pawn.IsColonist)
                return;
            var active = InterestBase.GetActiveSkill(pawn);
            if (active == null)
            {
                DecreaseAllergy(pawn);
                return;
            }
            SkillRecord skill = pawn.skills.GetSkill(active);
            if (skill == null)
                return;
            int passion = (int)skill.passion;
            int allergic = InterestBase.interestList["DAllergic"];
            HediffSet hediffSet = pawn.health.hediffSet;
            Hediff firstHediffOfDef = hediffSet.GetFirstHediffOfDef(this.hediff, false);
            if (passion != allergic && firstHediffOfDef != null) // pawn's active skill isn't causing an allergy, but they have the allergic hediff
            {
                DecreaseAllergy(pawn);
                //firstHediffOfDef.Severity -= gainRate;
            }
            else if (passion == allergic) // pawn's active skill is allergy causing
            {
                IncreaseAllergy(pawn);
            } 

            base.OnIntervalPassed(pawn, cause);
        }

        public void IncreaseAllergy(Pawn pawn)
        {
            HealthUtility.AdjustSeverity(pawn, this.hediff, gainRate);
        }

        public void DecreaseAllergy(Pawn pawn)
        {
            HealthUtility.AdjustSeverity(pawn, this.hediff, -gainRate);
        }
    }
}
