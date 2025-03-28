using BigAndSmall;
using RimWorld;
using StrainCultures.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace StrainCultures
{
    public class Need_Corruption : Need
    {
        private float lastEffectiveDelta;
        const float dailyGain = 0.5f;
        const int ticksPerDay = 60000;
        const int needTrackerTick = 150;
        const float CorruptionGainPerTick = dailyGain / ticksPerDay * needTrackerTick;

        public override int GUIChangeArrow
        {
            get
            {
                if (IsFrozen)
                {
                    return 0;
                }
                return Math.Sign(lastEffectiveDelta);
            }
        }

        public override float MaxLevel => pawn.GetStatValue(DefOfStrains.SC_Defiance, cacheStaleAfterTicks: 1000);

        public float TargetLevel => pawn.GetStatValue(DefOfStrains.SC_Corruption, cacheStaleAfterTicks: 1000);

        public override bool ShowOnNeedList => !Disabled;

        private bool Disabled
        {
            get
            {
                if (!pawn.Dead)
                {
                    return TargetLevel > 0;
                }
                return true;
            }
        }

        public Need_Corruption(Pawn pawn)
            : base(pawn)
        {
            threshPercents =
            [
                0.6f,
                0.4f,
                0.2f,
                0.05f,
            ];
        }

        public override void SetInitialLevel()
        {
            CurLevel = 1f;
        }

        public override void NeedInterval()
        {
            if (Disabled)
            {
                CurLevel = 1f;
            }
            else if (!IsFrozen)
            {
                float targetLevel = TargetLevel;
                float curLevel = CurLevel;
                if (!CurLevel.ApproximatelyEquals(targetLevel))
                {
                    if (curLevel < targetLevel)
                    {
                        CurLevel += CorruptionGainPerTick;
                    }
                    else if (curLevel > targetLevel)
                    {
                        CurLevel -= CorruptionGainPerTick;
                    }
                }
            }
        }
    }
}
