using MessageProcessing.Domain.Common;
using MessageProcessing.IServiceMessages;
using MessageProcessing.ServiceMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessing.MessageProcessor
{
    /// <summary>
    /// Processing message and log sales report.
    /// </summary>
    public class SalesProcessor
    {
        private List<Sale> sales = new List<Sale>();
        private List<Adjustment> adjustments = new List<Adjustment>();
        private int messageCount = 0;
        private const int ReportInterval = 10;
        private const int MaxMessage = 50;
        private ILogger logger;

        public SalesProcessor(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Processing messages for each product type.
        /// </summary>
        /// <param name="saleMessage"></param>
        public void ProcessingMessage(ISaleMessage saleMessage)
        {
            saleMessage.Process(this);
            messageCount++;

            //Log sales report after every 10 messages.
            if (messageCount % ReportInterval == 0)
            {
                GenerateSalesReport();
            }

            //Log adjustment sales report and pausing application after 50 messages.
            if (messageCount == MaxMessage)
            {
                GenerateAdjustmentReport();
                PauseProcessing();
            }
        }

        /// <summary>
        /// Creating and adding sales based on the product type.
        /// </summary>
        /// <param name="sale"></param>
        public void RecordSale(Sale sale)
        {
            sales.Add(sale);
        }


        /// <summary>
        /// Adjusting the sales based on the operations.
        /// </summary>
        /// <param name="adjustment"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ApplyAdjustment(Adjustment adjustment)
        {
            adjustments.Add(adjustment);

            //Adjusting sales based on operations.
            foreach (var sale in sales)
            {
                if (sale.ProductType == adjustment.ProductType)
                {
                    switch (adjustment.Operation)
                    {
                        case AdjustmentOperation.Add:
                            sale.Value += adjustment.Value;
                            break;
                        case AdjustmentOperation.Substract:
                            sale.Value -= adjustment.Value;
                            break;
                        case AdjustmentOperation.Multiply:
                            sale.Value *= adjustment.Value;
                            break;
                        default:
                            throw new ArgumentException("Invalid adjustment operation.", nameof(adjustment));
                    }
                }
            }
        }


        /// <summary>
        /// Generating sales report for every 10 messages.
        /// </summary>
        private void GenerateSalesReport()
        {
            logger.Log("Sales Report:");
            var salesByProduct = sales.GroupBy(s => s.ProductType);

            foreach (var group in salesByProduct)
            {
                int count = group.Count();
                decimal totalValue = group.Sum(s => s.Value);
                logger.Log($"{group.Key}: {count} sales, Total value: {totalValue} ");
            }

            logger.Log("---------------------------------------------");

        }

        /// <summary>
        /// Generating adjusted resport after 50 messages.
        /// </summary>
        private void GenerateAdjustmentReport()
        {
            logger.Log("Adjustment Report:");

            foreach (var adjustment in adjustments)
            {
                logger.Log($"{adjustment.ProductType}: {adjustment.Operation} {adjustment.Value}");
            }

            logger.Log("---------------------------------------------");
        }

        private void PauseProcessing()
        {
            logger.Log("Application is paused...");
        }
    }
}
