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
        void UpdateGui( CallBackInfo info, int id );
        [OperationContract(IsOneWay = true)]
        void ShowMessage(string message);
        [OperationContract(IsOneWay = true)]
        void ChangeUI( bool b );
        [OperationContract(IsOneWay = true)]
        void ResetUI();
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
        [OperationContract(IsOneWay = true)]
        void Stay(int clientId);
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

        public void Stay(int clientId)
        {
            clientData[clientId].TotalPoints += clientData[clientId].BankedPoints;
            clientData[clientId].BankedPoints = 0;

            bool gameEnd=false;
            int winner = 0;
            int x = 1;

            CallBackInfo temp = new CallBackInfo();
            temp.DieRoll = clientData[clientId].DieRoll;
            temp.BankedPoints = 0;
            temp.TotalPoints = clientData[clientId].TotalPoints;

            foreach (ICallback cb in clientCallbacks.Values)
            {
                if (clientData[x].TotalPoints > 10)
                {
                    gameEnd = true;
                    winner = x;
                }
                cb.UpdateGui(temp, clientId);
                x++;
            }

            if (gameEnd == false)
                //change player turns
                Game();
            else
            {
                GameEnd(winner);
            }
        }

        private void GameEnd(int player){
            SendMessage("Player " + player + " is the winner!");

            //this loop resets all the players data for a new game
            foreach (CallBackInfo stuff in clientData.Values)
            {
                stuff.BankedPoints = 0;
                stuff.Ready = false;
                stuff.TotalPoints = 0;
                stuff.DieRoll = 0;
            }
            //this loop resets all the ui elements
            foreach (ICallback cb in clientCallbacks.Values)
            {
                cb.ResetUI();
            }
        }

        public void Roll(int clientId)
        {
            //for making random numbers
            var r = new Random();
            //put a random number between 1 and 6 into this int.
            int roll = r.Next(1, 6);

            //check to see if the roll is a 1
            if (roll == 1)
            {
                //put data in clientData to be sent to client
                clientData[clientId].DieRoll = roll;
                //sets banked point to 0 because game rules =(
                clientData[clientId].BankedPoints = 0;

                CallBackInfo temp = new CallBackInfo();
                temp.DieRoll = roll;
                temp.BankedPoints = 0;
                temp.TotalPoints = clientData[clientId].TotalPoints;

                int x = 1;
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(temp, clientId);
                    x++;
                }

                //change player turns
                Game();
            }
            else//if its not a one, add the die roll to the banked points for that player
            {
                clientData[clientId].DieRoll = roll;
                clientData[clientId].BankedPoints += roll;

                CallBackInfo temp = new CallBackInfo();
                temp.DieRoll = roll;
                temp.BankedPoints = 0;
                temp.TotalPoints = clientData[clientId].TotalPoints;

                int x = 1;
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(temp, clientId);
                    x++;
                }
            }
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
                    //start with player turns
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
           //sets player id to the next player
            playerId++;
            //check to see if next player exceeds total players
            if (playerId > clientData.Count)
            {
                //if it does, it sets it back to player one
                playerId = 1;
            }
            //enable the current players turn and disable the rest
            EnablePlayerUI(playerId);
        }

    }
}
