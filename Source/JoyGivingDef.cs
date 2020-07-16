using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DInterests;
using RimWorld;
using Verse;
using Verse.AI;

namespace DInterestsCallings
{
    class JoyGivingDef : InterestDef
    {
        public float visibleJoyFactorForSkill = 0.0020f; // equilibrium at many joy levels

        public override void UpdatePersistentWorkEffect(Pawn pawn, Pawn instigator)
        {
            if (pawn.CanSee(instigator))
            {
                pawn.needs.joy.CurLevel += visibleJoyFactorForSkill;
            }
        }
    }
}
