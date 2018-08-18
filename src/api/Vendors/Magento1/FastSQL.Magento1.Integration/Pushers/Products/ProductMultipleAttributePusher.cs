using FastSQL.Magento1.Magento1Soap;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastSQL.Magento1.Integration.Pushers.Products
{
    public class ProductMultipleAttributePusher : BaseAttributePusher
    {
        private readonly SoapM1 soap;
        private List<string> HexaFields => new List<string> { "Id", "SourceId", "DestinationId", "State", "LastUpdated" };

        public ProductMultipleAttributePusher(ProductMultipleAttributePusherOptionManager optionManager,
            ProductProcessor entityProcessor, 
            MultipleAttributeProcessor attributeProcessor, 
            FastProvider provider,
            FastAdapter adapter,
            SoapM1 soap) : base(optionManager, entityProcessor, attributeProcessor, provider, adapter)
        {
            this.soap = soap;
        }

        public override PushState Create(out string destinationId)
        {
            destinationId = IndexedItem.GetDestinationId();

            return UpdateProduct(destinationId);
        }
        
        public override string GetDestinationId()
        {
            return IndexedItem.GetDestinationId();
        }

        public override PushState Remove(string destinationId = null)
        {
            destinationId = string.IsNullOrWhiteSpace(destinationId) ? IndexedItem.GetDestinationId() : destinationId;
            return UpdateProduct(destinationId);
        }

        public override PushState Update(string destinationId = null)
        {
            destinationId = string.IsNullOrWhiteSpace(destinationId) ? IndexedItem.GetDestinationId() : destinationId;
            return UpdateProduct(destinationId);
        }

        private PushState UpdateProduct(string destinationId)
        {
            var attrModel = (AttributeModel)GetIndexModel();
            var entityModel = (EntityModel)GetEntityModel();
            soap.SetOptions(Adapter.Options);

            try
            {
                var storeIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "store_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var data = LoadData();

                soap.Begin();
                var client = soap.GetClient();
                var sessionId = soap.GetSession();

                foreach (var storeId in storeIds)
                {
                    client.catalogProductUpdate(sessionId, destinationId, data, storeId, "id");
                }
                return PushState.Success;
            }
            finally
            {
                soap.End();
            }
        }

        private catalogProductCreateEntity LoadData()
        {
            var relatedIndexedItems = EntityRepository.GetIndexedItemsBySourceId(GetIndexModel(), IndexedItem.GetSourceId());
            var normalizedValues = relatedIndexedItems
                .Select(i => new { k = i.GetId(), v = GetNormalizedValuesByDependencies(i) })
                .ToDictionary(x => x.k, x => x.v);

            var attributes = IndexedItem.Properties().Where(p => !HexaFields.Contains(p.Name))
               .Select(a => new associativeMultiEntity
               {
                   key = a.Name,
                   value = normalizedValues.Select(v => {
                       var relatedIndexedItem = relatedIndexedItems.FirstOrDefault(r => r.GetId() == v.Key);
                       if (relatedIndexedItem.HasState(ItemState.Removed))
                       {
                           return null;
                       }
                       var normalizedValue = v.Value;

                       if (!normalizedValue.ContainsKey(a.Name))
                       {
                           return relatedIndexedItem.Value<string>(a.Name);
                       }
                       return normalizedValue[a.Name];
                   })
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .ToArray()
               })
               .ToArray();

            var result = new catalogProductCreateEntity
            {
                additional_attributes = new catalogProductAdditionalAttributesEntity
                {
                    multi_data = attributes
                }
            };

            return result;
        }

    }
}
