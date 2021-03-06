﻿using System;
using System.Diagnostics;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// Value-type replacement for <see cref="Stopwatch"/> which avoids allocations.
    /// </summary>
    /// <remarks>
    /// Inspired on <seealso href="https://github.com/dotnet/extensions/blob/master/src/Shared/src/ValueStopwatch/ValueStopwatch.cs"/>.
    /// </remarks>
    public struct ValueStopwatch
    {
        private static readonly double _timestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private long _startTimestamp;

        private ValueStopwatch(long startTimestamp)
        {
            _startTimestamp = startTimestamp;
        }

        /// <summary>
        /// Gets the time elapsed since the stopwatch was created with <see cref="StartNew"/>.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                // Start timestamp can't be zero in an initialized ValueStopwatch. It would have to be literally the first thing executed when the machine boots to be 0.
                // So it being 0 is a clear indication of default(ValueStopwatch)
                if (_startTimestamp == 0)
                {
                    throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
                }

                var end = Stopwatch.GetTimestamp();
                var timestampDelta = end - _startTimestamp;
                var ticks = (long)(_timestampToTicks * timestampDelta);
                return new TimeSpan(ticks);
            }
        }

        public void Restart() => _startTimestamp = Stopwatch.GetTimestamp();

        /// <summary>
        /// Creates a new <see cref="ValueStopwatch"/> that is ready to be used.
        /// </summary>
        public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());
    }
}
