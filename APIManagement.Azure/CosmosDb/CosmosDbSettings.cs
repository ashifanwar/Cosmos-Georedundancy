using System;
using System.Collections.Generic;
using System.Text;

namespace APIManagement.Azure.CosmosDb
{
    public abstract class CosmosDbSettings
    {
        public string EndpointUrl { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public string AuthKey { get; set; }

        public abstract string PrefferedLocation { get; }
    }
}
