

using System;
using System.Collections.Generic;
using System.Text;

namespace HT.Common.Documentation
{
    /// <summary>
    /// Marks codes that is a unqiue extension of code that might be attributable to another source
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class CitationExclusionAttribute : Attribute
    {
    }
}