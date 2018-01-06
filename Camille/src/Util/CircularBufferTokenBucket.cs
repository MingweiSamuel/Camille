using System;
using System.Runtime.CompilerServices;

namespace MingweiSamuel.Camille.Util
{
    /// <inheritDoc/>
    /// <summary>
    /// <p>A circular buffer keeps track of tokens. The value of each buffer index represents the number of requests
    /// sent during that time period and as time passes, old indices are zeroed and the current index advances.The
    /// entire length of the buffer minus one represents a entire timespan (each index represents a fraction of the
    /// total timespan). The extra index prevents violations due to bucket misalignment.A rolling total is kept of
    /// the buffer's contents.</p>
    ///
    /// <p>When trying to obtain a token, we first check the rolling total is less than the limit.If so, we obtain
    /// a token by incrementing the rolling total and incrementing the buffer's current index.</p>
    ///
    /// <p>The length of the buffer is one more than the temporal factor supplied to the constructor. The temporal
    /// factor represents the multiplicative increase in temporal resolution provided with more buffer indices.</p>
    ///
    /// <p>Additionally, a non-zero spreading factor can be provided to prevent a single index from supplying all of
    /// a timespan's tokens. A spreading factor of 0.0 means no spreading, a factor of 0.5 means each index can
    /// supply up to half of the tokens, and a factor of 1.0 means tokens will be evenly supplied by all indices
    /// (provided there is enough demand).</p>
    ///
    /// <p>Checking the availability of tokens is done using the {@link #getDelay()} method. Tokens are obtained
    /// using the {@link #getToken()} method. Both these methods are synchronized on the bucket instance. Because
    /// the state of the bucket may change if there are multiple threads, it is best to call these methods in a
    /// synchronized block, as shown below.</p>
    ///
    /// <code>
    /// ITokenBucket bucket = ...;
    /// while (true)
    /// {
    ///     long delay;
    ///     synchronized(bucket) {
    ///         delay = bucket.getDelay();
    ///         if (delay == -1)
    ///         {
    ///             bucket.GetTokens(1);
    ///             break;
    ///         }
    ///     }
    ///     // Waiting is done outside of the synchronized block.
    ///     Thread.sleep(delay);
    /// }
    /// // Token is obtained.
    /// ...
    /// </code>
    /// </summary>
    public class CircularBufferTokenBucket : ITokenBucket
    {
        /// <summary>A tick-based time supplier. A simulated supplier can be used for debugging purposes.</summary>
        private readonly Func<long> _tickSupplier;

        /// <summary>The timespan of this bucket.</summary>
        private readonly long _tickSpan;
        /// <summary>The raw number of tokens per timespan, used for the getTotalLimit().</summary>
        private readonly int _totalLimit;

        /// <summary>The maximum number of tokens that can be supplied per timespan.</summary>
        private readonly int _adjustedTotalLimit;
        /// <summary>The maximum number of tokens a single index can supply per timespan.</summary>
        private readonly int _indexLimit;
        /// <summary>The timespan represented by a single index.</summary>
        private readonly long _indexTickSpan;

        /// <summary>
        /// Circular buffer storing the number of tokens supplied by each index, corresponding to a section of the
        /// total time span.
        /// </summary>
        private readonly int[] _buffer;
        /// <summary>The rolling total of tokens supplied.</summary>
        private volatile int _total = 0;
        /// <summary>The tick stamp corresponding to the most recent update of the buffer.</summary>
        private long _tickStamp = -1;

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="timeSpan">Timespan of this bucket.</param>
        /// <param name="totalLimit">Total limit of this bucket.</param>
        /// <param name="temporalFactor">Temporal multiplier corresponding to token time tracking.</param>
        /// <param name="spreadFactor">Factor corresponding to token supply spread (from multiple indices).</param>
        /// <param name="totalLimitFactor">
        ///     Factor to multiply adjustedTotalLimit by to decrease the chance of hitting the rate limit.
        /// </param>
        public CircularBufferTokenBucket(TimeSpan timeSpan, int totalLimit, int temporalFactor, float spreadFactor, float totalLimitFactor)
            : this(timeSpan, totalLimit, temporalFactor, spreadFactor, totalLimitFactor, () => DateTimeOffset.UtcNow.Ticks)
        {}

