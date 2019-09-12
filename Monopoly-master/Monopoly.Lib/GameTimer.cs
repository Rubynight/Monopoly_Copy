using System;
using System.Collections.Generic;
using System.Timers;

namespace Monopoly.Lib
{
    public class GameTimer
    {
        public delegate void TickerMethod(object sender, EventArgs e);
        private Timer timer;

        /*
         * Sets up Timer that runs every second.
         */
        public GameTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
        }

        /*
         * Sets up Timer that runs on a interval
         * determined by the user.
         */
        public GameTimer(int milliseconds)
        {
            timer = new System.Timers.Timer();
            timer.Interval = milliseconds;
        }

        /*
         * Runs one method on the timer. 
         * Method must have the parameters (object sender, EventArgs e).
         * Determined by the delegate
         */
        public void SetupGameTimer(TickerMethod tickerMethod)
        {
            timer.Elapsed += new ElapsedEventHandler(tickerMethod);
            timer.Enabled = true;
            timer.Start();
        }


        /*
         * Runs a list of methods on the timer. 
         * Methods must have the parameters (object sender, EventArgs e).
         * Determined by the delegate
         */
        public void SetupGameTimer(List<TickerMethod> tickerMethods)
        {
            foreach (TickerMethod tickerMethod in tickerMethods)
                timer.Elapsed += new ElapsedEventHandler(tickerMethod);

            timer.Enabled = true;
            timer.Start();
        }
    }
}
