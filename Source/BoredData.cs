using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DInterestsCallings
{
    class BoredData : MapComponent
    {
        public Dictionary<Pawn, float> boredomLevels = new Dictionary<Pawn, float>();
        public Dictionary<Pawn, int> napTimers = new Dictionary<Pawn, int>();

        List<Pawn> boredomKeys = new List<Pawn>();
        List<float> boredomVals = new List<float>();
        List<Pawn> napKeys = new List<Pawn>();
        List<int> napValues = new List<int>();

        public BoredData(Map map) : base(map) { }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look<Pawn, float>(ref boredomLevels, "BoredomLevels", LookMode.Reference, LookMode.Value, ref boredomKeys, ref boredomVals);
            Scribe_Collections.Look<Pawn, int>(ref napTimers, "NapTimers", LookMode.Reference, LookMode.Value, ref napKeys, ref napValues);
        }

    }
}
