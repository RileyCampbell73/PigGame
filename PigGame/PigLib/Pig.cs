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
        [OperationContract(IsOneWay = true)]
        void ShowMessage(string message);
        
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IPig
    {
        [OperationContract]
        int RegisterForCallbacks();
        [OperationContract(IsOneWay = true)]
        void UnregisterForCallbacks(int id);
        [OperationContract(IsOneWay = true)]
        void ClientReady( int id );
        [OperationContract(IsOneWay = true)]
        void ClientUnReady(int id);
        [OperationContract(IsOneWay = true)]
        void StartGame();
        [OperationContract]
        void Game();
        
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Pig : IPig
    {
        //dictionary for clients
        private Dictionary<int, ICallback> clientCallbacks = new Dictionary<int, ICallback>();
        private Dictionary<int, bool> readyList = new Dictionary<int, bool>(); // hold the client's key and if that client is ready
        private int nextCallbackId = 1;
        private bool startGame = false; // when all clients are ready, flip this
        //private CallBackInfo info;

        // C'tor
        public Pig()
        {
            Console.WriteLine("Creating a Pig!");
        }


        public int Roll()
        {
            var r = new Random();
            //shove everything to gui
            //need to populate an info object. Keep it global? make it when game is started!

            //foreach (ICallback cb in clientCallbacks.Values)
            //    cb.UpdateGui(info);
            return r.Next(1, 6);

        }

        public int RegisterForCallbacks()
        {
            // Store the ICallback interface (client object) reference for 
            // the client which is currently calling RegisterForCallbacks()
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            clientCallbacks.Add(nextCallbackId, cb);
            readyList.Add(nextCallbackId, false); // when they're first registered they won't be ready to start

            return nextCallbackId++;
        }

        public void UnregisterForCallbacks(int id)
        {
            clientCallbacks.Remove(id);
        }

        public void ClientReady( int id )
        {
            readyList[id] = true;
        }

        public void ClientUnReady( int id )
        {
            readyList[id] = false;
        }

        public void StartGame()
        {
            // make sure we have an appropriate amount of players
            if( clientCallbacks.Count >= 2 && clientCallbacks.Count <= 6 )
            {
                int numReady = 0;
                foreach ( bool flag in readyList.Values )
                {
                    if ( flag == true )
                        numReady++;
                }
                // we have an appropriate amount of players, all registered, and all ready to play
                if (numReady == clientCallbacks.Count)
                {
                    startGame = true;
                    SendMessage("Game Starting!");
                    //construct the new info object
                    //info = new CallBackInfo();
                    //call the game method

                }
            }
            else
            {
                // tell the clients that there are too many or too few players
                SendMessage("Only 2 to 6 players allowed!");
            }

        }

        // this allows us to send a message to one or all clients, if the id is 0 we'll show the message to everyone
        public void SendMessage( string message, int id = 0 )
        {
            foreach ( KeyValuePair<int,ICallback> kvp in clientCallbacks )
            {
                if (id == kvp.Key || id == 0)
                    kvp.Value.ShowMessage(message); // ShowMessage written in the client
            }
        }

        //Just an idea, Not even sure this will work
        public void Game()
        {

            //Stop more clients from connecting.
                //OR if game has started (the bool 'startGame') then disable the ready button in the client
                // maybe they will be all disabled by default anyway
                
                //pick a player to start first. or default it to player 1
                
                    //neverending loop here ( or until one of the players has 100 points)
            for (; ; )
            {
                //Note: Not sure how to make it wait for players
                //enable that players UI, disable the others.


                //Then player will roll dice until stuff happens (put in its own function maybe?)
                    //HOW DO WE WAIT FOR THE PLAYER?
                        //another loop and wait for a return code?
                //When they are done, pass it to next player, end loop
            }

                    //after loop, display winner
                    // end game, or prep for another


        }

    }
}
