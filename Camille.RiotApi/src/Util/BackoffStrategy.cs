namespace Camille.RiotApi.Util
{
    /// <summary>
    /// A backoff strategy for determining how long to delay when response status is 429 (too many requests)
    /// but have a missing/invalid Retry-After header.
    /// </summary>
    /// <param name="retries">Number of retries (0 if this is the first attempt).</param>
    /// <param name="num429s">Number of previous 429 responses (0 if this is the first 429 response).</param>
    /// <returns>Seconds to delay.</returns>
    public delegate double BackoffStrategy(int retries, int num429s);
}
