using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageProcessing.Domain.Common;
using MessageProcessing.IServiceMessages;
using MessageProcessing.ServiceMessages;

namespace MessageProcessing.MessageValidators
{
    public class ValidateMessages
    {
        string messages;
        private ILogger logger;
        public ValidateMessages(string messages, ILogger logger)
        {
            this.messages = messages;
            this.logger = logger;
        }

        /// <summary>
        /// Checking message is blank or null
        /// </summary>
        /// <param name="message">message type</param>
        /// <returns>True/False</returns>
        public bool IsValid(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return enum of message type based on messages
        /// </summary>
        /// <param name="message">message type</param>
        /// <returns>Message type enumerator</returns>
        public MessageType GetMessageType(string message)
        {
            if (message.ToLower().Contains("sales") && message.ToLower().Contains("each"))
            {
                return MessageType.Type2;
            }
            else if (message.ToLower().Contains("add") || message.ToLower().Contains("substract") || message.ToLower().Contains("multiply"))
            {
                return MessageType.Type3;
            }
            else
            {
                return MessageType.Type1;
            }
        }

        /// <summary>
        /// Extracting message details for processing sales.
        /// </summary>
        /// <param name="message">Message type</param>
        /// <returns>Sales object</returns>
        public Sale ExtractMessageType1Details(string message)
        {
            var sale = new Sale();
            try
            {
                if (!IsValid(message))
                {
                    throw new Exception("Message type 1 cannot be blank.");
                }

                string[] part = message.ToLower().Split(new[] { " at " }, StringSplitOptions.RemoveEmptyEntries);
                if (part.Count() > 1)
                {
                    sale.ProductType = part[0].ToLower();
                    string priceStr = part[1].ToLower().Contains("p") ? part[1].TrimEnd('p') : part[1];

                    if (decimal.Parse(priceStr) < 0)
                    {
                        throw new Exception("Message type 1: Price cannot be negative");
                    }
                    sale.Value = decimal.Parse(priceStr);
                }
                else
                    throw new Exception("Message type 1 is invalid.");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }
            return sale;
        }

        /// <summary>
        /// Extracting message details for processing sales with quantity.
        /// </summary>
        /// <param name="message">Message Type</param>
        /// <returns>Sale With Quantity object </returns>
        public SaleWithQuantity ExtractMessageType2Details(string message)
        {
            var saleWithQuantity = new SaleWithQuantity();
            try
            {
                if (!IsValid(message))
                {
                    throw new Exception("Message type 2 cannot be blank.");
                }

                string[] part = message.Split(new[] { " sales of ", " at ", " each" }, StringSplitOptions.RemoveEmptyEntries);
                if (part.Count() > 2)
                {
                    saleWithQuantity.Quantity = int.Parse(part[0]);
                    saleWithQuantity.ProductType = part[1].ToLower().TrimEnd('s');
                    string priceStr = part[2].ToLower().Contains("p") ? part[2].TrimEnd('p') : part[2];

                    if (decimal.Parse(priceStr) < 0)
                    {
                        throw new Exception("Message type 2: Price cannot be negative");
                    }
                    saleWithQuantity.Value = decimal.Parse(priceStr);
                }
                else
                    throw new Exception("Message type 2 is invalid.");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }

            return saleWithQuantity;
        }

        /// <summary>
        /// Extracting message details for processing sales with quantity.
        /// </summary>
        /// <param name="message">Message Type</param>
        /// <returns>Adjustmen object</returns>
        public Adjustment ExtractMessageType3Details(string message)
        {
            var adjustment = new Adjustment();
            try
            {
                if (!IsValid(message))
                {
                    throw new Exception("Message type 3 cannot be blank.");
                }
                string[] parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Count() > 2)
                {
                    adjustment.Operation = (AdjustmentOperation)Enum.Parse(typeof(AdjustmentOperation), parts[0],true);
                    string priceStr = parts[1].ToLower().Contains("p") ? parts[1].TrimEnd('p') : parts[1];
                    if (decimal.Parse(priceStr) < 0)
                    {
                        throw new Exception("Message type 3: Price cannot be negative");
                    }
                    adjustment.Value = decimal.Parse(priceStr);
                    adjustment.ProductType = parts[2].ToLower().TrimEnd('s');
                }
                else
                    throw new Exception("Message type 3 is invalid.");
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
            }

            return adjustment;
        }
    }
}
