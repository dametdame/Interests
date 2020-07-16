using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace DInterestsCallings
{
    class InterestsMod : Mod
    {

        InterestsSettings settings;

        public InterestsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<InterestsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            InterestsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "[D] Interests";
        }


        public override void WriteSettings()
        {
            InterestsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}
