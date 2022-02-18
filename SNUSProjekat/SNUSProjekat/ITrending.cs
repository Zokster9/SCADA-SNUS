using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SNUSProjekat
{
    [ServiceContract (CallbackContract = typeof(ITrendingCallback))]
    public interface ITrending
    {
        [OperationContract]
        void Init();
    }
}
