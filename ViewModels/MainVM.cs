using ElevatorSimulator.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WGO.ViewModels;

namespace ElevatorSimulator.ViewModels
{
    public class MainVM : ObservableObject
    {
        //Creates an instance of MainVM so this class and its properties can be referenced from anywhere
        //
        public static MainVM Instance { get; private set; }
        internal static MainVM m_MainVM = null;
        public static void SetWnd(MainVM wnd)
        {
            m_MainVM = wnd;
        }
        public MainVM()
        {
            Instance = this;
            SetWnd(Instance);

            myThread = new Thread(ActivateElevator);
            myThread.IsBackground = true;
            //ValidateTxtFile("C:\\Users\\yuall\\Desktop\\Coding\\TestElevator.txt");

        }

        Thread myThread = null;
        #region Variables


        private string m_testList;
        public string testList
        {
            get { return m_testList; }
            set
            {
                if (this.m_testList != value)
                {
                    this.m_testList = value;
                    this.RaisePropertyChangedEvent("testList");
                }
            }
        }

        private int passengerCount = 1;
        private int waitInterval = 10;
        private int floorExit = -1;
        private bool elevatorActivated = false;
        private int delayTime = 2000;
        private Elevator m_elevatorObj = new Elevator();
        public Elevator elevatorObj
        {
            get { return m_elevatorObj; }
            set
            {
                if (this.m_elevatorObj != value)
                {
                    this.m_elevatorObj = value;
                    this.RaisePropertyChangedEvent("elevatorObj");
                }
            }
        }


        private string m_fileName;
        public string fileName
        {
            get { return m_fileName; }
            set
            {
                if (this.m_fileName != value)
                {
                    this.m_fileName = value;
                    this.RaisePropertyChangedEvent("fileName");
                }
            }
        }


        private string m_statusUpdate = "";
        public string statusUpdate
        {
            get { return m_statusUpdate; }
            set
            {
                if (this.m_statusUpdate != value)
                {
                    this.m_statusUpdate = value;
                    this.RaisePropertyChangedEvent("statusUpdate");
                }
            }
        }


        private int m_floorRequested = -1;
        public int floorRequested
        {
            get { return m_floorRequested; }
            set
            {
                if (this.m_floorRequested != value)
                {
                    this.m_floorRequested = value;
                    this.RaisePropertyChangedEvent("floorRequested");
                }
            }
        }

        private Visibility m_elevatorFloorsVisibility = Visibility.Collapsed;
        public Visibility elevatorFloorsVisibility
        {
            get { return m_elevatorFloorsVisibility; }
            set
            {
                if (this.m_elevatorFloorsVisibility != value)
                {
                    this.m_elevatorFloorsVisibility = value;
                    this.RaisePropertyChangedEvent("elevatorFloorsVisibility");
                }
            }
        }

        private ObservableCollection<Passengers> m_passengerList = new ObservableCollection<Passengers>();
        public ObservableCollection<Passengers> passengerList
        {
            get { return m_passengerList; }
            set
            {
                if (this.m_passengerList != value)
                {
                    this.m_passengerList = value;
                    this.RaisePropertyChangedEvent("passengerList");
                }
            }
        }

        private ObservableCollection<HelperClass> m_elevatorMessages = new ObservableCollection<HelperClass>();
        public ObservableCollection<HelperClass> elevatorMessages
        {
            get { return m_elevatorMessages; }
            set
            {
                if (this.m_elevatorMessages != value)
                {
                    this.m_elevatorMessages = value;
                    this.RaisePropertyChangedEvent("elevatorMessages");
                }
            }
        }
        #endregion

        #region Buttons
        //Opens a file dialog box to select a file to import
        //Calls ValidateTxtFile to check the selected file
        public ICommand ImportTxtFileCommand { get { return new DelegateCommand(m_ImportTxtFileCommand); } }
        private void m_ImportTxtFileCommand()
        {
            Nullable<bool> result = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text Files (*.txt)|*.txt";
            result = dialog.ShowDialog();
            if (result == true)
                fileName = dialog.FileName;
            ValidateTxtFile(fileName);
        }

        //Test Function
        //Creates 5 random passengers with random floors to enter and exit on (1-10)
        //Starts the elevator
        public ICommand CreateRandomPassengers { get { return new DelegateCommand(m_CreateRandomPassengers); } }
        private void m_CreateRandomPassengers()
        {
            Random rnd = new Random();
            for (int i = 0; i < 5; i++) 
            {
                int start = Convert.ToInt32(rnd.NextInt64(1,11));
                int exit = Convert.ToInt32(rnd.NextInt64(1, 11));
                passengerList.Add(new Passengers(passengerCount, start, exit));
                testList += (start + "-" + exit + "\n");
                passengerCount++;
            }

            if (!myThread.IsAlive)
            {
                myThread = new Thread(ActivateElevator);
                myThread.Start();
            }
        }

        //When the elevator is called to go up on any floor, the view changes to the panel to select the floor to get off on.
        //Button command, CommandParameter - Button Name
        public ICommand ElevatorUpCommand { get { return new RelayCommand<string>(m_ElevatorUpCommand); } }
        private void m_ElevatorUpCommand(string s)
        {
            s = s.Replace("RequestUp", "");
            elevatorFloorsVisibility = Visibility.Visible;
            floorRequested = Convert.ToInt32(s);
        }

