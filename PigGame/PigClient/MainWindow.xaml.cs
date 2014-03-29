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

        private delegate void ClientUpdateDelegate(CallBackInfo info);
        public void UpdateGui(CallBackInfo info)
        {
            //use this method to refresh everyones GUI?
            
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                try
                {
                    switch (info.DieRoll)
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
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

        private delegate void EnableUIDelegate( bool b );
        public void ChangeUI( bool enableUI )
        {
            try
            {
                if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
                {
                    buttonHit.IsEnabled = enableUI;
                    buttonStay.IsEnabled = enableUI;
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new EnableUIDelegate(ChangeUI), enableUI);
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void buttonHit_Click(object sender, RoutedEventArgs e)
        {
            pig.Roll(callbackId);
        }
    }
}
