using System.Collections.Generic;
using System.ServiceModel;

namespace ProfitCenterService
{
    [ServiceContract]
    public interface IStockService
    {
        [OperationContract(IsOneWay = true)]
        void SendStockDetail(Stock stock);
    }
}
