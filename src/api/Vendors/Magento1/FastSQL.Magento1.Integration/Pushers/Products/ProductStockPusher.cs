using FastSQL.Magento1.Magento1Soap;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Pusher;
using System.Linq;

namespace FastSQL.Magento1.Integration.Pushers.Products
{
    public class ProductStockPusher : BaseAttributePusher
    {
        private readonly SoapM1 soap;

        public ProductStockPusher(ProductStockPusherOptionManager optionManager, 
            ProductProcessor entityProcessor, 
            StockAttributeProcessor attributeProcessor,
            FastAdapter adapter,
            SoapM1 soap) : base(optionManager, entityProcessor, attributeProcessor, adapter)
        {
            this.soap = soap;
        }

        public override PushState Create(out string destinationId)
        {
            destinationId = IndexedItem.GetDestinationId();
            return UpdateStock(IndexedItem.GetDestinationId());
        }

        private PushState UpdateStock(string destinationId)
        {
            var attrModel = (AttributeModel)GetIndexModel();
            var entityModel = (EntityModel)GetEntityModel();
            soap.SetOptions(Adapter.Options);
            try
            {
                var updateData = Load();

                soap.Begin();
                var client = soap.GetClient();
                var sessionId = soap.GetSession();

                client.catalogInventoryStockItemUpdate(sessionId, destinationId, updateData.stock_data);
                return PushState.Success;
            }
            finally
            {
                soap.End();
            }
        }

        private catalogProductCreateEntity Load()
        {
            var qty = IndexedItem.Value<double>("stock");
            var inStock = qty > 0 ? 1 : 0;
            var manageStock = Options.FirstOrDefault(o => o.Name == "manage_stock").Value == bool.TrueString ? 1 : 0;
            var useQtyDecimal = Options.FirstOrDefault(o => o.Name == "decimal").Value == bool.TrueString ? 1 : 0;
            return new catalogProductCreateEntity
            {
                stock_data = new catalogInventoryStockItemUpdateEntity()
                {
                    qty = IndexedItem.Value<string>("stock"),
                    is_in_stock = inStock,
                    manage_stock = manageStock,
                    is_in_stockSpecified = true,
                    is_qty_decimal = useQtyDecimal,
                    is_qty_decimalSpecified = true,
                }
            };
        }

        public override string GetDestinationId()
        {
            return IndexedItem.GetDestinationId();
        }

        public override PushState Remove(string destinationId = null)
        {
            return PushState.Success;
        }

        public override PushState Update(string destinationId = null)
        {
            destinationId = !string.IsNullOrWhiteSpace(destinationId) ? destinationId : IndexedItem.GetDestinationId();
            return UpdateStock(destinationId);
        }
    }
}
