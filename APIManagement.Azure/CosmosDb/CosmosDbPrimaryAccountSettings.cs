using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIManagement.Azure.CosmosDb
{
    public class CosmosDbPrimaryAccountSettings : CosmosDbSettings
    {
        public override string PrefferedLocation { get => LocationNames.EastUS; }
    }
}
