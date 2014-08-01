using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADLite;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class InvokeResult
    {
        public StateCodes? State
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public InvokeType? Type
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public InvokeResult()
        {
            State = null;
            Message = null;
            Type = null;
            Value = string.Empty;
        }
    }
}