        //When the elevator is called to go down on any floor, the view changes to the panel to select the floor to get off on.
        //Button command, CommandParameter - Button Name
        public ICommand ElevatorDownCommand { get { return new RelayCommand<string>(m_ElevatorDownCommand); } }
        private void m_ElevatorDownCommand(string s)
        {
            s = s.Replace("RequestDown", "");
            elevatorFloorsVisibility = Visibility.Visible;
            floorRequested = Convert.ToInt32(s);
        }

        //Cancel option to return to view all the floors
        public ICommand CancelCommand { get { return new DelegateCommand(m_CancelCommand); } }
        private void m_CancelCommand()
        {
            elevatorFloorsVisibility = Visibility.Collapsed;
        }

        //RelayCommand for when a button on the elevator panel is selected, the command parameter is the
        //floor selected to exit on
        //Button Command, CommandParameter - Button Content
        public ICommand ElevatorFloorButtonCommand { get { return new RelayCommand<string>(m_ElevatorFloorButtonCommand); } }
        private void m_ElevatorFloorButtonCommand(string s)
        {
            elevatorFloorsVisibility = Visibility.Collapsed;
            floorExit = Convert.ToInt32(s);
            passengerList.Add(new Passengers(passengerCount,floorRequested, floorExit));
            passengerCount++;
            if (!myThread.IsAlive)
            {
                myThread = new Thread(ActivateElevator);
                myThread.Start();
            }
        }
        #endregion

        #region Functions
        //Checks the file to make sure that the values are valid
        //if so, add all passengers to the list and start the elevator using ActivateElevator()
        private void ValidateTxtFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = "";
                int lineCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    string[] enterExit = line.Split('-');
                    if (enterExit.Length == 2)
                    {
                        if (int.TryParse(enterExit[0], out int enter) && int.TryParse(enterExit[1], out int exit))
                        {
                            if (enter > 0 && enter < 11 && exit > 0 && exit < 11)
                            {

                                passengerList.Add(new Passengers(passengerCount, enter, exit));
                                passengerCount++;
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Error: Valid floors are 1-10.");
                                return;
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show($"Error: Please make sure the file is formatted properly to be read into the tool.\n\n[enterFloor]-[exitFloor]\n\nError found on line: {lineCount}");
                            return;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Error: Please make sure the file is formatted properly to be read into the tool.\n\n[enterFloor]-[exitFloor]\n\nError found on line: {lineCount}");
                        return;
                    }
                }
            }
            if (!myThread.IsAlive)
            {
                myThread = new Thread(ActivateElevator);
                myThread.Start();
            }
        }

        //In order for the elevator to move, this function must be called
        private async void ActivateElevator()
        {
            while (passengerList.Count > 0)
            {
                //Console.WriteLine(elevatorObj.moving);
                //if (elevatorObj.moving)
                //{
                //    Thread.Sleep(TimeSpan.FromSeconds(1));
                //    continue;
                //}
                //else
                elevatorObj.direction = passengerList[0].direction;
                if (passengerList[0].direction == "up")
                {
                    if (elevatorObj.currentFloor != passengerList[0].enterFloor && elevatorObj.currentFloor > passengerList[0].enterFloor)
                        ElevatorDown();
                    else
                        ElevatorUp();
                }
                else
                {
                    if (elevatorObj.currentFloor != passengerList[0].enterFloor && elevatorObj.currentFloor < passengerList[0].enterFloor)
                        ElevatorUp();
                    else
                        ElevatorDown();
                }
            }
            if (passengerCount == 0)
                elevatorObj.moving = false;
        }

        //When the elevator is actived, and the elevator's next passenger has selected to go up
        private async void ElevatorUp()
        {
            elevatorObj.moving = true;
            #region if passenger requested to go down, but the elevator needs to go up to pick them up
            if (passengerList.Count() != 0)
            {
                if (passengerList[0].direction == "down")
                {
                    while (elevatorObj.currentFloor != passengerList[0].enterFloor)
                    {
                        IncrementPassengerTime();
                        //await Task.Delay(delayTime);
                        if (elevatorObj.currentFloor + 1 < 11 && passengerList.Count() > 0 &&
                            passengerList.Where(u => u.direction == "down").Count() > 0)
                            elevatorObj.currentFloor++;
                    }
                    elevatorObj.moving = false;
                    return;
                }
            }
            #endregion

            var passengers = passengerList.Where(u => u.enterFloor >= elevatorObj.currentFloor && (u.direction == "up"));
            foreach (Passengers pass in passengers)
            {
                pass.passengerStatus = "in queue";
            }

            //Check for passengers ENTERING elevator
            while (passengerList.Where(u => u.passengerStatus == "in queue" || u.passengerStatus == "in elevator").Count() > 0) // && u.direction == "up"
            {
                passengers = passengerList.Where(u => u.enterFloor == elevatorObj.currentFloor && (u.direction == "up") && (u.passengerStatus != "in elevator"));
                foreach (Passengers pass in passengers)
                {
                    pass.inElevator = true;
                    pass.passengerStatus = "in elevator";

                    AddMessages($"Passenger {pass.passengerNo} ENTER from floor {elevatorObj.currentFloor}");


                }
                passengers = passengerList.Where(u => u.exitFloor == elevatorObj.currentFloor && (u.direction == "up") && u.passengerStatus == "in elevator");
                //if (passengers.Count() > 0)
                //    IncrementPassengerTime();
                for (int i = passengers.Count() - 1; i >= 0; i--)
                {
                    AddMessages($"Passenger {passengers.ElementAt(i).passengerNo} EXIT on floor {elevatorObj.currentFloor} (Enter: {passengers.ElementAt(i).enterFloor}, Exit: {passengers.ElementAt(i).exitFloor})\n -> [Wait Time: {passengers.ElementAt(i).waitTime}] [Travel Time: {passengers.ElementAt(i).inElevatorTime}]");
                    ElevatorSimulator.App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        passengerList.Remove(passengers.ElementAt(i));
                    });
                }

