using Mogo.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.Util
{
    public class MogoCountDown
    {
        readonly static public int SECONDS_DAY = 86400;
        readonly static public int SECONDS_HOUR = 3600;
        readonly static public int SECONDS_MINUTE = 60;

        public enum TimeStringType
        {
            Full,
            UpToMonth,
            UpToDay,
            Date,
            UpToHour, // 时：分：秒
            UpToMinutes, // 分：秒
            UpToSecond, // 秒
            UpToDayHour,// 大于1天显示为【剩余xx天xx小时】，少于1天则显示为【剩余xx小时】
        }

        private UILabel label;

        /// <summary>
        /// days:天数；hours:小时数；minutes:分钟数；seconds:秒数
        /// </summary>
        private int days;
        private int hours;
        private int minutes;
        private int seconds;

        private string countingText;
        private string stopText;
        private string endText;

        private bool stillCountingDown = true;
        private Action endAction;

        private TimeStringType timeType;

        private string yearMonthSplitSign = "/";
        private string monthDaySplitSign = "/";
        private string dayHourSplitSign = " ";

        private string hourMinuteSplitSign = ":";
        private string minuteSecondSplitSign = ":";
        private string secondSign = "";

        private bool hasRelease = false;

        #region 构造函数和析构函数

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, Action theEndAction = null)
        {
            label = theLabel;

            countingText = "";
            stopText = "";
            endText = "";

            days = 0;
            hours = 0;
            minutes = 0;
            seconds = 0;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = TimeStringType.UpToMinutes; 
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="theHours"></param>
        /// <param name="theMinutes"></param>
        /// <param name="theSeconds"></param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, int theHours, int theMinutes, int theSeconds, Action theEndAction = null)
        {
            label = theLabel;

            countingText = "";
            stopText = "";
            endText = "";

            days = 0;
            hours = theHours;
            minutes = theMinutes;
            seconds = theSeconds;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = TimeStringType.UpToMinutes; 
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="theHours"></param>
        /// <param name="theMinutes"></param>
        /// <param name="theSeconds"></param>
        /// <param name="theCountingText"></param>
        /// <param name="theStopText"></param>
        /// <param name="theEndText"></param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, int theHours, int theMinutes, int theSeconds, string theCountingText, string theStopText, string theEndText, Action theEndAction = null)
        {
            label = theLabel;

            countingText = theCountingText;
            stopText = theStopText;
            endText = theEndText;

            days = 0;
            hours = theHours;
            minutes = theMinutes;
            seconds = theSeconds;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = TimeStringType.UpToMinutes; 
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="theHours">小时数</param>
        /// <param name="theMinutes">分钟数</param>
        /// <param name="theSeconds">秒数</param>
        /// <param name="theCountingText"></param>
        /// <param name="theStopText"></param>
        /// <param name="theEndText"></param>
        /// <param name="theTimeStringType"></param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, int theHours, int theMinutes, int theSeconds, string theCountingText, string theStopText, string theEndText, TimeStringType theTimeStringType, Action theEndAction = null)
        {
            label = theLabel;

            countingText = theCountingText;
            stopText = theStopText;
            endText = theEndText;

            days = 0;
            hours = theHours;
            minutes = theMinutes;
            seconds = theSeconds;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = theTimeStringType; 
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="theDays">天数</param>
        /// <param name="theHours">小时数</param>
        /// <param name="theMinutes">分钟数</param>
        /// <param name="theSeconds">描述</param>
        /// <param name="theCountingText"></param>
        /// <param name="theStopText"></param>
        /// <param name="theEndText"></param>
        /// <param name="theTimeStringType">显示类型</param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, int theDays, int theHours, int theMinutes, int theSeconds, string theCountingText, string theStopText, string theEndText, TimeStringType theTimeStringType, Action theEndAction = null)
        {
            label = theLabel;

            countingText = theCountingText;
            stopText = theStopText;
            endText = theEndText;

            days = theDays;
            hours = theHours;
            minutes = theMinutes;
            seconds = theSeconds;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = theTimeStringType;
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD构造函数
        /// </summary>
        /// <param name="theLabel"></param>
        /// <param name="secondsNum">CD秒数</param>
        /// <param name="theCountingText"></param>
        /// <param name="theStopText"></param>
        /// <param name="theEndText"></param>
        /// <param name="theTimeStringType"></param>
        /// <param name="theEndAction">CD结束时回调</param>
        public MogoCountDown(UILabel theLabel, int secondsNum, string theCountingText, string theStopText, string theEndText, TimeStringType theTimeStringType, Action theEndAction = null)
        {
            label = theLabel;

            countingText = theCountingText;
            stopText = theStopText;
            endText = theEndText;

            days = secondsNum / SECONDS_DAY;
            hours = secondsNum % SECONDS_DAY / SECONDS_HOUR;
            minutes = secondsNum % SECONDS_HOUR / SECONDS_MINUTE;
            seconds = secondsNum % SECONDS_MINUTE;
            stillCountingDown = true;

            endAction = theEndAction;

            timeType = theTimeStringType;
            label.text = FormatTime();

            EventDispatcher.AddEventListener(Events.OtherEvent.SecondPast, CountDown);
        }

        /// <summary>
        /// CD析构函数
        /// </summary>
        ~MogoCountDown()
        {
            if (!hasRelease)
                Release();
        }

        #endregion

        #region 倒计时

        public void Release()
        {
            if (stillCountingDown)
                EventDispatcher.RemoveEventListener(Events.OtherEvent.SecondPast, CountDown);
            hasRelease = true;
        }

        /// <summary>
        /// 倒计时
        /// </summary>
        public void CountDown()
        {
            if (stillCountingDown)
            {
                seconds--;
                if (seconds < 0)
                {
                    seconds = 59;
                    minutes--;
                    if (minutes < 0)
                    {
                        minutes = 59;
                        hours--;
                        if (hours < 0)
                        {
                            days--;
                            if (days < 0)
                            {
                                stillCountingDown = false;
                                EventDispatcher.RemoveEventListener(Events.OtherEvent.SecondPast, CountDown);
                                label.text = endText;
                                if (endAction != null)
                                    endAction();
                                return;
                            }
                            else
                            {
                                hours = 23;
                            }
                        }
                    }
                }
            }
            label.text = FormatTime();
        }

        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            stillCountingDown = false;
            label.text = stopText;
        }
        
        /// <summary>
        /// 倒计时结束
        /// </summary>
        public void EndCountDown()
        {
            stillCountingDown = false;
            EventDispatcher.RemoveEventListener(Events.OtherEvent.SecondPast, CountDown);
            label.text = endText;
        }

        /// <summary>
        /// 获取倒计时剩余秒数
        /// </summary>
        /// <returns></returns>
        public int GetLastSeconds()
        {
            int lastSeconds = 0;

            if (days > 0)
                lastSeconds += days * SECONDS_DAY;
            if(hours > 0)
                lastSeconds += hours * SECONDS_HOUR;
            if(minutes > 0)
                lastSeconds += minutes * SECONDS_MINUTE;
            if (seconds > 0)
                lastSeconds += seconds;

            return lastSeconds;
        }


        #endregion

        #region 修改文字

        public void SetSplitSign(string splitSign)
        {
            yearMonthSplitSign = splitSign;
            monthDaySplitSign = splitSign;
            dayHourSplitSign = splitSign;

            hourMinuteSplitSign = splitSign;
            minuteSecondSplitSign = splitSign;
        }

        public void SetSplitSign(string theHourMinuteSplitSign, string theMinuteSecondSplitSign)
        {
            hourMinuteSplitSign = theHourMinuteSplitSign;
            minuteSecondSplitSign = theMinuteSecondSplitSign;
        }

        public void SetSplitSign(string theYearMonthSplitSign, string theMonthDaySplitSign, string theDayHourSplitSign, string theHourMinuteSplitSign, string theMinuteSecondSplitSign, string theSecondSign)
        {
            yearMonthSplitSign = theYearMonthSplitSign;
            monthDaySplitSign = theMonthDaySplitSign;
            dayHourSplitSign = theDayHourSplitSign;

            hourMinuteSplitSign = theHourMinuteSplitSign;
            minuteSecondSplitSign = theMinuteSecondSplitSign;
            secondSign = theSecondSign;
        }

        public void UpdateCountingText(string newCountingText, bool isFlushNow = false)
        {
            countingText = newCountingText;
            if (isFlushNow)
                FormatTime();
        }

        public void UpdateStopText(string newStopText, bool isFlushNow = false)
        {
            stopText = newStopText;
            if (isFlushNow)
                StopCountDown();
        }

        public void UpdateEndText(string newEndText, bool isFlushNow = false)
        {
            endText = newEndText;
            if (isFlushNow)
                EndCountDown();
        }

        private string FormatTime()
        {
            string hoursString = String.Empty;
            string minutesString = String.Empty;
            string secondsString = String.Empty;

            if (hours < 10)
                hoursString = String.Concat("0", hours.ToString());
            else
                hoursString = hours.ToString();

            if (minutes < 10)
                minutesString = String.Concat("0", minutes.ToString());
            else
                minutesString = minutes.ToString();

            if (seconds < 10)
                secondsString = String.Concat("0", seconds.ToString());
            else
                secondsString = seconds.ToString();

            switch (timeType)
            {
                case TimeStringType.Full:
                    return String.Empty;

                case TimeStringType.UpToMonth:
                    return String.Empty;

                case TimeStringType.UpToDay:
                    StringBuilder sb = new StringBuilder();
                    if (days > 0)
                    {
                        sb.Append(countingText).Append(days).Append(dayHourSplitSign).Append(hoursString).Append(hourMinuteSplitSign).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).Append(secondSign);
                    }
                    else if (hours > 0)
                    {
                        sb.Append(countingText).Append(hoursString).Append(hourMinuteSplitSign).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).Append(secondSign);
                    }
                    else if (minutes > 0)
                    {
                        sb.Append(countingText).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).Append(secondSign);
                    }
                    else if (seconds > 0)
                    {
                        sb.Append(countingText).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).Append(secondSign);
                    }
                    return sb.ToString();

                case TimeStringType.Date:
                    return String.Empty;

                case TimeStringType.UpToHour:
                    return new StringBuilder().Append(countingText).Append(hoursString).Append(hourMinuteSplitSign).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).ToString();

                case TimeStringType.UpToMinutes:
                    return new StringBuilder().Append(countingText).Append(minutesString).Append(minuteSecondSplitSign).Append(secondsString).ToString();

                case TimeStringType.UpToSecond:
                    return new StringBuilder().Append(seconds).ToString();

                case TimeStringType.UpToDayHour:
                    {
                        string formatTime = countingText;
                        if (days > 0)
                        {
                            formatTime += string.Format(LanguageData.GetContent(47602), days, hours + 1);
                            return formatTime;
                        }
                        else
                        {
                            formatTime += string.Format(LanguageData.GetContent(47603), hours + 1);
                            return formatTime;
                        }
                    }

                default:
                    return String.Empty;
            }
        }

        #endregion
    }
}
