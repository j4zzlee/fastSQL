using FastSQL.Magento1.Magento1Soap;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FastSQL.Magento1.Integration.Pushers
{
    public class ProductPusher : BaseEntityPusher
    {
        private readonly SoapM1 soap;
        private List<string> HexaFields => new List<string> { "Id", "SourceId", "DestinationId", "State", "LastUpdated" };
        private List<string> ProductFields => new List<string> { "type", "sku", "attribute_set_id" };
        //private Dictionary<string, string> _normalizedValues = new Dictionary<string, string>();

        public ProductPusher(ProductPusherOptionManager optionManager,
            ProductProcessor processor,
            FastAdapter adapter,
            SoapM1 soap) : base(optionManager, processor, adapter)
        {
            this.soap = soap;
        }

        public override PushState Create(out string destinationId)
        {
            var pushState = PushState.Success;
            soap.SetOptions(Adapter.Options);
            var indexedModel = GetIndexModel();
            var additionalFields = IndexedItem.Properties().Where(p => !HexaFields.Contains(p.Name) && !ProductFields.Contains(p.Name)).Select(p => p.Name);
            var websiteIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "website_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var storeIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "store_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var normalizedValues = GetNormalizedValuesByDependencies();
            try
            {
                var additionalAttrs = additionalFields.Select(f => new associativeEntity
                {
                    key = f,
                    value = normalizedValues.ContainsKey(f) ? normalizedValues[f] : IndexedItem.Value<string>(f)
                });
               
                var attributes = new catalogProductAdditionalAttributesEntity { single_data = additionalAttrs.ToArray() };
                var data = new catalogProductCreateEntity
                {
                    website_ids = websiteIds,
                    additional_attributes = attributes,
                    status = "2",
                    visibility = "4",
                    stock_data = new catalogInventoryStockItemUpdateEntity()
                    {
                        qty = "0",
                        is_in_stock = 1,
                    }
                };
                
                soap.Begin();
                var client = soap.GetClient();
                destinationId = client
                   .catalogProductCreate(soap.GetSession(),
                       normalizedValues.ContainsKey("type") ? normalizedValues["type"] : IndexedItem.Value<string>("type"),
                       normalizedValues.ContainsKey("attribute_set_id") ? normalizedValues["attribute_set_id"] : IndexedItem.Value<string>("attribute_set_id"),
                       IndexedItem.Value<string>("sku"),
                       data,
                       "0").ToString();
                foreach (var storeId in storeIds)
                {
                    client.catalogProductMultiUpdateAsync(
                        soap.GetSession(), 
                        new string[] { destinationId },
                        new catalogProductCreateEntity[]
                        {
                            new catalogProductCreateEntity
                            {
                                website_ids = websiteIds,
                                status = "2" // Creating product always set status to false. itemModel.removed == DIndex.ConstYes ? "2" : "1"
                            }
                        },
                        storeId,
                        "id");
                }

                return pushState;

            }
            finally
            {
                soap.End();
            }
        }

        public override string GetDestinationId()
        {
            soap.SetOptions(Adapter.Options);
            try
            {
                soap.Begin();
                var client = soap.GetClient();
                var product = client.catalogProductInfo(
                    soap.GetSession(), 
                    IndexedItem.Value<string>("sku"),
                    "0" /* Store 0 always contains all products */, 
                    null, 
                    "sku");
                return product.product_id;
            }
            finally
            {
                soap.End();
            }
        }

        public override PushState Remove(string destinationId = null)
        {
            var destId = !string.IsNullOrWhiteSpace(destinationId) ? destinationId : IndexedItem.GetDestinationId();
            var pushState = PushState.Success;
            soap.SetOptions(Adapter.Options);
            try
            {
                var websiteIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "website_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var storeIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "store_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                soap.Begin();
                var client = soap.GetClient();
                foreach (var storeId in storeIds)
                {
                    client.catalogProductMultiUpdateAsync(soap.GetSession(), new[] { destId },
                        new[]
                        {
                            new catalogProductCreateEntity
                            {
                                website_ids = websiteIds,
                                status = "2"
                            }
                        },
                        storeId,
                        "id");
                }

            }
            finally
            {
                soap.End();
            }
            return pushState;

        }

        public override PushState Update(string destinationId = null)
        {
            var destId = !string.IsNullOrWhiteSpace(destinationId) ? destinationId : IndexedItem.GetDestinationId();
            var pushState = PushState.Success;
            soap.SetOptions(Adapter.Options);
            try
            {
                var websiteIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "website_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var storeIds = Regex.Split(Options.FirstOrDefault(o => o.Name == "store_ids").Value, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                soap.Begin();
                var client = soap.GetClient();
                foreach (var storeId in storeIds)
                {
                    client.catalogProductMultiUpdateAsync(soap.GetSession(), new[] { destId },
                        new[]
                        {
                            new catalogProductCreateEntity
                            {
                                website_ids = websiteIds,
                                status = "1"
                            }
                        },
                        storeId,
                        "id");
                }
            }
            finally
            {
                soap.End();
            }
            return pushState;
        }
    }
}
