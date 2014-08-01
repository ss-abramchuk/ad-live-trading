using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class AsyncResult : IAsyncResult
    {
        public object AsyncState
        {
            get;
            set;
        }

        public WaitHandle AsyncWaitHandle
        {
            get;
            set;
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get;
            set;
        }

        ~AsyncResult()
        {
            if (AsyncWaitHandle != null)
                AsyncWaitHandle.Close();
        }
    }
}
