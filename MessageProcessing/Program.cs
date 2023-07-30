// See https://aka.ms/new-console-template for more information
using MessageProcessing.Domain.Common;
using MessageProcessing.IServiceMessages;
using MessageProcessing.MessageProcessor;
using MessageProcessing.MessageValidators;
using MessageProcessing.ServiceMessages;
using System.Collections.Generic;

var logger = new ConsoleLogger();
var processor = new SalesProcessor(logger);
var salesMessages = new List<ISaleMessage>();


// list of message types.
string[] strMessage = {
            "apple at 10p",
            "20 sales of apples at 10p each",
            "Add 20p apples",
};

try
{
    foreach (var message in strMessage)
    {
        //Validate each message with its types
        ValidateMessages validateMessages = new ValidateMessages(message, logger);
        MessageType MessageType = validateMessages.GetMessageType(message);

        //Add in validated message for processing.
        switch (MessageType)
        {
            case MessageType.Type1:
                salesMessages.Add(validateMessages.ExtractMessageType1Details(message));
                break;
            case MessageType.Type2:
                salesMessages.Add(validateMessages.ExtractMessageType2Details(message));
                break;
            case MessageType.Type3:
                salesMessages.Add(validateMessages.ExtractMessageType3Details(message));
                break;
        }
    }


    //Processing each and every messages as per product type.
    foreach (var saleMessage in salesMessages)
    {
        processor.ProcessingMessage(saleMessage);
    }

}
catch (Exception ex)
{
    logger.Log(ex.Message);
}

Console.ReadLine();