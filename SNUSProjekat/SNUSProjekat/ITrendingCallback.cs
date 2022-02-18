using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    public interface ITrendingCallback
    {
        [OperationContract (IsOneWay = true)]
        void InputTagValueChanged(string tagName, double value);
    }
}
