using MessageProcessing.Domain.Common;
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
    /// Adjustment class to adjust sales based on the operation.
    /// </summary>
    public class Adjustment : ISaleMessage
    {
        public string ProductType { get; set; }
        public decimal Value { get; set; }
        public AdjustmentOperation Operation { get; set; }

        public void Process(SalesProcessor salesProcessor)
        {
            salesProcessor.ApplyAdjustment(this);
        }
    }
}
