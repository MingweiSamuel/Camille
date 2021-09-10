﻿﻿
// This file is automatically generated.
// Do not directly edit.
// Generated on 2021-09-10T02:14:57.565Z

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Camille.Core;
using Camille.Enums;


#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable IDE0028 // Simplify collection initialization

namespace Camille.LolGame
{
    /// <summary>
    /// LiveClientData endpoints. This class is automatically generated.<para />
    /// Official API Reference <a href="https://developer.riotgames.com/api-methods/#LiveClientData">https://developer.riotgames.com/api-methods/#LiveClientData</a>
    /// </summary>
    public class LiveClientDataEndpoints : Endpoints
    {
        internal LiveClientDataEndpoints(ILolGameApi @base) : base(@base)
        {}

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.ActivePlayer GetActivePlayer(CancellationToken? cancellationToken = null)
        {
            return GetActivePlayerAsync(cancellationToken).Result;
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.ActivePlayer> GetActivePlayerAsync(CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/activeplayer");
            return @base.Send<LiveClientData.ActivePlayer>(request, cancellationToken);
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.ActivePlayerAbilities GetActivePlayerAbilities(CancellationToken? cancellationToken = null)
        {
            return GetActivePlayerAbilitiesAsync(cancellationToken).Result;
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.ActivePlayerAbilities> GetActivePlayerAbilitiesAsync(CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/activeplayerabilities");
            return @base.Send<LiveClientData.ActivePlayerAbilities>(request, cancellationToken);
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetActivePlayerName(CancellationToken? cancellationToken = null)
        {
            return GetActivePlayerNameAsync(cancellationToken).Result;
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<string> GetActivePlayerNameAsync(CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/activeplayername");
            return @base.Send<string>(request, cancellationToken);
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.ActivePlayerRunes GetActivePlayerRunes(CancellationToken? cancellationToken = null)
        {
            return GetActivePlayerRunesAsync(cancellationToken).Result;
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.ActivePlayerRunes> GetActivePlayerRunesAsync(CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/activeplayerrunes");
            return @base.Send<LiveClientData.ActivePlayerRunes>(request, cancellationToken);
        }

        /// <param name="eventID">ID of the next event you expect to see (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.AllGameData GetAllGameData(int? eventID = null, CancellationToken? cancellationToken = null)
        {
            return GetAllGameDataAsync(eventID, cancellationToken).Result;
        }

        /// <param name="eventID">ID of the next event you expect to see (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.AllGameData> GetAllGameDataAsync(int? eventID = null, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            if (null != eventID) queryParams.Add(new KeyValuePair<string?, string?>(nameof(eventID), eventID.Value.ToString()));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/allgamedata?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.AllGameData>(request, cancellationToken);
        }

        /// <param name="eventID">ID of the next event you expect to see (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.EventData GetEventData(int? eventID = null, CancellationToken? cancellationToken = null)
        {
            return GetEventDataAsync(eventID, cancellationToken).Result;
        }

        /// <param name="eventID">ID of the next event you expect to see (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.EventData> GetEventDataAsync(int? eventID = null, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            if (null != eventID) queryParams.Add(new KeyValuePair<string?, string?>(nameof(eventID), eventID.Value.ToString()));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/eventdata?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.EventData>(request, cancellationToken);
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.GameStats GetGameStats(CancellationToken? cancellationToken = null)
        {
            return GetGameStatsAsync(cancellationToken).Result;
        }

        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.GameStats> GetGameStatsAsync(CancellationToken? cancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/gamestats");
            return @base.Send<LiveClientData.GameStats>(request, cancellationToken);
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.PlayerItem[] GetPlayerItems(string summonerName, CancellationToken? cancellationToken = null)
        {
            return GetPlayerItemsAsync(summonerName, cancellationToken).Result;
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.PlayerItem[]> GetPlayerItemsAsync(string summonerName, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            queryParams.Add(new KeyValuePair<string?, string?>(nameof(summonerName), summonerName));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/playeritems?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.PlayerItem[]>(request, cancellationToken);
        }

        /// <param name="teamID">Heroes team ID. Optional, returns all players on all teams if null.  (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.Player[] GetPlayerList(string? teamID = null, CancellationToken? cancellationToken = null)
        {
            return GetPlayerListAsync(teamID, cancellationToken).Result;
        }

        /// <param name="teamID">Heroes team ID. Optional, returns all players on all teams if null.  (optional, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.Player[]> GetPlayerListAsync(string? teamID = null, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            if (null != teamID) queryParams.Add(new KeyValuePair<string?, string?>(nameof(teamID), teamID));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/playerlist?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.Player[]>(request, cancellationToken);
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.PlayerRunes GetPlayerMainRunes(string summonerName, CancellationToken? cancellationToken = null)
        {
            return GetPlayerMainRunesAsync(summonerName, cancellationToken).Result;
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.PlayerRunes> GetPlayerMainRunesAsync(string summonerName, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            queryParams.Add(new KeyValuePair<string?, string?>(nameof(summonerName), summonerName));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/playermainrunes?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.PlayerRunes>(request, cancellationToken);
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.PlayerScores GetPlayerScores(string summonerName, CancellationToken? cancellationToken = null)
        {
            return GetPlayerScoresAsync(summonerName, cancellationToken).Result;
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.PlayerScores> GetPlayerScoresAsync(string summonerName, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            queryParams.Add(new KeyValuePair<string?, string?>(nameof(summonerName), summonerName));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/playerscores?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.PlayerScores>(request, cancellationToken);
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LiveClientData.PlayerSummonerSpells GetPlayerSummonerSpells(string summonerName, CancellationToken? cancellationToken = null)
        {
            return GetPlayerSummonerSpellsAsync(summonerName, cancellationToken).Result;
        }

        /// <param name="summonerName">Summoner name of player (required, in query)</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel this task. (optional)</param>
        public Task<LiveClientData.PlayerSummonerSpells> GetPlayerSummonerSpellsAsync(string summonerName, CancellationToken? cancellationToken = null)
        {
            var queryParams = new List<KeyValuePair<string?, string?>>();
            queryParams.Add(new KeyValuePair<string?, string?>(nameof(summonerName), summonerName));
            HttpRequestMessage request;
            using (var content = new FormUrlEncodedContent(queryParams))
                request = new HttpRequestMessage(HttpMethod.Get, $"/liveclientdata/playersummonerspells?{content.ReadAsStringAsync().Result}");
            return @base.Send<LiveClientData.PlayerSummonerSpells>(request, cancellationToken);
        }

    }
}

#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore IDE0017 // Simplify object initialization

