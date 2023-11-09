using ElevatorSimulator.Models;
using ElevatorSimulator.ViewModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElevatorSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBlock> textBlocks= new List<TextBlock>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            ((INotifyCollectionChanged)messagesListView.ItemsSource).CollectionChanged += new NotifyCollectionChangedEventHandler(MessengerCollectionChanged);
            textBlocks.Add(Floor1);
            textBlocks.Add(Floor2);
            textBlocks.Add(Floor3);
            textBlocks.Add(Floor4);
            textBlocks.Add(Floor5);
            textBlocks.Add(Floor6);
            textBlocks.Add(Floor7);
            textBlocks.Add(Floor8);
            textBlocks.Add(Floor9);
            textBlocks.Add(Floor10);
        }
        
        //When the Messages ListView's collection is changed, this function is called
        //Scrolls into view the newest message on the bottom
        //Also changes the floor up/down button colors as it is called / a passenger is picked up
        private void MessengerCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<HelperClass> list = (ObservableCollection<HelperClass>)sender;
            messagesListView.ScrollIntoView(list[list.Count() - 1]);

            if (list[list.Count() - 1].Text == "Elevator Stopped")
            {
                list[list.Count() - 1].Text = $"Elevator Stopped on floor {MainVM.Instance.elevatorObj.currentFloor}";
            }

            foreach (Passengers item in MainVM.Instance.passengerList)
            {
                if (item.enterFloor == 1)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp1.Background = Brushes.Green;
                        else
                            RequestDown1.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp1.Background = Brushes.Transparent;
                        else
                            RequestDown1.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 2)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")

                    {
                        if (item.direction == "up")
                            RequestUp2.Background = Brushes.Green;
                        else
                            RequestDown2.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp2.Background = Brushes.Transparent;
                        else
                            RequestDown2.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 3)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp3.Background = Brushes.Green;
                        else
                            RequestDown3.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp3.Background = Brushes.Transparent;
                        else
                            RequestDown3.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 4)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp4.Background = Brushes.Green;
                        else
                            RequestDown4.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp4.Background = Brushes.Transparent;
                        else
                            RequestDown4.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 5)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp5.Background = Brushes.Green;
                        else
                            RequestDown5.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp5.Background = Brushes.Transparent;
                        else
                            RequestDown5.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 6)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp6.Background = Brushes.Green;
                        else
                            RequestDown6.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp6.Background = Brushes.Transparent;
                        else
                            RequestDown6.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 7)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp7.Background = Brushes.Green;
                        else
                            RequestDown7.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp7.Background = Brushes.Transparent;
                        else
                            RequestDown7.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 8)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp8.Background = Brushes.Green;
                        else
                            RequestDown8.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp8.Background = Brushes.Transparent;
                        else
                            RequestDown8.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 9)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp9.Background = Brushes.Green;
                        else
                            RequestDown9.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp9.Background = Brushes.Transparent;
                        else
                            RequestDown9.Background = Brushes.Transparent;
                    }
                }
                else if (item.enterFloor == 10)
                {
                    if (item.passengerStatus == "waiting" || item.passengerStatus == "in queue")
                    {
                        if (item.direction == "up")
                            RequestUp10.Background = Brushes.Green;
                        else
                            RequestDown10.Background = Brushes.Red;
                    }
                    else
                    {
                        if (item.direction == "up")
                            RequestUp10.Background = Brushes.Transparent;
                        else
                            RequestDown10.Background = Brushes.Transparent;
                    }
                }
            }

        }

        //Changes the Floor # textblock to show which floor the elevator is on
        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            int floor = Convert.ToInt32(((TextBlock)sender).Text) - 1;
            foreach (TextBlock txtBlck in textBlocks)
                txtBlck.FontWeight = FontWeights.Normal;
            textBlocks[floor].FontWeight = FontWeights.Bold;
        }
    }
}