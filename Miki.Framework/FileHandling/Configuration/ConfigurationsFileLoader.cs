using Miki.Framework.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.FileHandling.Configuration
{
	public class ConfigurationsFileLoader
	{
		private string file = "config.json";
		private List<Module> modules = new List<Module>();

		private ConfigurationsFileLoader()
		{
		}

		public ConfigurationsFileLoader(string filename)
		{
			file = filename + ".json";
		}

		// TODO: finish this
		public async Task SaveConfigurationsToFile(List<Module> modules)
		{
		}
	}
}