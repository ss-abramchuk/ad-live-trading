using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class InvokeDescription
    {
        public InvokeType Type
        {
            get;
            set;
        }

        public IAsyncResult AsyncResult
        {
            get;
            set;
        }

        public AsyncCallback CallBack
        {
            get;
            set;
        }

        public object Query
        {
            get;
            set;
        }
    }
}
