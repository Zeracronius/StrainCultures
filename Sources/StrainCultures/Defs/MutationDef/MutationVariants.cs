using BigAndSmall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures
{
    public class MutationVariants : Scoreable
    {
        private List<MutationScore>? cachedConditions = null;
        public string label = "";
        public string description = "";
        protected MutationScore? condition;
        protected List<MutationScore> conditions = [];
        public List<PawnRenderNodeProperties> renderNodeProperties = [];
        // Having no conditions is perfectly valid, but should default lower than having conditions without a score set.
        public float defaultPriority = -0.1f;
        public float mutationPriorityFactor = 1;

        public override IEnumerable<IScoreProvider> Selectors => cachedConditions ??=
            condition == null ? conditions : [condition, .. conditions];
        public bool HasGraphics => !renderNodeProperties.NullOrEmpty();
        public override float GetDefaultValue => defaultPriority;
    }
}
