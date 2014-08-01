using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.Abstract
{
    public interface IADStreamingProvider : IADConnectionProvider
    {
        event Action<string> NewQuote;

        void StreamDataSubscribe(object symbols);
        void StreamDataUnsubscribe();
    }
}
