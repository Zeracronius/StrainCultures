using BigAndSmall;
using StrainCultures.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures
{
    public class MutationScore : ScoreData
    {
        public enum PawnType
        {
            HumanlikeAny,
            Humanlike,
            HumanlikeAnimal,
            ToolUser,
            Animal,
        }
        /// <summary>
        /// Influence strength of the mutation.
        /// </summary>
        public PawnType? pawnType = null;
        public FloatRange? allowedPowerRange = null;
        public List<TagValueRange> tagValueRanges = [];

        protected override void MatchObj(object obj, ref bool allMached, ref int matchCount)
        {
            if (obj is Tuple<Pawn, Mutated, float> pm)
            {
                (Pawn pawn, Mutated mutated, float power) = pm;
                MatchPawn(pawn, ref allMached, ref matchCount);
                MatchMutated(mutated, ref allMached, ref matchCount);
                MatchPower(ref allMached, ref matchCount, power);
            }
        }

        private void MatchPower(ref bool allMached, ref int matchCount, float power)
        {
            if (allowedPowerRange.HasValue)
            {
                if (!allowedPowerRange.Value.Includes(power)) allMached = false;
                else matchCount++;
            }
        }

        protected virtual void MatchMutated(Mutated mutated, ref bool allMached, ref int matchCount)
        {
            foreach (var tagValueR in tagValueRanges)
            {
                if (!tagValueR.range.Includes(mutated.mutationTagValues.TryGetValue(tagValueR.tag, fallback: float.MinValue))) allMached = false;
                else matchCount++;
            }
        }
    }
}
