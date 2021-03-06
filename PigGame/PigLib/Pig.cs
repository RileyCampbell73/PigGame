﻿///File Name: Pig.cs
///Authors: James Haig, Riley Campbell
///Date: 4/11/2014
///Version 1.00.00
///Purpose: This class holds all the server logic for a game of Pig.
         

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

namespace PigLib
{

    //The public interface of ICallBack
        //these will be seen by the Client 
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
        [OperationContract(IsOneWay = true)]
        void StartGame(int totalPlayers);
        [OperationContract(IsOneWay = true)]
        void UpdatePlayerId(int newId);
    }

    //The public interface of IPig
     //these will be seen by the Pig Class 
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
        private int nextCallbackId = 1;
        private bool startGame = false; // when all clients are ready, flip this
        //dictionary for the players data
        private Dictionary<int, CallBackInfo> clientData = new Dictionary<int, CallBackInfo>();
        private int numWinningPoints = 100;
        private int playerId;

        // C'tor
        public Pig()
        {
            Console.WriteLine("Creating a Pig!");
        }

        //this method is for when players want to stay.
        public void Stay(int clientId)  
        {
            //Takes the points they have accrued and adds them to that players total points
            clientData[clientId].TotalPoints += clientData[clientId].BankedPoints;
            clientData[clientId].BankedPoints = 0;

            CallBackInfo temp = new CallBackInfo();
            temp.DieRoll = 0;
            temp.BankedPoints = 0;
            temp.TotalPoints = clientData[clientId].TotalPoints;

            //If a player has gotten over t100 points then they win the game
            if (clientData[clientId].TotalPoints >= numWinningPoints)
                GameEnd(clientId);
            else
            {
                //Then updates all the GUI's
                foreach( ICallback cb in clientCallbacks.Values )
                    cb.UpdateGui(temp, clientId);
                Game();
            }
        }

        //This method is for when the game ends.    
        private void GameEnd(int player){
            SendMessage("Player " + player + " is the winner with " +clientData[player].TotalPoints +" points!");

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
            startGame = false; // so new players can join
        }

        //This method is for when a player chooses to Roll
        public void Roll(int clientId)
        {
            //for making random numbers
            var r = new Random();
            //put a random number between 1 and 6 into this int. (range is exclusive, so we'll never actually roll a 7)
            int roll = r.Next(1, 7);

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

                //update the players 
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(temp, clientId);
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
                temp.BankedPoints = clientData[clientId].BankedPoints;
                temp.TotalPoints = clientData[clientId].TotalPoints;
                //update the players 
                foreach (ICallback cb in clientCallbacks.Values)
                {
                    cb.UpdateGui(temp, clientId);
                }
            }
        }

        //Registers new players for callbacks
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

                return nextCallbackId++;
            }
            else
                return 0;
        }
        
        //unregisters players when they close their clients
        public void UnregisterForCallbacks(int id)
        {
            clientCallbacks.Remove(id);
            clientData.Remove(id);
            if (clientCallbacks.Count < 2)
            {
                SendMessage("Too many players have left!  Game over!");
                // disable the one player's UI who is left
                clientCallbacks.First().Value.ChangeUI(false);
            }
            else
            {
                // if the player left who was currently supposed to be playing, move on to the next player
                if (playerId == id)
                    Game(); // keep the game going
            }
        }

        //method for when a client is ready to start the game
        public void ClientReady( int id )
        {
            if (id != 0) // make sure this player has a valid ID, they shouldn't be added if they joined after the game started
                clientData[id].Ready = true;
        }

        //method for when a client is NOT ready to start the game
        public void ClientUnReady( int id )
        {
            if (id != 0)
                clientData[id].Ready = false;
        }

        //Method to start the game for all the players and lock out any other players so they cannot join part way through
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
                    
                    // just in case people have joined and left and left holes in our Id's, shuffle the players so we have no gaps
                    removeIdGaps();
                    
                    //start with player turns
                    foreach (ICallback cb in clientCallbacks.Values)
                    {
                        cb.StartGame(clientData.Count());
                    }
                    //set playerId so we know who should start first;
                    playerId = 0;
                    Game();
                }
            }
            else
            {
                // tell the clients that there are too many or too few players
                SendMessage("Only 2 to 6 players allowed!");
            }

        }

        // this allows us to send a message to one or all clients, if the id is -1 we'll show the message to everyone
        public void SendMessage( string message, int id = -1 )
        {
            foreach ( KeyValuePair<int,ICallback> kvp in clientCallbacks )
            {
                if (id == kvp.Key || id == -1)
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
        
        //this method is for the game logic
            //it makes turn advance to the next player
        public void Game()
        {
            bool foundNextPlayer = false;
            foreach(int i in clientCallbacks.Keys)
            {
                if (i > playerId)
                {
                    playerId = i;
                    foundNextPlayer = true;
                    break;
                }
            }
            // if we didn't find another player, we already enabled the last player's UI
            if (!foundNextPlayer)
            {
                playerId = clientCallbacks.Keys.First();
            }

            EnablePlayerUI(playerId);
        }

        // helper to remove gaps in id's to be used between games.
        // this is because if we end up having an id over 6 our client UI will be messed up
        private void removeIdGaps()
        {
            // create new dictionaries and fill them with the old data, with new Id's that don't have gaps
            int i = 1;
            Dictionary<int,ICallback> newCallbacks = new Dictionary<int,ICallback>();
            foreach( KeyValuePair<int, ICallback> cb in clientCallbacks )
            {
                newCallbacks.Add(i++, cb.Value);
            }

            i = 1;
            Dictionary<int, CallBackInfo> newClientData = new Dictionary<int, CallBackInfo>();
            foreach (KeyValuePair<int, CallBackInfo> cbi in clientData)
            {
                newClientData.Add(i++, cbi.Value);
            }

            // feed the new dictionaries into our global objects and reset the next client id so we can keep adding new players with correct Id's
            clientCallbacks = newCallbacks;
            clientData = newClientData;
            nextCallbackId = clientCallbacks.Keys.Last() + 1;

            // Let each player know about their new Id
            foreach( KeyValuePair<int, ICallback> cb in clientCallbacks )
            {
                cb.Value.UpdatePlayerId(cb.Key);
            }
        }

    }
}
