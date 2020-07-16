using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using DInterests;

namespace DInterestsCallings
{
    public class Need_Compulsion : Need
    {

		private Trait TraitNeurotic
		{
			get
			{
				TraitDef neurotic = TraitDef.Named("Neurotic");
				return this.pawn.story.traits.GetTrait(neurotic);
			}
		}

		public SkillRecord InterestCompulsive
		{
			get
			{
				int compulse = InterestBase.interestList["DCompulsion"];
				if (compulse < 0)
					return null;
				foreach (SkillRecord sr in this.pawn.skills.skills)
				{
					if ((int)sr.passion == compulse)
						return sr;
				}
				return null;
			}
		}

		private SimpleCurve FallCurve
		{
			get
			{
				Trait traitNeurotic = this.TraitNeurotic;
				if (traitNeurotic == null)
					return Need_Compulsion.NormalDegreeFallCurve;
				else if (traitNeurotic.Degree == 1)
					return Need_Compulsion.NeuroticDegreeFallCurve;
				else
					return Need_Compulsion.VeryNeuroticDegreeFallCurve;
			}
		}

		private float FallPerNeedIntervalTick
		{
			get
			{
				Trait traitNeurotic = this.TraitNeurotic;
				float num = 1f;
				if (traitNeurotic != null) 
				{
					if (traitNeurotic.Degree == 1)
						num = FallPerTickFactorForNeurotic;
					else
						num = FallPerTickFactorForVeryNeurotic;
				}
				num *= this.FallCurve.Evaluate(this.CurLevel);
				return this.def.fallPerDay * num / 60000f * 150f;
			}
		}

		private Need_Compulsion.LevelThresholds CurrentLevelThresholds
		{
			get
			{
				var trait = this.TraitNeurotic;
				if (trait == null)
					return Need_Compulsion.NormalDegreeLevelThresholdsForMood;
				else if (trait.Degree == Need_Compulsion.NeuroticTraitDegree)
					return Need_Compulsion.NeuroticDegreeLevelThresholdsForMood;
				else
					return Need_Compulsion.VeryNeuroticDegreeLevelThresholdsForMood;
			}
		}

		public Need_Compulsion.MoodBuff MoodBuffForCurrentLevel
		{
			get
			{
				if (this.Disabled)
				{
					return Need_Compulsion.MoodBuff.Neutral;
				}
				Need_Compulsion.LevelThresholds currentLevelThresholds = this.CurrentLevelThresholds;
				float curLevel = this.CurLevel;
				if (curLevel <= currentLevelThresholds.extremelyNegative)
				{
					return Need_Compulsion.MoodBuff.ExtremelyNegative;
				}
				if (curLevel <= currentLevelThresholds.veryNegative)
				{
					return Need_Compulsion.MoodBuff.VeryNegative;
				}
				if (curLevel <= currentLevelThresholds.negative)
				{
					return Need_Compulsion.MoodBuff.Negative;
				}
				return Need_Compulsion.MoodBuff.Neutral;
			}
		}

		public override int GUIChangeArrow
		{
			get
			{
				float curInstantLevelPercentage = base.CurInstantLevelPercentage;
				if (curInstantLevelPercentage > base.CurLevelPercentage + 0.05f)
				{
					return 1;
				}
				if (curInstantLevelPercentage < base.CurLevelPercentage - 0.05f)
				{
					return -1;
				}
				return 0;
			}
		}

		public override bool ShowOnNeedList
		{
			get
			{
				return !this.Disabled;
			}
		}

		private bool Disabled
		{
			get
			{
				return this.InterestCompulsive == null;
			}
		}

		public void Notify_Worked(float xp)
		{
			this.CurLevel += xp * GainForWork;
		}

		public Need_Compulsion(Pawn pawn) : base(pawn){ }

		public override void SetInitialLevel()
		{
			this.CurLevel = 1f;
		}

		public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
		{
			Trait traitNeurotic = this.TraitNeurotic;
			SkillRecord skill = InterestCompulsive;
			if (skill != null && skill != this.associatedSkill)
			{
				this.threshPercents = new List<float>();
				Need_Compulsion.LevelThresholds currentLevelThresholds = this.CurrentLevelThresholds;
				this.threshPercents.Add(currentLevelThresholds.extremelyNegative);
				this.threshPercents.Add(currentLevelThresholds.veryNegative);
				this.threshPercents.Add(currentLevelThresholds.negative);
			}
			
			base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
		}

		public override void NeedInterval()
		{
			if (this.Disabled)
			{
				this.SetInitialLevel();
				return;
			}
			if (this.IsFrozen)
			{
				return;
			}
			this.CurLevel -= this.FallPerNeedIntervalTick;
		}

		public SkillRecord associatedSkill = null;

		public const int NeuroticTraitDegree = 1;
		public const int VeryNeuroticTraitDegree = 2;

		private const float FallPerTickFactorForNeurotic = 1.2f;
		private const float FallPerTickFactorForVeryNeurotic = 1.3f;

		public const float GainForWork = 0.004f;

		private static readonly SimpleCurve NormalDegreeFallCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.3f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.NormalDegreeLevelThresholdsForMood.negative, 0.5f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.NormalDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1f),
				true
			}
		};

		private static readonly SimpleCurve NeuroticDegreeFallCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.4f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.NeuroticDegreeLevelThresholdsForMood.negative, 0.6f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.NeuroticDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.10f),
				true
			}
		};

		private static readonly SimpleCurve VeryNeuroticDegreeFallCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.5f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.VeryNeuroticDegreeLevelThresholdsForMood.negative, 0.7f),
				true
			},
			{
				new CurvePoint(Need_Compulsion.VeryNeuroticDegreeLevelThresholdsForMood.negative + 0.001f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.20f),
				true
			}
		};

		private static readonly Need_Compulsion.LevelThresholds NormalDegreeLevelThresholdsForMood = new Need_Compulsion.LevelThresholds
		{
			extremelyNegative = 0.01f,
			veryNegative = 0.15f,
			negative = 0.3f,
		};

		private static readonly Need_Compulsion.LevelThresholds NeuroticDegreeLevelThresholdsForMood = new Need_Compulsion.LevelThresholds
		{
			extremelyNegative = 0.1f,
			veryNegative = 0.2f,
			negative = 0.4f,
		};

		private static readonly Need_Compulsion.LevelThresholds VeryNeuroticDegreeLevelThresholdsForMood = new Need_Compulsion.LevelThresholds
		{
			extremelyNegative = 0.1f,
			veryNegative = 0.3f,
			negative = 0.5f,
		};

		public enum MoodBuff
		{
			ExtremelyNegative,
			VeryNegative,
			Negative,
			Neutral
		}

		public struct LevelThresholds
		{
			public float extremelyNegative;
			public float veryNegative;
			public float negative;
		}
	}
}
 