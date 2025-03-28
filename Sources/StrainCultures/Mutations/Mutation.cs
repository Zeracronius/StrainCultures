using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Mutations
{
	public class Mutation : IExposable
	{
		public MutationDef? def;
		public float power = 0;
		public float priority = 0;
        protected MutationVariants? state = null;

        // Needs to be hooked into the render system.
        public List<PawnRenderNodeProperties> RenderNodeProperties => state == null ? [] : state.renderNodeProperties; 

        public virtual void SetMutationState(MutationVariants state, bool postLoad)
        {
            if (!postLoad && this.state != state)
            {
                OnStateDeactivated(this.state);
                this.state = state;
                OnStateActivated(state);
            }
        }
        public virtual void OnApplied(bool postLoad) { }
        public virtual void OnInit(bool postLoad) { }
        public virtual void OnRemoved(bool postLoad) { }

        protected virtual void OnStateActivated(MutationVariants state) { }
        protected virtual void OnStateDeactivated(MutationVariants? state) { }
        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref power, "power");
            Scribe_Values.Look(ref priority, "priority");
            Scribe_Defs.Look(ref def, "def");
        }
    }
}
