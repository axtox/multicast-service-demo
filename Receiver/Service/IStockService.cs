using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Receiver.Service
{
    [ServiceContract]
    public interface IStockService
    {
        [OperationContract(IsOneWay = true)]
        Task SendStock(int stock);
    }
}
