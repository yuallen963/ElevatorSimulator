using ElevatorSimulator.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WGO.ViewModels;

namespace ElevatorSimulator.ViewModels
{
    public class MainVM : ObservableObject
    {
        public MainVM()
        {
            ValidateTxtFile("C:\\Users\\yuall\\Desktop\\Coding\\TestElevator.txt");
        }
        #region Variables
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


        private List<Passengers> m_passengerList = new List<Passengers>();
        public List<Passengers> passengerList
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



        private List<Passengers> m_passengersWaiting = new List<Passengers>();
        public List<Passengers> passengersWaiting
        {
            get { return m_passengersWaiting; }
            set
            {
                if (this.m_passengersWaiting != value)
                {
                    this.m_passengersWaiting = value;
                    this.RaisePropertyChangedEvent("passengersWaiting");
                }
            }
        }


        private List<Passengers> m_passengersInQueue = new List<Passengers>();
        public List<Passengers> passengersInQueue
        {
            get { return m_passengersInQueue; }
            set
            {
                if (this.m_passengersInQueue != value)
                {
                    this.m_passengersInQueue = value;
                    this.RaisePropertyChangedEvent("passengersInQueue");
                }
            }
        }


        private List<Passengers> m_passengersInElevator = new List<Passengers>();
        public List<Passengers> passengersInElevator
        {
            get { return m_passengersInElevator; }
            set
            {
                if (this.m_passengersInElevator != value)
                {
                    this.m_passengersInElevator = value;
                    this.RaisePropertyChangedEvent("passengersInElevator");
                }
            }
        }









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
        #endregion

        #region Functions
        private void ValidateTxtFile(string fileName)
        {
            passengersWaiting.Clear();
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
                                passengersWaiting.Add(new Passengers(passengersWaiting.Count() + 1, enter, exit));
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
            ElevatorUp();
            if (passengersWaiting.Count() != 0)
            {
                //if (passengersWaiting)
            }
        }
        private async void ElevatorUp()
        {
            if (!elevatorActivated)
            {
                elevatorActivated = true;

                if (elevatorObj.direction == "up")
                {
                    //passengersWaiting.Sort((x, y) => x.enterFloor.CompareTo(y.enterFloor));
                    for (int i = 0; i < passengersWaiting.Count(); i++)
                    {
                        if (passengersWaiting[i].enterFloor >= elevatorObj.currentFloor &&
                            Convert.ToInt32(passengersWaiting[i].exitFloor) > Convert.ToInt32(passengersWaiting[i].enterFloor))
                        {
                            passengersInQueue.Add(passengersWaiting[i]);
                            passengersWaiting.RemoveAt(i);
                            i--;
                        }
                    }
                    while (passengersInQueue.Count() != 0 || passengersInElevator.Count() != 0)
                    {
                        if (passengersInQueue.Count() > 0)
                        {
                            //if (passengersInQueue[0].enterFloor == elevatorObj.currentFloor)
                            List<Passengers> pass = passengersInQueue.FindAll(u => u.enterFloor == elevatorObj.currentFloor);
                            if (pass.Count() > 0)
                            {
                                for (int i = pass.Count() - 1; i >= 0; i--)
                                {
                                    passengersInElevator.Add(pass[i]);
                                    Debug.WriteLine($"Passenger {pass[i].passengerNo} entering elevator");
                                    passengersInQueue.Remove(pass[i]);
                                }
                            }
                        }
                        if (passengersInElevator.Count() > 0)
                        {
                            List<Passengers> pass = passengersInElevator.FindAll(u => u.exitFloor == elevatorObj.currentFloor);
                            if (pass.Count() > 0)
                            {
                                for (int i = pass.Count() - 1; i >= 0; i--)
                                {
                                    Debug.WriteLine($"Passenger {pass[i].passengerNo} leaving elevator");
                                    passengersInElevator.Remove(pass[i]);
                                }
                            }
                        }
                        if (passengersInQueue.Count() == 0 || passengersInElevator.Count() == 0)
                            break;
                        await Task.Delay(2000);
                        elevatorObj.currentFloor++;
                        IncrementPassengerTime();
                    }
                }
                elevatorActivated = false;
            }
        }

        private async void ElevatorDown()
        {
            if (!elevatorActivated)
            {
                elevatorActivated = true;

                if (elevatorObj.direction == "down")
                {
                    //passengersWaiting.Sort((x, y) => x.enterFloor.CompareTo(y.enterFloor));
                    for (int i = 0; i < passengersWaiting.Count(); i++)
                    {
                        if (passengersWaiting[i].enterFloor >= elevatorObj.currentFloor &&
                            Convert.ToInt32(passengersWaiting[i].exitFloor) > Convert.ToInt32(passengersWaiting[i].enterFloor))
                        {
                            passengersInQueue.Add(passengersWaiting[i]);
                            passengersWaiting.RemoveAt(i);
                            i--;
                        }
                    }
                    while (passengersInQueue.Count() != 0 || passengersInElevator.Count() != 0)
                    {
                        if (passengersInQueue.Count() > 0)
                        {
                            //if (passengersInQueue[0].enterFloor == elevatorObj.currentFloor)
                            List<Passengers> pass = passengersInQueue.FindAll(u => u.enterFloor == elevatorObj.currentFloor);
                            if (pass.Count() > 0)
                            {
                                for (int i = pass.Count() - 1; i >= 0; i--)
                                {
                                    passengersInElevator.Add(pass[i]);
                                    Debug.WriteLine($"Passenger {pass[i].passengerNo} entering elevator");
                                    passengersInQueue.Remove(pass[i]);
                                }
                            }
                        }
                        if (passengersInElevator.Count() > 0)
                        {
                            List<Passengers> pass = passengersInElevator.FindAll(u => u.exitFloor == elevatorObj.currentFloor);
                            if (pass.Count() > 0)
                            {
                                for (int i = pass.Count() - 1; i >= 0; i--)
                                {
                                    Debug.WriteLine($"Passenger {pass[i].passengerNo} leaving elevator");
                                    passengersInElevator.Remove(pass[i]);
                                }
                            }
                        }
                        if (passengersInQueue.Count() == 0 || passengersInElevator.Count() == 0)
                            break;
                        await Task.Delay(2000);
                        elevatorObj.currentFloor++;
                    }
                }
                elevatorActivated = false;
            }
        }

        private void IncrementPassengerTime()
        {
            foreach (Passengers passenger in passengersInQueue)
                passenger.waitTime += 2;
            foreach (Passengers passenger in passengersWaiting)
                passenger.waitTime += 2;
            foreach (Passengers passenger in passengersInElevator)
                passenger.inElevatorTime += 2;
        }
        #endregion
    }


}
