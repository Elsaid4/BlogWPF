using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OSKHelpers.Common
{
    public class Paths
    {
        private static readonly string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string AssemblyPath => _assemblyPath;
    }
}
