using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.ViewModels
{
    public class EntityTemplateOptionRequestViewModel
    {
        public string SourceProcessorId { get; set; }
        public string DestinationProcessorId { get; set; }
        public string SourceConnectionId { get; set; }
        public string DestinationConnectionId { get; set; }
    }

    public class AttributeTemplateOptionRequestViewModel
    {
        public string EntityId { get; set; }
        public string SourceProcessorId { get; set; }
        public string DestinationProcessorId { get; set; }
        public string SourceConnectionId { get; set; }
        public string DestinationConnectionId { get; set; }
    }
}
