using System.Collections.Generic;
using System.ServiceModel;

namespace ProfitCenterService
{
    [ServiceContract]
    public interface IStockService
    {
        [OperationContract(IsOneWay = true)]
        void SendStock(Stock stock);
    }
}
