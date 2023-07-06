using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindTest.Util
{
    internal class EventTimer
    {
        /// <summary>
        /// Does this timer loop?
        /// </summary>
        public bool Loop { get; init; } = false;

        /// <summary>
        /// Current time since timer started current loop
        /// </summary>
        public float CurrentTime { get; set; } = 0;

        /// <summary>
        /// Time timer is counting towards
        /// </summary>
        public float TargetTime { get; init; }

        /// <summary>
        /// True when timer has reached target
        /// </summary>
        public bool TargetReached { get; private set; }

        public float GetPercentComplete()
        {
            return CurrentTime / TargetTime;
        }

        /// <summary>
        /// Increments the timers current time and returns true when target reached
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns>True if target has been reached this timestep, false otherwise</returns>
        public bool IncreaseTime(float deltaTime)
        {
            if (TargetReached && !Loop) return false;

            CurrentTime += deltaTime;

            TargetReached = CurrentTime >= TargetTime;
            if (TargetReached)
            {
                CurrentTime -= TargetTime;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
