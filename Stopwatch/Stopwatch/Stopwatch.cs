using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stopwatch
{
    class Stopwatch
    {
        private bool isStarted; //Used to check if stopwatch is turned on
        private TimeSpan timeToShow; //Used to store calculated time to show on list
        private TimeSpan startTime; //Used to store elapsed time

        //Properties
        public bool IsStarted { get => isStarted; set => isStarted = value; }
        public TimeSpan TimeToShow { get => timeToShow; set => timeToShow = value; }
        public TimeSpan StartTime { get => startTime; set => startTime = value; }

        public Stopwatch() //Constructor setting stopwatch state to turned off
        {
            this.IsStarted = false;
        }

        public void Start() //Start the stopwatch - set time, check for errors
        {
            if (this.IsStarted)
            {
                InvalidOperationException error = new InvalidOperationException();
                throw error;
            }
            else
            {
                this.IsStarted = true;
                this.StartTime = DateTime.Now.TimeOfDay;
            }
        }

        public void Stop() //Stop the stopwatch - calculate elapsed time, add to previous times
        {
            if (this.IsStarted)
            {
                this.IsStarted = false;
                this.TimeToShow += (DateTime.Now.TimeOfDay - this.StartTime);
            }
        }

        public void Reset() //Reset the stopwatch - set all values to zero
        {
            this.StartTime = TimeSpan.Zero;
            this.TimeToShow = TimeSpan.Zero;
            this.IsStarted = false;
        }
    }
}
