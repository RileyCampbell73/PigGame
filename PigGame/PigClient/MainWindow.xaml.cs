///File Name: MainWindow.xaml.ca
///Authors: James Haig, Riley Campbell
///Date: 4/11/2014
///Version 1.00.00
///Purpose: This is a client for a game of Pig.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ServiceModel;
using PigLib;

namespace PigClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ICallback
    {
        private IPig pig = null;
        private int callbackId = 0;
        private int totalPlayers = 0;

        public MainWindow()
        {
            InitializeComponent();

            //Initialization Code
            try
            {
                // Configure the Endpoint details
                DuplexChannelFactory<IPig> channel = new DuplexChannelFactory<IPig>(this, "Pig");

                // Activate a remote Shoe object
                pig = channel.CreateChannel();

                // Regsister this client for the callbacks service
                callbackId = pig.RegisterForCallbacks();
                
                // If they join after the game is started, this player will not be included in the game
                if( callbackId == 0 )
                {
                    MessageBox.Show("Game has already started");
                    this.Close();
                }

                this.Title = "Hello Player " + callbackId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //this event runs when the client is closed
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (callbackId != 0 && pig != null)
                //unregister the client for call backs
                pig.UnregisterForCallbacks(callbackId);
        }

        //this event runs when the checkbox is checked
        private void checkBoxReady_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.ClientReady(callbackId);
                pig.StartGame(); // send a message to the service which will check that we have enough players that are all ready

            }
            catch(CommunicationObjectFaultedException ex){
                MessageBox.Show ("The server has been shut down. Please restart the server");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }
        //this event runs when the checkbox is unchecked
        private void checkBoxReady_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.ClientUnReady(callbackId);
            }
                //catch an error if the server is not online
            catch (CommunicationObjectFaultedException ex)
            {
                MessageBox.Show("The server has been shut down. Please restart the server");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        //this method updates the GUI for all clients
        private delegate void ClientUpdateDelegate(CallBackInfo info, int id);
        public void UpdateGui(CallBackInfo info, int id)
        {

            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                try
                {
                    //this statment changes the group box colour of the current player
                    switch (id)
                    {
                        case 1:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Red);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Gray);
                            break;
                        case 2:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Red);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Gray);
                            break;
                        case 3:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Red);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Gray);
                            break;
                        case 4:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Red);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Gray);
                            break;
                        case 5:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Red);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Gray);
                            break;
                        case 6:
                            groupBoxPlayer1.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer2.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer3.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer4.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer5.BorderBrush = new SolidColorBrush(Colors.Gray);
                            groupBoxPlayer6.BorderBrush = new SolidColorBrush(Colors.Red);
                            break;
                    }
                        
                    //}

                    if (info.DieRoll > 1)
                    {
                        // change the die picture based on whatever the player rolled
                        changeDiePicture(info.DieRoll);
                        
                        switch (id)
                        {
                            case 1:
                                textBoxPlayerPoints1.Text = Convert.ToString(info.BankedPoints);
                                break;
                            case 2:
                                textBoxPlayerPoints2.Text = Convert.ToString(info.BankedPoints);
                                break;
                            case 3:
                                textBoxPlayerPoints3.Text = Convert.ToString(info.BankedPoints);
                                break;
                            case 4:
                                textBoxPlayerPoints4.Text = Convert.ToString(info.BankedPoints);
                                break;
                            case 5:
                                textBoxPlayerPoints5.Text = Convert.ToString(info.BankedPoints);
                                break;
                            case 6:
                                textBoxPlayerPoints6.Text = Convert.ToString(info.BankedPoints);
                                break;
                        }

                        // update the log based on the roll
                        textBoxLog.AppendText("Player " + id + ": rolled a " + info.DieRoll + "\n"); 
                        scrollViewer.ScrollToBottom();
                    }
                    else if (info.DieRoll == 1)
                    {
                        // change the die picture based on whatever the player rolled
                        changeDiePicture(info.DieRoll);

                        switch (id)
                        {
                            case 1:
                                textBoxPlayerPoints1.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal1.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 2:
                                textBoxPlayerPoints2.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal2.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 3:
                                textBoxPlayerPoints3.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal3.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 4:
                                textBoxPlayerPoints4.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal4.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 5:
                                textBoxPlayerPoints5.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal5.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 6:
                                textBoxPlayerPoints6.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal6.Text = Convert.ToString(info.TotalPoints);
                                break;
                        }

                        // update the log based on the roll
                        textBoxLog.AppendText("Player " + id + ": rolled a " + info.DieRoll + " Oh No!\n");
                        
                        if (id + 1 > totalPlayers)
                        {
                            id = 1;
                            textBoxLog.AppendText("Player " + id + "'s turn!\n");
                        }
                        else
                            textBoxLog.AppendText("Player " + (id + 1) + "'s turn!\n");

                        scrollViewer.ScrollToBottom();
                    }
                    else//if die roll is 0 then that means the user is staying
                    {
                        switch (id)
                        {
                            case 1:
                                textBoxPlayerPoints1.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal1.Text = Convert.ToString(info.TotalPoints);
                                break;

                            case 2:
                                textBoxPlayerPoints2.Text = Convert.ToString(info.BankedPoints);
                                textBoxPlayerTotal2.Text = Convert.ToString(info.TotalPoints);
                                break;
                        }

                        textBoxLog.AppendText("Player " + id + ": is staying.\n");
                        if (id + 1 > totalPlayers)
                        {
                            id = 1;
                            textBoxLog.AppendText("Player " + id + "'s turn!\n");
                        }
                        else
                        textBoxLog.AppendText("Player " + (id + 1)  + "'s turn!\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info, id);
            }
        }

        //method for sending messages to the client
        private delegate void MessageDelegate(string s);
        public void ShowMessage( string message )
        {
            try
            {
                if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
                {
                    MessageBox.Show(message);
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new MessageDelegate(ShowMessage), message);
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

            //this method is for starting the game 
           private delegate void StartGameDelegate(int t);
           public void StartGame(int total)
           {

               if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
               {
                   // since it's possible for the player's Id to have changed (if some players join and leave before or between games), change the title so that the player knows.
                   this.Title = "Hello Player " + callbackId;

                   totalPlayers = total;
                   //set all text boxes to 0 and dice to blank

                   imgDie.Source = null;

                   checkBoxReady.IsEnabled = false;

                   textBoxPlayerPoints1.Text = "";
                   textBoxPlayerTotal1.Text = "";

                   textBoxPlayerPoints2.Text = "";
                   textBoxPlayerTotal2.Text = "";

                   textBoxPlayerPoints3.Text = "";
                   textBoxPlayerTotal3.Text = "";

                   textBoxPlayerPoints4.Text = "";
                   textBoxPlayerTotal4.Text = "";

                   textBoxPlayerPoints5.Text = "";
                   textBoxPlayerTotal5.Text = "";

                   textBoxPlayerPoints6.Text = "";
                   textBoxPlayerTotal6.Text = "";
               }
               else
               {
                   this.Dispatcher.BeginInvoke(new StartGameDelegate(StartGame), total);
               }

           }
               
        //this method resets the UI
        private delegate void ResetUIDelegate();
        public void ResetUI()
        {
            try
            {
                if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
                {
                    //reset the buttons and show check box
                    buttonRoll.IsEnabled = false;
                    buttonStay.IsEnabled = false;
                    checkBoxReady.IsEnabled = true;
                    checkBoxReady.IsChecked = false;

                }
                else
                {
                    this.Dispatcher.BeginInvoke(new ResetUIDelegate(ResetUI));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //This method is changing the UI depending whos turn it is
        private delegate void ChangeUIDelegate( bool b );
        public void ChangeUI( bool enableUI )
        {
            try
            {
                if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
                {
                    buttonRoll.IsEnabled = enableUI;
                    buttonStay.IsEnabled = enableUI;
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new ChangeUIDelegate(ChangeUI), enableUI);
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // update the player's Id if it needs to change (it will if people leave between or before games)
        public void UpdatePlayerId( int newId )
        {
            callbackId = newId;
        }

        //this event is called when the RollButton is clicked
        private void buttonRoll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.Roll(callbackId);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                MessageBox.Show("The server has been shut down. Please restart the server");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //this event is called when the Stay button is clicked
        private void buttonStay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.Stay(callbackId);
            }
            catch (CommunicationObjectFaultedException ex)
            {
                MessageBox.Show("The server has been shut down. Please restart the server");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //this method is for changing the die picture in the client
        private void changeDiePicture(int diceRoll)
        {
            switch (diceRoll)
            {
                case 1:
                    BitmapImage dieOne = new BitmapImage(new Uri(@"images/die_one.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieOne;
                    break;

                case 2:
                    BitmapImage dieTwo = new BitmapImage(new Uri(@"images/die_two.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieTwo;
                    break;

                case 3:
                    BitmapImage dieThree = new BitmapImage(new Uri(@"images/die_three.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieThree;
                    break;

                case 4:
                    BitmapImage dieFour = new BitmapImage(new Uri(@"images/die_four.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieFour;
                    break;

                case 5:
                    BitmapImage dieFive = new BitmapImage(new Uri(@"images/die_five.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieFive;
                    break;

                case 6:
                    BitmapImage dieSix = new BitmapImage(new Uri(@"images/die_six.gif", UriKind.RelativeOrAbsolute));
                    imgDie.Source = dieSix;
                    break;
            } 
        }
    }
}
