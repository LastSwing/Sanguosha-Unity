using System;
using System.Data;
using CommonClassLibrary;

namespace CommonClassLibrary
{
    public class MessageEventArgs : EventArgs
    {
        private string msg;
        private int hallNumber;
        private int deskNumber;

        public string Message
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
            }
        }

        public int HallNumber
        {
            get
            {
                return hallNumber;
            }
            set
            {
                hallNumber = value;
            }
        }

        public int DeskNumber
        {
            get
            {
                return deskNumber;
            }
            set
            {
                deskNumber = value;
            }
        }
    }

    //public class JoinHallEventArgs : EventArgs
    //{
    //    private JoinHall getObject;

    //    public JoinHall GetObject
    //    {
    //        get
    //        {
    //            return getObject;
    //        }
    //        set
    //        {
    //            getObject = value;
    //        }
    //    }
    //}

    //public class JoinDeskEventArgs : EventArgs
    //{
    //    private JoinDesk getObject;

    //    public JoinDesk GetObject
    //    {
    //        get
    //        {
    //            return getObject;
    //        }
    //        set
    //        {
    //            getObject = value;
    //        }
    //    }
    //}

    //public class LeaveDeskEventArgs : EventArgs
    //{
    //    private LeaveDesk getObject;

    //    public LeaveDesk GetObject
    //    {
    //        get
    //        {
    //            return getObject;
    //        }
    //        set
    //        {
    //            getObject = value;
    //        }
    //    }
    //}

    //public class GameOverEventArgs : EventArgs
    //{
    //    private GameOver getObject;

    //    public GameOver GetObject
    //    {
    //        get
    //        {
    //            return getObject;
    //        }
    //        set
    //        {
    //            getObject = value;
    //        }
    //    }
    //}
}

