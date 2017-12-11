using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.Util
{
    public class MogoTime
    {
        private static MogoTime m_Instance;
        public static MogoTime Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new MogoTime();
                }
                return MogoTime.m_Instance;
            }
        }

        private int timeZone = 0;
        private DateTime defaultFixServerTime;

        private int second;
        private int escapeSecond;

        private DateTime dateTime;

        private int defalutSyncTime = 3;
        private int curSyncTime = 3;

        private uint timer;
        private uint escapeTimer;

        private MogoTime()
        {
            second = 0;
            escapeSecond = 0;

            defaultFixServerTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // LoggerHelper.Error("defaultFixServerTime: " + defaultFixServerTime.ToString());
            timer = uint.MaxValue;
            escapeTimer = uint.MaxValue;
        }

        // 代替析构函数
        public void ReleaseMogoTimeData()
        {
            TimerHeap.DelTimer(timer);
            TimerHeap.DelTimer(escapeTimer);
        }

        public void InitTimeFromServer()
        {
            GetTimeFromServer();
            GetTimeZoneFromServer();
        }

        public void GetTimeFromServer()
        {
            second++;
            escapeSecond++;

            curSyncTime++;
            dateTime = defaultFixServerTime.AddMinutes(timeZone).AddSeconds(second);
            EventDispatcher.TriggerEvent(Events.OtherEvent.SecondPast);
            if (curSyncTime >= defalutSyncTime)
            {
                curSyncTime = 0;
                MogoWorld.thePlayer.GetServerTimeStampReq();
            }
            timer = TimerHeap.AddTimer(1000, 0, GetTimeFromServer);
        }

        public void GetEscapeTimeFromServer()
        {
            MogoWorld.thePlayer.GetServerTimeEscapeReq();
            escapeTimer = TimerHeap.AddTimer(30000, 0, GetTimeFromServer);
        }

        public void GetTimeZoneFromServer()
        {
            MogoWorld.thePlayer.GetServerTimeZoneReq();
        }
        
        public void SetTimeFromServer(int theSecond)
        {
            second = theSecond;
        }

        public void SetEscapeTimeFromServer(int theSecond)
        {
            escapeSecond = theSecond;
        }

        public void SetTimeZone(int theTimeZone)
        {
            timeZone = 0 - theTimeZone;
        }

        public int GetSecond()
        {
            return second;
        }

        public int GetEscapeSecond()
        {
            return escapeSecond;
        }

        public int GetTimeZone()
        {
            return timeZone;
        }

        public DateTime GetCurrentDateTime()
        {
            return defaultFixServerTime.AddMinutes(timeZone).AddSeconds(second);
        }

        public DateTime GetCurrentEscapeDateTime()
        {
            return defaultFixServerTime.AddMinutes(timeZone).AddSeconds(escapeSecond);
        }

        public DateTime GetDateTimeBySecond(int s)
        {
            return defaultFixServerTime.AddMinutes(timeZone).AddSeconds(s);
        }

        public int CalculateTimeSpanSecond(int endTime)
        {
            return endTime - second;
        }

        //public DateTime CalculateDateTimeSecond(int endTime)
        //{
        //    int temp = endTime - second; 
        //    return new DateTime(0, 0, 0, temp / 3600, (temp % 3600) / 60, (temp % 3600) % 60);
        //}

        public TimeSpan CalculateTimeSpanDateTime(int endTime)
        {
            return defaultFixServerTime.AddMinutes(timeZone).AddSeconds(endTime) - defaultFixServerTime.AddMinutes(timeZone).AddSeconds(second);
        }

        public int CalculateEscapeTimeSpanSecond(int endTime)
        {
            return endTime - escapeSecond;
        }

        public TimeSpan CalculateEscapeTimeSpanDateTime(int endTime)
        {
            return defaultFixServerTime.AddMinutes(timeZone).AddSeconds(endTime) - defaultFixServerTime.AddMinutes(timeZone).AddSeconds(escapeSecond);
        }
    }
}
