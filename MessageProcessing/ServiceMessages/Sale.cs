using MessageProcessing.IServiceMessages;
using MessageProcessing.MessageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessing.ServiceMessages
{
    /// <summary>
    /// Creating sales based on product type.
    /// </summary>
    public class Sale : ISaleMessage
    {
        public string ProductType { get; set; }
        public decimal Value { get; set; }

        public void Process(SalesProcessor processor)
        {
            processor.RecordSale(this);
        }
    }
}

