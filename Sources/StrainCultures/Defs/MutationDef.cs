using BigAndSmall;
using RimWorld;
using StrainCultures.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures
{
    public class MutationDef : Def
    {
        private static List<MutationDef>? allDefs = null;
        public static List<MutationDef> AllDefs => allDefs ??= DefDatabase<MutationDef>.AllDefsListForReading;

        public List<TagValue> strainTags = [];
        public List<string> conflictingMutations = [];
        public List<MutationVariants> variants = [];
        /// <summary>
        /// Priority of the mutation. If several MutationDefs conflict they will be sorted by their strain-influence multiplied
        /// by this value, or a stage if an override there exists.
        /// </summary>
        protected float priorityFactor = 1;

        public bool TryGetMutationState(Mutated mutatedHediff, out MutationVariants? state, out float score)
        {
            float internalScore = GetInternalScore(mutatedHediff.mutationTagValues);
            state = null;
            score = float.MinValue;
            if (internalScore <= 0)
            {
                return false;
            }
            var scoredStates = variants.Select(s=>(state:s, score:s.GetScore(mutatedHediff))).Where(s => s.score != null);
            if (scoredStates.Any())
            {
                var result = scoredStates.MaxBy(s => s.score).state;
                state = result;
                score = internalScore * priorityFactor * result.mutationPriorityFactor;
                return true;
            }
            return false;
        }

        protected float GetInternalScore(Dictionary<string, float> pawnTagValues)
        {
            float score = 0;
            foreach (var tagValue in strainTags)
            {
                if (pawnTagValues.TryGetValue(tagValue.tag, out var pawnTagValue))
                {
                    score += tagValue.value * pawnTagValue;
                }
            }
            return score;
        }
    }
}