        /// <summary>
        /// Secondary constructor, mainly for debugging.
        /// </summary>
        /// <param name="timeSpan">Timespan of this bucket.</param>
        /// <param name="totalLimit">Total limit of this bucket.</param>
        /// <param name="temporalFactor">Temporal multiplier corresponding to token time tracking.</param>
        /// <param name="spreadFactor">Factor corresponding to token supply spread (from multiple indices).</param>
        /// <param name="totalLimitFactor">
        ///     Factor to multiply adjustedTotalLimit by to decrease the chance of hitting the rate limit.
        /// </param>
        /// <param name="tickSupplier">Supplies non-descending tick time, useful for debugging.</param>
        public CircularBufferTokenBucket(TimeSpan timeSpan, int totalLimit, int temporalFactor, float spreadFactor,
            float totalLimitFactor, Func<long> tickSupplier)
        {
            _tickSupplier = tickSupplier;

            _tickSpan = timeSpan.Ticks;
            _totalLimit = totalLimit;

            _adjustedTotalLimit = (int) (totalLimit * totalLimitFactor);
            _indexLimit = (int) (totalLimit * totalLimitFactor / spreadFactor / temporalFactor);
            _indexTickSpan = (long) Math.Ceiling(_tickSpan / (double) temporalFactor);

            _buffer = new int[temporalFactor + 1];
        }

        /// <summary>
        /// Gets the delay till next available token, or -1 if available.
        /// </summary>
        /// <returns>Delay in ticks, or -1 if available.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long GetDelay()
        {
            var index = Update();
            if (_total < _adjustedTotalLimit)
            {
                if (_buffer[index] >= _indexLimit)
                    return GetTimeToBucket(1);
                return -1;
            }

            // Check how soon into the future old buckets will be zeroed, making requests available.
            var i = 1;
            for (; i < _buffer.Length; i++)
            {
                if (_buffer[(index + i) % _buffer.Length] > 0)
                    break;
            }
            return GetTimeToBucket(i);
        }


        /// <summary>
        /// Updates the circular buffer and TickStamp to match the passage of time.
        /// </summary>
        /// <returns>The current buffer index.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private int Update()
        {
            // The first time this is called, we initialize the time.
            if (_tickStamp < 0)
            {
                _tickStamp = _tickSupplier.Invoke();
                return GetIndex(_tickStamp);
            }
            var index = GetIndex(_tickStamp);
            var length = GetLength(_tickStamp, (_tickStamp = _tickSupplier.Invoke()));

            if (length < 0)
                throw new InvalidOperationException($"Length should be non-negative: {length}.");
            if (length == 0)
                return index;
            if (length >= _buffer.Length)
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                _total = 0;
                return index;
            }
            for (var i = 0; i < length; i++)
            {
                index++;
                index %= _buffer.Length;
                _total -= _buffer[index];
                _buffer[index] = 0;
            }
            if (GetIndex(_tickStamp) != index)
                throw new InvalidOperationException($"Get index time: {GetIndex(_tickStamp)}, index: {index}.");
            return index;
        }

        /// <summary>
        /// Gets n tokens, regardless of whether they are available.
        /// </summary>
        /// <param name="n">Number of tokens to get.</param>
        /// <returns>True if the tokens were obtained without violating limits, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool GetTokens(int n)
        {
            int index = Update();
            _buffer[index] += n;
            _total += n;
            return _total <= _adjustedTotalLimit && _buffer[index] <= _indexLimit;
        }

        public long GetTickSpan()
        {
            return _tickSpan;
        }

        public int GetTotalLimit()
        {
            return _totalLimit;
        }

        /// <summary>
        /// Gets the circular buffer index corresponding to a particular timestamp
        /// </summary>
        /// <param name="tickStamp">Tick timestamp.</param>
        /// <returns>Buffer index.</returns>
        private int GetIndex(long tickStamp)
        {
            return (int) ((tickStamp / _indexTickSpan) % _buffer.Length);
        }

        /// <summary>
        /// Gets the index distance between two timestamps. Because the buffer is circular, the distance between
        /// indices may be greater than the length of the buffer.
        /// </summary>
        /// <param name="startTickStamp">Start tick timestamp.</param>
        /// <param name="endTickStamp">End tick timestamp.</param>
        /// <returns>The index distance. May be greater than the length of the buffer.</returns>
        private int GetLength(long startTickStamp, long endTickStamp)
        {
            return (int) (endTickStamp / _indexTickSpan - startTickStamp / _indexTickSpan);
        }

        /// <summary>
        /// </summary>
        /// <param name="n">Number of buckets in the future to look (n=1 -> next bucket).</param>
        /// <returns>Time until the next nth bucket in ticks.</returns>
        private long GetTimeToBucket(int n)
        {
            return n * _indexTickSpan - (_tickStamp % _indexTickSpan);
        }
    }
}
