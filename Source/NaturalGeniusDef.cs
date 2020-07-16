using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DInterests;
using RimWorld;
using Verse;


namespace DInterestsCallings
{
    class NaturalGeniusDef : InterestDef
    {
        public override void HandleTick(SkillRecord sr, Pawn pawn)
        {
            if(pawn.Awake())
                sr.Learn(4.0f/learnFactor, true);
        }
    }
}
