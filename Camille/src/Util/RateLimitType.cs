using System;

namespace MingweiSamuel.Camille.Util
{
    public enum RateLimitType
    {
        Application,
        Method
    }

    public static class RateLimitTypeInfo
    {
        public static string TypeName(this RateLimitType type)
        {
            switch (type)
            {
                case RateLimitType.Application: return "application";
                case RateLimitType.Method: return "method";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static string LimitHeader(this RateLimitType type)
        {
            switch (type)
            {
                case RateLimitType.Application: return "X-App-Rate-Limit";
                case RateLimitType.Method: return "X-Method-Rate-Limit";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static string CountHeader(this RateLimitType type)
        {
            switch (type)
            {
                case RateLimitType.Application: return "X-App-Rate-Limit-Count";
                case RateLimitType.Method: return "X-Method-Rate-Limit-Count";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
