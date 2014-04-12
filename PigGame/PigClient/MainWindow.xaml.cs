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
        private int currentPlayerId = 0;
        private int totalPlayers = 0;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Configure the Endpoint details
                DuplexChannelFactory<IPig> channel = new DuplexChannelFactory<IPig>(this, "Pig");

                // Activate a remote Shoe object
                pig = channel.CreateChannel();

                // Regsister this client for the callbacks service
                callbackId = pig.RegisterForCallbacks();

                //updateCardCounts();

                this.Title = "Hello Player " + callbackId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (callbackId != 0 && pig != null)
                pig.UnregisterForCallbacks(callbackId);
        }

        private void checkBoxReady_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.ClientReady(callbackId);
                pig.StartGame(); // send a message to the service which will check that we have enough players that are all ready

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void checkBoxReady_Unchecked(object sender, RoutedEventArgs e)
        {
            pig.ClientUnReady(callbackId);
        }

        private delegate void ClientUpdateDelegate(CallBackInfo info, int id);
        public void UpdateGui(CallBackInfo info, int id)
        {
            //use this method to refresh everyones GUI?
            
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                try
                {
                    //// if the player has changed
                    //if (id != currentPlayerId)
                    //{
                    //    textBoxLog.AppendText("Player " + id + "'s turn\n");
                    //    scrollViewer.ScrollToBottom();
                    //    currentPlayerId = id;
                    //    // change the border for the groupbox for whatever player is player
                    //    // have to figure out a clean way to get the correct groupBox for the current player

                    //    //PROBLEM WITH THIS IS HOW TO WE SET THE PREVIOUS PLAYERS GROUPBOX TO THE DEFAULT COLOUR?
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
                        //**************************************
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
           private delegate void StartGameDelegate(int t);
           public void StartGame(int total)
           {

               if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
               {
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
                  // this.Dispatcher.BeginInvoke(new MessageDelegate(ShowMessage), message);
                   this.Dispatcher.BeginInvoke(new StartGameDelegate(StartGame), total);
               }

           }
               
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

        private void buttonRoll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.Roll(callbackId);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonStay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pig.Stay(callbackId);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

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
