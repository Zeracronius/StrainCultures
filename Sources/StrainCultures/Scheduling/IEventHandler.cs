using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrainCultures.Scheduling
{
	public interface IEventHandler
	{
		void HandleEvent(string? signal);
	}
}
