using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace StrainCultures.Scheduling
{
	public interface IEventHandler : ILoadReferenceable
	{
		void HandleEvent(string? signal);
	}
}
