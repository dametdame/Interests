using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DInterests;
using RimWorld;
using UnityEngine;
using Verse;

namespace DInterestsCallings
{
    class InterestsSettings : ModSettings
    {

        public static Vector2 scrollPos = new Vector2();

        public static Dictionary<string, int> learnRates;
        public static Dictionary<string, bool> canAppear;
        public static Dictionary<string, int> defaults = null;

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref learnRates, "learnRates", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref canAppear, "canAppear", LookMode.Value, LookMode.Value);
            base.ExposeData();
        }

        public static void WriteAll()
        {
            if (defaults == null)
            {
                defaults = new Dictionary<string, int>();
                foreach (InterestDef id in InterestBase.interestList)
                    defaults.Add(id.defName, (int)Math.Round((id.learnFactor*100f)));
            }
            foreach (var kvp in learnRates ?? Enumerable.Empty<KeyValuePair<string, int>>())
            {
                int id = InterestBase.interestList[kvp.Key];
                if (id != -1)
                {
                    InterestBase.interestList[id].learnFactor = ((float)kvp.Value) / 100f;
                }
            }
            foreach (var kvp in canAppear ?? Enumerable.Empty<KeyValuePair<string, bool>>())
            {
                int id = InterestBase.interestList[kvp.Key];
                if (id != -1)
                {
                    InterestBase.interestList[id].canAppear = kvp.Value;
                }
            }
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);
            
            float height =  52 + InterestBase.interestList.Count * (Text.LineHeight * 3 + 12*1 + 5);
            Rect contents = new Rect(rect.x, rect.y, rect.width - 30f, height);
            Widgets.BeginScrollView(rect, ref scrollPos, contents, true);
            ls.ColumnWidth = contents.width *2.0f / 3.0f;
            ls.Begin(contents);
            ls.Gap();
            ls.Label("Warning: Disabling Minor or Major passion may break some mods.");

            if (learnRates == null)
                learnRates = new Dictionary<string, int>();
            if (canAppear == null)
                canAppear = new Dictionary<string, bool>();

            foreach (InterestDef id in InterestBase.interestList)
            {
                ls.GapLine();
                DrawInterest(id, ref contents, ls);
            }
           
            ls.End();
            Widgets.EndScrollView();
        }

        public static void DrawInterest(InterestDef id, ref Rect root, Listing_Standard ls)
        {
            if (!learnRates.ContainsKey(id.defName))
            {
                learnRates.Add(id.defName, (int)Math.Round((id.learnFactor*100f)));
            }
            if (!canAppear.ContainsKey(id.defName))
            {
                canAppear.Add(id.defName, id.canAppear);
            }
            int learnRate = learnRates[id.defName];
            bool appears = canAppear[id.defName];

            Texture2D image = id.GetTexture();
            Rect labelRect;
            Rect rectLine = ls.GetRect(Text.LineHeight);

            if (image != null)
            {
                Rect imageRect = rectLine.LeftPartPixels(Text.LineHeight);
                labelRect = rectLine.RightPartPixels(rectLine.width - Text.LineHeight - 5);
                GUI.DrawTexture(imageRect, image);
            }
            else
            {
                labelRect = rectLine;
            }

            TextAnchor buffer = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, id.LabelCap);
            Text.Anchor = buffer;

            if(id != InterestBase.interestList.GetDefault())
                ls.CheckboxLabeled("Enabled", ref appears, "Allow this interest to appear on new pawns?");

            Rect learnLine = ls.GetRect(Text.LineHeight);
            Rect learnLabel = learnLine.LeftHalf();
            Rect learnField = learnLine.RightHalf();
            buffer = Text.Anchor;
            string lr = "Learn rate";
            if (defaults.ContainsKey(id.defName))
                lr += " (default = " + defaults[id.defName] + ")";
            lr += ": ";
            Widgets.Label(learnLabel, lr);
            Text.Anchor = buffer;
            string stringBuffer = learnRate.ToString();
            Widgets.TextFieldNumeric(learnField, ref learnRate, ref stringBuffer, 0, 10000);

            learnRates[id.defName] = learnRate;
            canAppear[id.defName] = appears;
        }
    }
}
