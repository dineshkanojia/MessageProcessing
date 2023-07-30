using MessageProcessing.IServiceMessages;
using MessageProcessing.MessageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessing.ServiceMessages
{
    /// <summary>
    /// Adding quantity to the existing sales.
    /// </summary>
    public class SaleWithQuantity : ISaleMessage
    {
        public string ProductType { get; set; }
        public decimal Value { get; set; }
        public int Quantity { get; set; }

        public void Process(SalesProcessor salesProcessor)
        {
            for (int i = 0; i < Quantity; i++)
            {
                var sale = new Sale { ProductType = this.ProductType, Value = this.Value };
                salesProcessor.RecordSale(sale);    
            }
        }
    }
}
