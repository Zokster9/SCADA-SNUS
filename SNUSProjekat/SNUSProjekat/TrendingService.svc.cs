using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SNUSProjekat
{
    public class TrendingService : ITrending
    {
        public void Init()
        {
            TagProcessing.InputTagValueChanged += OperationContext.Current
                .GetCallbackChannel<ITrendingCallback>().InputTagValueChanged;
        }
    }
}
