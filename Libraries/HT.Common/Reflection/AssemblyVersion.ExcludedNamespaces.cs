using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HT.Common.Reflection
{
    public partial class AssemblyVersion
    {
        /// <summary>
        /// These namespace(s) will not be included in generated stack traces.  This helps clean up logging and reporting.  During a stack
        /// trace dump, as the stack is unwound it will not report any namespaces until reaching a namespace NOT included here.  After
        /// that, then all namespaces are included in the stack dump. Excluded namespaces='System','Microsoft','MacroPoint.Common'
        /// </summary>
        private static string[] _excludedNamespaces = new string[] { "System", "Microsoft", "MacroPoint.Common", getRootNamespace() }; //EXTENSION
    }
}