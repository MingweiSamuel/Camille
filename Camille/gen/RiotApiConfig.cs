namespace MingweiSamuel.Camille
{
    public class RiotApiConfig
    {
        /// <summary>Riot Games API key.</summary>
        public readonly string ApiKey;

        /// <summary>Maximum number of concurrent requests allowed.</summary>
        public readonly int MaxConcurrentRequests;

        private ApiConfig(string apiKey, int maxConcurrentRequests)
        {
            ApiKey = apiKey;
            MaxConcurrentRequests = maxConcurrentRequests;
        }

        public class Builder
        {
            /// <summary>Riot Games API key.</summary>
            public string ApiKey = default(string);

            /// <summary>Maximum number of concurrent requests allowed.</summary>
            public int MaxConcurrentRequests = 1000;

            private Builder()
            {}

            public ApiConfig Build()
            {
                return new ApiConfig(ApiKey, MaxConcurrentRequests);
            }
        }
    }
}
