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
        void UpdateGui( CallBackInfo info);
        [OperationContract(IsOneWay = true)]
        void ShowMessage(string message);
        [OperationContract(IsOneWay = true)]
        void ChangeUI( bool b );
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
        [OperationContract(IsOneWay = true)]
        void Roll(int clientId);
        
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Pig : IPig
    {
        //dictionary for clients
        private Dictionary<int, ICallback> clientCallbacks = new Dictionary<int, ICallback>();
        //private Dictionary<int, bool> readyList = new Dictionary<int, bool>(); // hold the client's key and if that client is ready
        private int nextCallbackId = 1;
        private bool startGame = false; // when all clients are ready, flip this
        //private CallBackInfo info = new CallBackInfo();
        private Dictionary<int, CallBackInfo> clientData = new Dictionary<int, CallBackInfo>();

        

        // C'tor
        public Pig()
        {
            Console.WriteLine("Creating a Pig!");
        }


        public void Roll(int clientId)
        {
            var r = new Random();
            //shove everything to gui
            //need to populate an info object. Keep it global? make it when game is started!
            int roll = r.Next(1, 6);

            if (roll == 1)
            {
                clientData[clientId].DieRoll = roll;
                clientData[clientId].BankedPoints = 0;

                int x = 1;
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(clientData[x]);
                    x++;
                }

                //change player turns
                Game();
            }
            else
            {

                clientData[clientId].DieRoll = roll;
                clientData[clientId].BankedPoints += roll;

                int x = 1;
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(clientData[x]);
                    x++;
                }
            }
           // return r.Next(1, 6);

        }

        public int RegisterForCallbacks()
        {
            if (!startGame)
            {
                // Store the ICallback interface (client object) reference for 
                // the client which is currently calling RegisterForCallbacks()
                ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
                clientCallbacks.Add(nextCallbackId, cb);
                CallBackInfo info = new CallBackInfo();
                clientData.Add(nextCallbackId, info);
                //readyList.Add(nextCallbackId, false); // when they're first registered they won't be ready to start

                return nextCallbackId++;
            }
            else
                return 0;
        }

        public void UnregisterForCallbacks(int id)
        {
            clientCallbacks.Remove(id);
            clientData.Remove(id);
        }

        public void ClientReady( int id )
        {
            if (id != 0) // make sure this player has a valid ID, they shouldn't be added if they joined after the game started
                clientData[id].Ready = true;
            else
                SendMessage("Game has already started!");
        }

        public void ClientUnReady( int id )
        {
            if (id != 0)
                clientData[id].Ready = false;
        }

        public void StartGame()
        {
            // make sure we have an appropriate amount of players
            if( clientCallbacks.Count >= 2 && clientCallbacks.Count <= 6 )
            {
                int numReady = 0;
                foreach ( KeyValuePair<int,CallBackInfo> kvp in clientData )
                {
                    if ( kvp.Value.Ready == true )
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
                    Game();
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

        // enables a player's UI, disables everyone elses
        public void EnablePlayerUI( int id )
        {
            foreach (KeyValuePair<int, ICallback> kvp in clientCallbacks)
            {
                if (id == kvp.Key)
                    kvp.Value.ChangeUI(true);
                else
                    kvp.Value.ChangeUI(false);
            }
        }
        int playerId = 0;
        //Just an idea, Not even sure this will work
        public void Game()
        {
            playerId++;
            if (playerId > clientData.Count)
            {
                playerId = 1;
            }
            //Maybe have each player roll to find out who goes first, but for now just choose player 1
            //int playerId = 1; // we'll start with player 1 for now
            bool gameOn = false;
            //neverending loop here ( or until one of the players has 100 points)
                //Note: Not sure how to make it wait for players
                //enable that players UI, disable the others.
            EnablePlayerUI(playerId);

                //Then player will roll dice until stuff happens (put in its own function maybe?)
                    //HOW DO WE WAIT FOR THE PLAYER?
                        //another loop and wait for a return code?
                //When they are done, pass it to next player, end loop

                    //after loop, display winner
                    // end game, or prep for another


        }

    }
}
