﻿using System;
using System.Threading;

namespace Marvin.Runtime.Base
{
    /// <summary>
    /// This class can wrap a timer callback in a way to make sure that if the execution 
    /// exceeds the call duration the timer threads do not accumulate.
    /// </summary>
    internal class NonStackingTimerCallback
    {
        #region Fields
        /// <summary>
        /// Flag if callback is currently executed in other thread
        /// </summary>
        private bool _callbackExecuting;

        /// <summary>
        /// Lock used to sync multiple timer threads
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// Callback for timer
        /// </summary>
        private readonly TimerCallback _callback;
        #endregion
        
        #region Constructors
        public NonStackingTimerCallback(Action timerCallback)
        {
            _callback = state => timerCallback();
        }

        public NonStackingTimerCallback(TimerCallback timerCallback)
        {
            _callback = timerCallback;
        }
        #endregion

        /// <summary>
        /// Callback for the threading timer that doesn't accumulate threads if execution time is greater than call period
        /// </summary>
        public TimerCallback Callback
        {
            get
            {
                return NonStackingCall;
            }
        }

        /// <summary>
        /// Non stacking call to the timer callback
        /// </summary>
        /// <param name="state">State object</param>
        private void NonStackingCall(object state)
        {
            // Retrieve lock only to ready execution flag
            lock (_syncLock)
            {
                if(_callbackExecuting)
                    return;
                _callbackExecuting = true;
            }

            // Execute callback only once at a time non stacking
            _callback(state);

            // Get lock again to reset execution flag
            lock (_syncLock)
            {
                _callbackExecuting = false;
            }
        }

        /// <summary>
        /// Implicit cast point to callback property
        /// </summary>
        /// <param name="item">Wrapped callback</param>
        /// <returns>Non stacking timer callback</returns>
        public static implicit operator TimerCallback(NonStackingTimerCallback item)
        {
            return item.Callback;
        }
    }

    public static class TimerCallbackExtension
    {
        public static TimerCallback StackProve(this TimerCallback callback)
        {
            var callbackWrapper = new NonStackingTimerCallback(callback);
            return callbackWrapper.Callback;
        }
    }
}