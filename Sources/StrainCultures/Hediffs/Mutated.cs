using StrainCultures.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Hediffs
{
	/// <summary>
	/// Contains and handles all the mutations applied to a pawn, and is responsible for drawing them.
	/// </summary>
	public class Mutated : Hediff
	{
		private List<Mutation> _mutations = [];
        public Dictionary<string, float> mutationTagValues = new()
        {
           { "BasicWings", 99 }  // Test.
        };

        public Mutated()
		{
			
		}

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            pawn.needs.AddOrRemoveNeedsAsAppropriate();
            RefreshMutations(false);
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.needs.AddOrRemoveNeedsAsAppropriate();
        }

        public void RefreshMutations(bool postLoad)
		{
			var mutationDefs = MutationDef.AllDefs;
			List<(MutationDef def, MutationVariants variants, float score)> allValidMutations = [];
			foreach (var mutation in mutationDefs)
			{
                if (mutation.TryGetMutationState(this, out MutationVariants? state, out float score))
                {
                    allValidMutations.Add((mutation, state!, score));
                }
            }

			allValidMutations = [.. allValidMutations.OrderByDescending(m => m.score)];
			int idx = 0;
            while (idx < allValidMutations.Count)
			{
                (MutationDef def, MutationVariants variants, float score) mutation = allValidMutations[idx];
                foreach(var conflictTag in mutation.def.conflictingMutations)
				{
                    allValidMutations.RemoveAll(m => m.def.conflictingMutations.Contains(conflictTag));
                }
            }

            List<Mutation> mutationTempList = new();
            foreach (var mutDef in allValidMutations)
            {
                bool existingFound = false;
                if (_mutations.FirstOrDefault(m => m.def == mutDef.def) is not Mutation mutation)
                {
                    mutation = new Mutation
                    {
                        def = mutDef.def
                    };
                    mutationTempList.Add(mutation);
                    mutation.OnApplied(postLoad);
                }
                else
                {
                    existingFound = true;
                }
                mutation.SetMutationState(mutDef.variants, postLoad);
                if (!existingFound)
                {
                    mutation.OnInit(postLoad);
                }
            }

            foreach (var mutation in _mutations)
            {
                if (mutationTempList.FirstOrDefault(m => m.def == mutation.def) is not Mutation)
                {
                    mutation.OnRemoved(postLoad);
                }
            }

            _mutations = mutationTempList;
        }


		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Collections.Look(ref _mutations, "mutations", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                RefreshMutations(true);
            }
        }
	}
}
