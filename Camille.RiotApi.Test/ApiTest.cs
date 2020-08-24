using System;
using System.IO;

namespace Camille.RiotApi.Test
{
    public class ApiTest
    {
        protected const string SummonerIdLugnutsK = "SBM8Ubipo4ge2yj7bhEzL7yvV0C9Oc1XA2l6v5okGMA_nCw";
        protected const string SummonerIdMa5tery  = "IbC4uyFEEW3ZkZw6FZF4bViw3P1EynclAcI6-p-vCpI99Ec";
        protected const string SummonerIdC9Sneaky = "ghHSdADqgxKwcRl_vWndx6wKiyZx0xKQv-LOhOcU5LU";

        protected const string AccountIdC9Sneaky  = "ML_CcLT94UUHp1iDvXOXCidfmzzPrk_Jbub1f_INhw";

        protected static readonly RiotApi Api = RiotApi.NewInstance
        (
            new RiotApiConfig.Builder(
                Environment.GetEnvironmentVariable("RGAPI_KEY")
                ?? File.ReadAllText("apikey.txt").Trim())
            {
                MaxConcurrentRequests = 10,
                Retries = 10,
                ConcurrentInstanceFactor = 0.5f, // Since two targets run at the same time.
            }.Build()
        );
    }
}
