using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.RenderNode
{
	internal class MutationRenderNodeWorker : PawnRenderNodeWorker
	{
		public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
		{
			base.AppendDrawRequests(node, parms, requests);

		}
	}
}
