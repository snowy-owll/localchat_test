using System.ServiceModel;

namespace ChatWCFService
{
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Message message);
    }
}
