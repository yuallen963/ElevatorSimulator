using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using WGO.ViewModels;

namespace ElevatorSimulator.Models
{
    public class Passengers : ObservableObject
    {
        public Passengers(int _passengerNo, int _entertFloor, int _exitFloor)
        {
            passengerNo = _passengerNo;
            exitFloor = _exitFloor;
            enterFloor = _entertFloor;
        }

        private int m_passengerNo = -1;
        public int passengerNo
        {
            get { return m_passengerNo; }
            set
            {
                if (this.m_passengerNo != value)
                {
                    this.m_passengerNo = value;
                    this.RaisePropertyChangedEvent("passengerNo");
                }
            }
        }

        private int m_enterFloor = 0;
        public int enterFloor
        {
            get { return m_enterFloor; }
            set
            {
                if (this.m_enterFloor != value)
                {
                    this.m_enterFloor = value;
                    this.RaisePropertyChangedEvent("enterFloor");
                }
            }
        }

        private int m_exitFloor = 0;
        public int exitFloor
        {
            get { return m_exitFloor; }
            set
            {
                if (this.m_exitFloor != value)
                {
                    this.m_exitFloor = value;
                    this.RaisePropertyChangedEvent("exitFloor");
                }
            }
        }

        private bool m_inElevator = false;
        public bool inElevator
        {
            get { return m_inElevator; }
            set
            {
                if (this.m_inElevator != value)
                {
                    this.m_inElevator = value;
                    this.RaisePropertyChangedEvent("inElevator");
                }
            }
        }

        private int m_waitTime = 0;
        public int waitTime
        {
            get { return m_waitTime; }
            set
            {
                if (this.m_waitTime != value)
                {
                    this.m_waitTime = value;
                    this.RaisePropertyChangedEvent("waitTime");
                }
            }
        }

        private int m_inElevatorTime = 0;
        public int inElevatorTime
        {
            get { return m_inElevatorTime; }
            set
            {
                if (this.m_inElevatorTime != value)
                {
                    this.m_inElevatorTime = value;
                    this.RaisePropertyChangedEvent("inElevatorTime");
                }
            }
        }


    }
}
