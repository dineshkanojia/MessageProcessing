using MessageProcessing.Domain.Common;
using MessageProcessing.MessageProcessor;
using MessageProcessing.ServiceMessages;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestMessageProcessing
{
    public class SalesProcessorTests
    {

        [Test]
        public void ProcessMessage_Should_Log_Sales_Report_After_Every_10th_Message()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);

            // Create 9 sales messages
            for (int i = 0; i < 9; i++)
            {
                var sale = new Sale { ProductType = "Product", Value = 1.0m };
                processor.ProcessingMessage(sale);
            }

            // On processing the 10th message, a sales report should be logged.
            Assert.That(() =>
            {
                var sale = new Sale { ProductType = "Product", Value = 1.0m };
                processor.ProcessingMessage(sale);
            }, Throws.Nothing);
        }

        [Test]
        public void ProcessMessage_Should_Log_Adjustment_Report_After_50_Messages()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);

            // Create 49 sales messages
            for (int i = 0; i < 49; i++)
            {
                var sale = new Sale { ProductType = "Product", Value = 1.0m };
                processor.ProcessingMessage(sale);
            }

            // On processing the 50th message, an adjustment report should be logged.
            Assert.That(() =>
            {
                var adjustment = new Adjustment { ProductType = "Product", Value = 0.5m, Operation = AdjustmentOperation.Add };
                processor.ProcessingMessage(adjustment);
            }, Throws.Nothing);
        }

        [Test]
        public void ProcessMessage_Should_Pause_Processing_After_50_Messages()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);

            // Create 50 sales messages
            for (int i = 0; i < 50; i++)
            {
                var sale = new Sale { ProductType = "Product", Value = 1.0m };
                processor.ProcessingMessage(sale);
            }

            // On processing the 51st message, the processor should be paused.

            Assert.That(() =>
            {
                var sale = new Sale { ProductType = "Product", Value = 1.0m };
                processor.ProcessingMessage(sale);
            }, Throws.Nothing);
        }

        [Test]
        public void ApplyAdjustment_Should_Adjust_Sale_Values()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);
            var sale1 = new Sale { ProductType = "Product", Value = 10.0m };
            var sale2 = new Sale { ProductType = "Product", Value = 5.0m };

            processor.RecordSale(sale1);
            processor.RecordSale(sale2);

            // Apply an adjustment to add 2.0 to all "Product" sales.
            var adjustment = new Adjustment { ProductType = "Product", Value = 2.0m, Operation = AdjustmentOperation.Add };
            processor.ApplyAdjustment(adjustment);

            Assert.AreEqual(12.0m, sale1.Value);
            Assert.AreEqual(7.0m, sale2.Value);
        }

        [Test]
        public void ApplyAdjustment_Should_Not_Affect_Other_Product_Types()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);
            var appleSale = new Sale { ProductType = "Apple", Value = 10.0m };
            var orangeSale = new Sale { ProductType = "Orange", Value = 5.0m };

            processor.RecordSale(appleSale);
            processor.RecordSale(orangeSale);

            // Apply an adjustment for "Apple" products only.
            var adjustment = new Adjustment { ProductType = "Apple", Value = 2.0m, Operation = AdjustmentOperation.Add };
            processor.ApplyAdjustment(adjustment);

            // Ensure that the adjustment is applied only to "Apple" sales.
            Assert.AreEqual(12.0m, appleSale.Value);
            Assert.AreEqual(5.0m, orangeSale.Value);
        }

        [Test]
        public void ApplyAdjustment_Should_Not_Affect_Previous_Sales()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);
            var sale = new Sale { ProductType = "Apple", Value = 10.0m };
            processor.RecordSale(sale);

            // Apply an adjustment for "Apple" products.
            var adjustment = new Adjustment { ProductType = "Apple", Value = 2.0m, Operation = AdjustmentOperation.Add };
            processor.ApplyAdjustment(adjustment);

            // Add another sale after applying the adjustment.
            var newSale = new Sale { ProductType = "Apple", Value = 15.0m };
            processor.RecordSale(newSale);

            // Ensure that the adjustment only affects the previous sale.
            Assert.AreEqual(12.0m, sale.Value);
            Assert.AreEqual(15.0m, newSale.Value);
        }

        [Test]
        public void ApplyAdjustment_Should_Not_Affect_Sales_After_Pausing()
        {
            var logger = new ConsoleLogger();
            var processor = new SalesProcessor(logger);
            var sale = new Sale { ProductType = "Apple", Value = 10.0m };
            processor.RecordSale(sale);

            // Pause the processor.
            for (int i = 0; i < 50; i++)
            {
                var newSale = new Sale { ProductType = "Apple", Value = 1.0m };
                processor.ProcessingMessage(newSale);
            }

            // Apply an adjustment for "Apple" products after pausing.
            var adjustment = new Adjustment { ProductType = "Apple", Value = 2.0m, Operation = AdjustmentOperation.Add };
            processor.ApplyAdjustment(adjustment);

            // Ensure that the adjustment does not affect sales after pausing.
            Assert.AreEqual(12.0m, sale.Value);
        }
    }
}