using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HT.Common.ApiMessaging
{
    public class ApiLinks : List<ApiLink>
    {

        public ApiLinkBuilder Add()
        {
            var newLink = new ApiLink();
            base.Add(newLink);

            return new ApiLinkBuilder(newLink);
        }

        public ApiLinkBuilder Add(string location, string relation)
        {
            return Add().Location(location).Relation(relation);
        }
        public ApiLinkBuilder Add(string location, string relation,string id)
        {
            return Add().Location(location).Relation(relation).Id(id);
           
        }

        /// <summary>
        /// Inject fully qualified path into list of links and replace any embedded tokens
        /// </summary>
        /// <param name="serviceRootUrl">(optional) Fully quaified path to root of link path</param>
        /// <param name="clone">True=Create a copy of links leaving original intact, False-resolve urls and store back into Location, replacing any embedded tokens</param>
        /// <returns></returns>
        public ApiLinks Resolve(string serviceRootUrl=null,bool clone=false)
        {
            var linkList = new ApiLinks();
            if (clone == true)
            {
                for(int x=0;x<this.Count;x++)
                {
                    linkList.Add(this[x].Clone());
                }
            }
            else
            {
                linkList = this;
            }

            serviceRootUrl = serviceRootUrl ?? ApiSettings.Response.ServiceRoot;

            for (int x = 0; x < this.Count; x++)
            {
                linkList[x].Resolve(serviceRootUrl);
            }
            return linkList;
        }

        [OnSerializing]
        internal void OnSerializingLink(StreamingContext context)
        {
            if (ApiSettings.Response.AutoResolveLinks)
            {
                Resolve(null, false);
            }
        }

        public class ApiLinkBuilder
        {
            ApiLink _link;

            public ApiLinkBuilder(ApiLink link)
            {
                _link = link;
            }


            public ApiLinkBuilder Title(string title, params object[] args)
            {
                if (title == null)
                {
                    _link.Title = null;
                }
                else
                {
                    _link.Title = string.Format(title, args);
                }
                return this;
            }

            public ApiLinkBuilder Location(string location, params object[] args)
            {
                if (location == null)
                {
                    _link.Location = null;
                }
                else
                {
                    _link.Location = string.Format(location, args);
                }
                return this;
            }

            public ApiLinkBuilder Relation(string relation, params object[] args)
            {
                if (relation == null)
                {
                    _link.Relation = null;
                }
                else
                {
                    _link.Relation = string.Format(relation, args);
                }
                return this;
            }
            public ApiLinkBuilder Id(string id, params object[] args)
            {
                if (id == null)
                {
                    _link.Id = null;
                }
                else
                {
                    _link.Id = string.Format(id, args);
                }
                return this;
            }

            public ApiLink Link
            {
                get
                {
                    return _link;
                }
            }
        }

    }
}
