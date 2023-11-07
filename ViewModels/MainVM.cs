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

            //ValidateTxtFile("C:\\Users\\yuall\\Desktop\\Coding\\TestElevator.txt");
        }

        #region Variables
        private int passengerCount = 1;
        private int waitInterval = 10;
        private int floorRequested = -1;
        private int floorExit = -1;
        private bool elevatorActivated = false;
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


        private HelperClass m_latestMessage = null;
        public HelperClass latestMessage
        {
            get { return m_latestMessage; }
            set
            {
                if (this.m_latestMessage != value)
                {
                    this.m_latestMessage = value;
                    this.RaisePropertyChangedEvent("latestMessage");
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
            get 
            {
                if (m_elevatorMessages.Count() != 0)
                    latestMessage = m_elevatorMessages[m_elevatorMessages.Count() - 1];
                return m_elevatorMessages; 
            }
            set
            {
                if (this.m_elevatorMessages != value)
                {
                    this.m_elevatorMessages = value;
                    this.RaisePropertyChangedEvent("elevatorMessages");
                }
            }
        }





        //private List<Passengers> m_passengersWaiting = new List<Passengers>();
        //public List<Passengers> passengersWaiting
        //{
        //    get { return m_passengersWaiting; }
        //    set
        //    {
        //        if (this.m_passengersWaiting != value)
        //        {
        //            this.m_passengersWaiting = value;
        //            this.RaisePropertyChangedEvent("passengersWaiting");
        //        }
        //    }
        //}


        //private List<Passengers> m_passengersInQueue = new List<Passengers>();
        //public List<Passengers> passengersInQueue
        //{
        //    get { return m_passengersInQueue; }
        //    set
        //    {
        //        if (this.m_passengersInQueue != value)
        //        {
        //            this.m_passengersInQueue = value;
        //            this.RaisePropertyChangedEvent("passengersInQueue");
        //        }
        //    }
        //}


        //private List<Passengers> m_passengersInElevator = new List<Passengers>();
        //public List<Passengers> passengersInElevator
        //{
        //    get { return m_passengersInElevator; }
        //    set
        //    {
        //        if (this.m_passengersInElevator != value)
        //        {
        //            this.m_passengersInElevator = value;
        //            this.RaisePropertyChangedEvent("passengersInElevator");
        //        }
        //    }
        //}









        #endregion

        #region Buttons
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


        public ICommand ElevatorUpCommand { get { return new RelayCommand<string>(m_ElevatorUpCommand); } }
        private void m_ElevatorUpCommand(string s)
        {
            s = s.Replace("RequestUp", "");
            elevatorFloorsVisibility = Visibility.Visible;
            floorRequested = Convert.ToInt32(s);
        }
        public ICommand ElevatorDownCommand { get { return new RelayCommand<string>(m_ElevatorDownCommand); } }
        private void m_ElevatorDownCommand(string s)
        {
            s = s.Replace("RequestDown", "");
            elevatorFloorsVisibility = Visibility.Visible;
            floorRequested = Convert.ToInt32(s);
        }

        public ICommand ElevatorFloorButtonCommand { get { return new RelayCommand<string>(m_ElevatorFloorButtonCommand); } }
        private void m_ElevatorFloorButtonCommand(string s)
        {
            elevatorFloorsVisibility = Visibility.Collapsed;
            floorExit = Convert.ToInt32(s);
            passengerList.Add(new Passengers(passengerCount,floorRequested, floorExit));
            passengerCount++;
            if (!elevatorObj.moving)
            {
                ActivateElevator();
            }
        }
        #endregion

        #region Functions
        private async void ValidateTxtFile(string fileName)
        {
            //passengersWaiting.Clear();
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
            ActivateElevator();
        }

        private async void ActivateElevator()
        {
            while (passengerList.Count > 0)
            {
                if (elevatorObj.moving)
                {
                    await Task.Delay(1000);
                    continue;
                }
                else
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
            //if (passengersWaiting.Count() != 0)
            //{
            //    //if (passengersWaiting)
            //}
        }
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
                        await Task.Delay(1000);
                        if (elevatorObj.currentFloor + 1 < 11 && passengerList.Count() > 0)
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

                    elevatorMessages.Add(new HelperClass() { Text = $"Passenger {pass.passengerNo} ENTERed floor {elevatorObj.currentFloor}" });


                }
                passengers = passengerList.Where(u => u.exitFloor == elevatorObj.currentFloor && (u.direction == "up"));
                //foreach (Passengers pass in passengers)
                //{
                    for (int i = passengers.Count() - 1; i >= 0; i--)
                    {
                        elevatorMessages.Add(new HelperClass() { Text = $"Passenger {passengers.ElementAt(i).passengerNo} EXITed on floor {elevatorObj.currentFloor}" });
                        passengerList.Remove(passengers.ElementAt(i));
                    }
                //}
                IncrementPassengerTime();
                await Task.Delay(2000);
                if (elevatorObj.currentFloor + 1 < 11 && passengerList.Count() > 0)
                    elevatorObj.currentFloor++;

            }
            elevatorObj.moving = false;
        }

        private async void ElevatorDown()
        {
            elevatorObj.moving = true;
            #region if passenger requested to go up, but the elevator needs to go down to pick them up
            if (passengerList.Count() != 0)
            {
                if (passengerList[0].direction == "up")
                {
                    while (elevatorObj.currentFloor != passengerList[0].enterFloor)
                    {
                        IncrementPassengerTime();
                        await Task.Delay(1000);
                        if (elevatorObj.currentFloor - 1 > 0 && passengerList.Count() > 0)
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

                    elevatorMessages.Add(new HelperClass() { Text = $"Passenger {pass.passengerNo} ENTERed floor {elevatorObj.currentFloor}" });
                }
                passengers = passengerList.Where(u => u.exitFloor == elevatorObj.currentFloor && (u.direction == "down"));
                //foreach (Passengers pass in passengers)
                //{
                for (int i = passengers.Count() - 1; i >= 0; i--)
                {
                    elevatorMessages.Add(new HelperClass() { Text = $"Passenger {passengers.ElementAt(i).passengerNo} EXITed on floor {elevatorObj.currentFloor}" });
                    passengerList.Remove(passengers.ElementAt(i));
                }
                //}
                IncrementPassengerTime();
                await Task.Delay(1000);
                if (elevatorObj.currentFloor - 1 > 0 && passengerList.Count() > 0)
                    elevatorObj.currentFloor--;

            }
            elevatorObj.moving = false;
        }

        private void IncrementPassengerTime()
        {
            foreach(Passengers pass in passengerList)
            {
                if (pass.inElevator)
                    pass.inElevatorTime += waitInterval;
                else
                    pass.waitTime += waitInterval;
            }
        }
        #endregion
    }

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
