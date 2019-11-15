using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace opplatApplication.Utils
{
    public class ModuleInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Assembly Assembly { get; set; }

        public bool IsBundledWithHost { get; set; }

        public Version Version { get; set; }

        public string ShortName
        {
            get
            {
                return Name.Split('.').Last();
            }
        }

        public string Path { get; set; }
    }
}
