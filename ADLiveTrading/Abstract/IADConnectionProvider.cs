using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Abstract
{
    public interface IADConnectionProvider
    {
        bool Connected
        {
            get;
        }
    }
}
