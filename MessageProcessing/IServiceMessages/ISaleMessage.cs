using MessageProcessing.MessageProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessing.IServiceMessages
{
    /// <summary>
    /// Declate Sales Message interface.
    /// </summary>
    public interface ISaleMessage
    {
        void Process(SalesProcessor processor);
    }
}
