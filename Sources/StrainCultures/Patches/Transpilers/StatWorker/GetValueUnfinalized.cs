using HarmonyLib;
using StrainCultures.Attributes;
using StrainCultures.Hediffs;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Patches.Transpilers.StatWorker
{
	[HotSwappable]
	[HarmonyLib.HarmonyPatch(typeof(RimWorld.StatWorker))]
	internal class GetValueUnfinalized
	{
		[HarmonyPatch("GetValueUnfinalized"), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			MethodInfo hediffCurStage = AccessTools.PropertyGetter(typeof(Hediff), nameof(Hediff.CurStage));

			Queue<LambdaExpression> stack = new Queue<LambdaExpression>();
			stack.Enqueue(() => GetMutatedHediffOffset);
			stack.Enqueue(() => GetMutatedHediffFactor);

			Queue<CodeInstruction> instruction = new Queue<CodeInstruction>();
			instruction.Enqueue(new(OpCodes.Add));
			instruction.Enqueue(new(OpCodes.Mul));

			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				CodeInstruction current = codes[i];

				if (codes[i].Is(OpCodes.Callvirt, hediffCurStage))
				{
					// Sanity checks:
					// Not end of code
					if (i + 1 >= codes.Count)
						continue;

					// Next instruction is is a store. (Should be skipped.
					CodeInstruction next = codes[i + 1];
					if (next.opcode != OpCodes.Stloc_S)
						continue;

					// Previous instruction is getting hediff. (Should be copied)
					CodeInstruction previousGetHediff = codes[i - 1];
					if (previousGetHediff.opcode != OpCodes.Callvirt)
						continue;

					CodeInstruction previousGetIndex = codes[i - 2];
					CodeInstruction previousGetcollection = codes[i - 3];

					var newInstructions = new List<CodeInstruction>
					{
						CodeInstruction.LoadLocal(0),
						new(previousGetcollection),
						new(previousGetIndex),
						new(previousGetHediff),
						CodeInstruction.LoadArgument(0), // Load this
						CodeInstruction.LoadField(typeof(RimWorld.StatWorker), "stat"), // Load stat
						CodeInstruction.Call(stack.Dequeue()), // Call GetMutatedHediffOffset or GetMutatedHediffFactor
						new(instruction.Dequeue()), // Add to value
						CodeInstruction.StoreLocal(0)
					};
					codes.InsertRange(i-1, newInstructions);
					i += newInstructions.Count;
				}
			}
			return codes.AsEnumerable();
		}

		private static float GetMutatedHediffOffset(Hediff hediff, RimWorld.StatDef stat)
		{
			if (hediff is Mutated mutatedHediff)
			{
				return mutatedHediff.GetStatOffset(stat);
			}
			return 0;
		}

		public static float GetMutatedHediffFactor(Hediff hediff, RimWorld.StatDef stat)
		{
			if (hediff is Mutated mutatedHediff)
			{
				return mutatedHediff.GetStatFactor(stat);
			}
			return 1;
		}
	}
}
