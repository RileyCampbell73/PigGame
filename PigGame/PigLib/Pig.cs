using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

namespace PigLib
{

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateGui(CallBackInfo info);
        
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IPig
    {
        [OperationContract]
        int RegisterForCallbacks();
        [OperationContract(IsOneWay = true)]
        void UnregisterForCallbacks(int id);
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Pig : IPig
    {
        //dictionary for clients
        private Dictionary<int, ICallback> clientCallbacks = new Dictionary<int, ICallback>();
        private int nextCallbackId = 1;

        // C'tor
        public Pig()
        {
            Console.WriteLine("Creating a Pig!");
        }

        public int RegisterForCallbacks()
        {
            // Store the ICallback interface (client object) reference for 
            // the client which is currently calling RegisterForCallbacks()
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            clientCallbacks.Add(nextCallbackId, cb);

            return nextCallbackId++;
        }

        public void UnregisterForCallbacks(int id)
        {
            clientCallbacks.Remove(id);
        }

    }
}
