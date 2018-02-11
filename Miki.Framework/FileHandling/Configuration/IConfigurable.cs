using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.FileHandling.Configuration
{
    interface IConfigurable
    {
        ConfigurationBase Configuration { get; set; }
    }
}
