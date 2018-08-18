using FastSQL.Magento1.Magento1Soap;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastSQL.Magento1.Integration.Pushers.Products
{
    public class ProductSingleAttributePusher : BaseAttributePusher
    {
        private readonly SoapM1 soap;
        private List<string> HexaFields => new List<string> { "Id", "SourceId", "DestinationId", "State", "LastUpdated" };

        public ProductSingleAttributePusher(
            ProductSingleAttributePusherOptionManager optionManager,
            ProductProcessor entityProcessor, 
            SingleAttributeProcessor attributeProcessor,
            FastProvider provider,
            FastAdapter adapter,
            SoapM1 soap) : base(optionManager, entityProcessor, attributeProcessor, provider, adapter)
        {
            this.soap = soap;
        }

        public override PushState Create(out string destinationId)
        {
            destinationId = IndexedItem.Value<string>("DestinationId");
            return Update();
        }

        public override string GetDestinationId()
        {
            return IndexedItem.Value<string>("DestinationId");
        }

        public override PushState Remove(string destinationId = null)
        {
            return PushState.Success;
        }

        public override PushState Update(string destinationId = null)
        {
            return Update();
        }

        private PushState Update()
        {
            var result = PushState.Success;
            var destinationId = IndexedItem.Value<string>("DestinationId");
            soap.SetOptions(Adapter.Options);
            try
            {
                var data = LoadEntity();
                var storeIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "store_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                soap.Begin();
                var client = soap.GetClient();
                foreach (var storeId in storeIds)
                {
                    var success = client.catalogProductUpdate(soap.GetSession(), destinationId, data, storeId, "id");
                }
                return result;
            }
            finally
            {
                soap.End();
            }
        }

        private catalogProductCreateEntity LoadEntity()
        {
            var normalizedValues = GetNormalizedValuesByDependencies();
            var attributes = IndexedItem.Properties().Where(p => !HexaFields.Contains(p.Name))
                .Select(a => new associativeEntity
                {
                    key = a.Name,
                    value = normalizedValues.ContainsKey(a.Name) ? normalizedValues[a.Name] : IndexedItem.Value<string>(a.Name)
                })
                .ToArray();
            
            var result = new catalogProductCreateEntity
            {
                additional_attributes = new catalogProductAdditionalAttributesEntity
                {
                    single_data = attributes
                },
            };
            return result;
        }
    }
}
