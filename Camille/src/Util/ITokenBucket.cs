using System;
using System.Collections.Generic;
using System.Text;

namespace MingweiSamuel.Camille.Util
{
    /// <summary>
    /// This interface defines a token bucket system. One instance represents one recurring bucket with a certain
    /// limit of tokens per timespan.
    /// </summary>
    public interface ITokenBucket
    {
        /// <summary>
        /// Get the delay til the next available token, or -1 if a token is available.
        /// </summary>
        /// <returns>Delay in ticks or -1</returns>
        long GetDelay();

        /// <summary>
        /// Gets n tokens, regardless of whether they are available.
        /// </summary>
        /// <param name="n">Number of tokens to take.</param>
        /// <returns>True if the tokens were obtained without violating limits, false otherwise.</returns>
        bool GetTokens(int n);

        /// <summary>
        /// Get the timespan of this bucket in ticks.
        /// </summary>
        /// <returns>Timespan of the bucket.</returns>
        long GetTickSpan();

        /// <summary>
        /// Get the total limit of this bucket per timespan.
        /// </summary>
        /// <returns>Total limit per timespan.</returns>
        int GetTotalLimit();
    }
}
