using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADLite;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class ADException : Exception
    {
        public StateCodes? ResultCode
        {
            get;
            set;
        }

        public string ResultMessage
        {
            get;
            set;
        }

        public ADException(StateCodes? resultCode, string resultMessage)
        {
            ResultCode = resultCode;
            ResultMessage = resultMessage;
        }      
    }
}