                IncrementPassengerTime();
                //await Task.Delay(delayTime);
                if (elevatorObj.currentFloor + 1 < 11 && passengerList.Count() > 0 && 
                    passengerList.Where(u => u.direction == "up").Count() > 0)
                    elevatorObj.currentFloor++;

            }
            elevatorObj.moving = false;
        }

        //When the elevator is actived, and the elevator's next passenger has selected to go down
        private async void ElevatorDown()
        {
            elevatorObj.moving = true;
            #region if passenger requested to go up, but the elevator needs to go down to pick them up
            if (passengerList.Count() != 0)
            {
                //var tempPassengersList = passengerList.Where(u => u.direction == "down");
                if (passengerList[0].direction == "up")
                {
                    while (elevatorObj.currentFloor != passengerList[0].enterFloor)
                    {
                        IncrementPassengerTime();
                        if (elevatorObj.currentFloor - 1 > 0 && passengerList.Count() > 0 &&
                            passengerList.Where(u => u.direction == "up").Count() > 0)
                            elevatorObj.currentFloor--;
                    }
                    elevatorObj.moving = false;
                    return;
                }
            }
            #endregion

            var passengers = passengerList.Where(u => u.enterFloor <= elevatorObj.currentFloor && (u.direction == "down"));
            foreach (Passengers pass in passengers)
            {
                pass.passengerStatus = "in queue";
            }

            //Check for passengers ENTERING elevator
            while (passengerList.Where(u => u.passengerStatus == "in queue" || u.passengerStatus == "in elevator").Count() > 0)
            {
                passengers = passengerList.Where(u => u.enterFloor == elevatorObj.currentFloor && (u.direction == "down") && (u.passengerStatus != "in elevator"));
                foreach (Passengers pass in passengers)
                {
                    pass.inElevator = true;
                    pass.passengerStatus = "in elevator";

                    AddMessages($"Passenger {pass.passengerNo} ENTER from floor {elevatorObj.currentFloor}" );
                }
                passengers = passengerList.Where(u => u.exitFloor == elevatorObj.currentFloor && (u.direction == "down") && u.passengerStatus == "in elevator");
                //if (passengers.Count() > 0)
                //    IncrementPassengerTime();
                for (int i = passengers.Count() - 1; i >= 0; i--)
                {
                    AddMessages($"Passenger {passengers.ElementAt(i).passengerNo} EXIT on floor {elevatorObj.currentFloor} (Enter: {passengers.ElementAt(i).enterFloor}, Exit: {passengers.ElementAt(i).exitFloor})\n -> [Wait Time: {passengers.ElementAt(i).waitTime}] [Travel Time: {passengers.ElementAt(i).inElevatorTime}]");
                    ElevatorSimulator.App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        passengerList.Remove(passengers.ElementAt(i));
                    });
                }

                IncrementPassengerTime();
                if (elevatorObj.currentFloor - 1 > 0 && passengerList.Count() > 0 &&
                    passengerList.Where(u => u.direction == "down").Count() > 0)
                    elevatorObj.currentFloor--;

            }
            elevatorObj.moving = false;
        }

        //Increments passenger wait time and travel time
        //if passenger is not in the elevator, wait time is increased
        //if passenger is in the elevator, travel time is increased.
        private void IncrementPassengerTime()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            foreach(Passengers pass in passengerList)
            {
                if (pass.inElevator)
                    pass.inElevatorTime += waitInterval;
                else
                    pass.waitTime += waitInterval;
            }
        }

        public void AddMessages(string message)
        {
            ElevatorSimulator.App.Current.Dispatcher.Invoke((Action)delegate
            {
                elevatorMessages.Add(new HelperClass() { Text = message });
            });
        }
        #endregion
    }

    //Helper class to display messages
    public class HelperClass : ObservableObject
    {
        private string m_Text;
        public string Text
        {
            get { return m_Text; }
            set
            {
                if (this.m_Text != value)
                {
                    this.m_Text = value;
                    this.RaisePropertyChangedEvent("Text");
                }
            }
        }
    }
}
