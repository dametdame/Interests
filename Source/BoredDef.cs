using DInterests;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DInterestsCallings
{

    class BoredDef : InterestDef
    {
        

        public float boredomGain = 200.0f/15000.0f; // 6 hours of work
        public float napPossibleThreshold = 0.6f; // pretty bored
        public int napLength = 5000/200; // 2 hours

        public override void HandleTick(SkillRecord sr, Pawn pawn)
        {
            if (pawn.Map == null)
                return;
            var boredomLevels = pawn.Map.GetComponent<BoredData>().boredomLevels;
            var napTimers = pawn.Map.GetComponent<BoredData>().napTimers;

            if (!boredomLevels.ContainsKey(pawn))
            {
                boredomLevels.Add(pawn, 0.0f);
                napTimers.Add(pawn, 0);
            }

            Pawn_JobTracker jt = pawn.jobs;
            if (jt == null) // no jobs, maybe idle? doesn't encompass sleeping
            {
                BoredomUntick(pawn);
                return;
            }
            JobDriver curDriver = jt.curDriver;
            if (curDriver == null) // no driver, not sure what this would mean but seems to happen
            {
                BoredomUntick(pawn);
                return;
            }

            if (curDriver.asleep && napTimers[pawn] > 0) // dude's taking a nap from boredom
            {
                Log.Message(napTimers[pawn].ToString());
                if (napTimers[pawn] == 1) // wake up
                    RestUtility.WakeUp(pawn);
                napTimers[pawn]--;
                BoredomUntick(pawn);
                return;
            }
            napTimers[pawn] = 0; // just in case the pawn's awake without a 0 nap timer

            if (pawn.skills == null) // no skills
            {
                BoredomUntick(pawn);
                return;
            }
            SkillDef active = curDriver.ActiveSkill;
            if (active != sr.def) // the skill with bored passion isn't the one being used
            {
                BoredomUntick(pawn);
                return;
            }
            // ok, the pawn's awake and it's doing something boring
            BoredomTick(pawn);
            Log.Message(boredomLevels[pawn].ToString());

            float boredomLevel = boredomLevels[pawn];
            if (boredomLevel > napPossibleThreshold) // pretty bored
            {
                
                float rand = Rand.Value*(1-napPossibleThreshold); // [0, (1 - napThreshold)], something like [0, 0.4]
                if (rand < (boredomLevel - napPossibleThreshold)) // fall asleep
                {
                    Job napJob = JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position);
                    napJob.forceSleep = true;
                    pawn.jobs.StartJob(napJob, JobCondition.InterruptForced, null, true, true, null, new JobTag?(JobTag.InMentalState), false, false);
                    if (PawnUtility.ShouldSendNotificationAbout(pawn))
                    {
                        Messages.Message("DMessageBoredNap".Translate(sr.def.defName.ToLower(), pawn), pawn, MessageTypeDefOf.SilentInput, true);
                        //Messages.Message("DMessageBoredNap".Translate(this.pawn.LabelShort, this.pawn), this.pawn, MessageTypeDefOf.NegativeEvent, true);
                    }
                    napTimers[pawn] = napLength;
                    boredomLevels[pawn] *= 0.25f;
                }
            }
            
        }

        public void BoredomTick(Pawn p)
        {
            ChangeBoredom(boredomGain, p);
        }

        public void BoredomUntick(Pawn p)
        {
            ChangeBoredom(-boredomGain, p);
        }

        public void ChangeBoredom(float amount, Pawn pawn)
        {
            pawn.Map.GetComponent<BoredData>().boredomLevels[pawn] = Mathf.Clamp(pawn.Map.GetComponent<BoredData>().boredomLevels[pawn] + amount, 0.0f, 1.0f);
        }

    }
}
