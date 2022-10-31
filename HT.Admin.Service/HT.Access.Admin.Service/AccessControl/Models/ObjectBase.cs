using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Access.Admin.Service.AccessControl.Models
{
    public class ObjectBase
    {
        /// <summary>
        /// Unique identifier of this object suitable for sending to insecure clients (DN name)
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Short human readable name of this object (Test Domain)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Program reference for this object ('test')
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Longer description of this object used for UI purposes
        /// </summary>
        public string Description { get; set; }

    }
}
