using System.Runtime.Serialization;

namespace ProfitCenterService
{
    [DataContract]
    public class Stock
    {

        [DataMember]
        public double Price;
    }
}
