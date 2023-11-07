using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGO.ViewModels;
using System.Diagnostics;
using ElevatorSimulator.ViewModels;

namespace ElevatorSimulator.Models
{
    public class Elevator : ObservableObject
    {


        private int m_currentFloor = 1;
        public int currentFloor
        {
            get { return m_currentFloor; }
            set
            {
                if (this.m_currentFloor != value)
                {
                    MainVM.Instance.elevatorMessages.Add(new HelperClass() { Text = ($"Floor {m_currentFloor} to {value}") });
                    this.m_currentFloor = value;
                    this.RaisePropertyChangedEvent("currentFloor");
                }
            }
        }

        private bool m_moving = false;
        public bool moving
        {
            get { return m_moving; }
            set
            {
                if (this.m_moving != value)
                {
                    this.m_moving = value;
                    this.RaisePropertyChangedEvent("moving");
                }
            }
        }

        private string m_direction = "up";
        public string direction
        {
            get { return m_direction; }
            set
            {
                if (this.m_direction != value)
                {
                    this.m_direction = value;
                    this.RaisePropertyChangedEvent("direction");
                }
            }
        }
    }
}
