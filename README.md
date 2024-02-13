# Camille

[![Github Actions](https://img.shields.io/github/workflow/status/MingweiSamuel/Camille/CI/release/3.x.x?label=release/3.x.x&logo=github&style=flat-square)](https://github.com/MingweiSamuel/Camille/actions?query=workflow%3ACI+branch%3Arelease%2F3.x.x) [![NuGet Stable](https://img.shields.io/nuget/v/Camille.RiotGames.svg?style=flat-square)](https://www.nuget.org/packages/Camille.RiotGames/) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Camille.RiotGames.svg?style=flat-square)](https://www.nuget.org/packages/Camille.RiotGames/absoluteLatest) [![API Reference](https://img.shields.io/badge/docfx-Camille-brightgreen.svg?style=flat-square)](http://www.mingweisamuel.com/Camille/)

C# Library for the [Riot Games API](https://developer.riotgames.com/)

Camille's goals are _speed_, _reliability_, and _maintainability_. Camille handles rate limits and large requests with ease.
Data classes are automatically generated from the
[Riot API Reference](https://developer.riotgames.com/api-methods/) ([Swagger](http://www.mingweisamuel.com/riotapi-schema/tool/)).

## Features

* Fast, asynchronous, thread-safe.
* Automatically retries failed requests.
* Automatic up-to-date nightlies, reflecting Riot API changes within 24 hours.
* Multi-targeted: .NET Standard 1.1+, .NET Framework 4.5+, .NET Core 2.0, .NET Standard 2.1+, .NET Core 3.0+.
* Highly-configurable.
* Riot API LOL-V4, TFT, LOR support.
* C# 8 nullable support.

## Installation

[Install via NuGet](https://www.nuget.org/packages?sortby=created-desc&q=Camille&prerel=True) (`Camille.RiotGames`). 

Type the following in the package manager console:

    Install-Package Camille.RiotGames -IncludePrerelease
    
Or use the .NET CLI:

    dotnet add package Camille.RiotGames --prerelease

## Usage

All API interactions are done using a `RiotApi` instance.
`RiotApi.NewInstance` takes either just an API key (for default settings) or a `IRiotApiConfig` instance (for custom settings).

```c#
using Camille.Enums;
using Camille.RiotGames;
```

```c#
var riotApi = RiotGamesApi.NewInstance("RGAPI-12345678-abcd-1234-abcd-123456abcdef");
// OR
var riotApi = RiotGamesApi.NewInstance(
    new RiotGamesApiConfig.Builder("RGAPI-12345678-abcd-1234-abcd-123456abcdef")
    {
        MaxConcurrentRequests = 200,
        Retries = 10,
        // ...
    }.Build()
);
// OR with a proxy server
var riotApi = RiotGamesApi.NewInstance(
    new RiotGamesApiConfig.Builder("")
    {
        // ...
        ApiURL = "proxy.myexample.app",
    }.Build()
);
// Which will send requests like https://proxy.myexample.app/lol/summoner/v4/summoners/by-name/NAME?region=NA1
```
You can find all configuration options [here](https://github.com/MingweiSamuel/Camille/blob/gh-pages/v/3.x.x/_gen/Camille.RiotGames/RiotGamesApiConfig.cs).

API methods are divided up into respective endpoints, corresponding to the left bar of the [API reference](https://developer.riotgames.com/api-methods/).

### Example

#### Print Summoner's Top Champions

```c#
// Get summoners by name synchronously. (Note: async is faster as it allows simultaneous requests).
var summoners = new[]
{
    riotApi.SummonerV4().GetBySummonerName(PlatformRoute.NA1, "jAnna kendrick"),
    riotApi.SummonerV4().GetBySummonerName(PlatformRoute.NA1, "lug nuts k")
};

foreach (var summoner in summoners)
{
    Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

    var masteries =
        riotApi.ChampionMasteryV4().GetAllChampionMasteriesByPUUID(PlatformRoute.NA1, summoner.Puuid);

    for (var i = 0; i < 10; i++)
    {
        var mastery = masteries[i];
        // Get champion for this mastery.
        var champ = (Champion) mastery.ChampionId;
        // print i, champ id, champ mastery points, and champ level
        Console.WriteLine("{0,3}) {1,-16} {2,10:N0} ({3})", i + 1, champ.ToString(),
            mastery.ChampionPoints, mastery.ChampionLevel);
    }
    Console.WriteLine();
}
```

Output (2022-01-17):
```
Janna Kendrick's Top 10 Champs:
  1) EKKO              1,803,701 (7)
  2) PYKE                266,410 (7)
  3) SYLAS               193,439 (7)
  4) MASTERYI            134,140 (7)
  5) MORDEKAISER         127,937 (7)
  6) YASUO                93,904 (7)
  7) VIEGO                88,267 (7)
  8) AHRI                 82,106 (7)
  9) JINX                 76,788 (6)
 10) POPPY                74,982 (7)
    
LugnutsK's Top 10 Champs:
  1) ZYRA                841,132 (7)
  2) SORAKA               81,793 (6)
  3) MORGANA              62,917 (5)
  4) SONA                 55,073 (6)
  5) NAMI                 49,917 (6)
  6) JANNA                46,563 (5)
  7) BRAND                46,401 (5)
  8) TARIC                41,302 (6)
  9) EKKO                 40,245 (5)
 10) POPPY                33,859 (5)
 ```

 <!--
 #### Print Summoner Ranked Match History

 This example takes advantage of C#'s `async`/`await` tasks to fetch 10 matches all at once.

 ```c#
var summonerNameQuery = "lugnutsk";

// Get summoners data (blocking).
var summonerData = await riotApi.SummonerV4().GetBySummonerNameAsync(Region.NA, summonerNameQuery);
if (null == summonerData)
{
    // If a summoner is not found, the response will be null.
    Console.WriteLine($"Summoner '{summonerNameQuery}' not found.");
    return;
}

Console.WriteLine($"Match history for {summonerData.Name}:");

// Get 10 most recent matches (blocking).
// Queue ID 420 is RANKED_SOLO_5v5 (TODO)
var matchlist = await riotApi.MatchV4().GetMatchlistAsync(
    Region.NA, summonerData.AccountId, queue: new[] { 420 }, endIndex: 10);
// Get match results (done asynchronously -> not blocking -> fast).
var matchDataTasks = matchlist.Matches.Select(
        matchMetadata => riotApi.MatchV4().GetMatchAsync(Region.NA, matchMetadata.GameId)
    ).ToArray();
// Wait for all task requests to complete asynchronously.
var matchDatas = await Task.WhenAll(matchDataTasks);

for (var i = 0; i < matchDatas.Count(); i++)
{
    var matchData = matchDatas[i];
    // Get this summoner's participant ID info.
    var participantIdData = matchData.ParticipantIdentities
        .First(pi => summonerData.Id.Equals(pi.Player.SummonerId));
    // Find the corresponding participant.
    var participant = matchData.Participants
        .First(p => p.ParticipantId == participantIdData.ParticipantId);

    var win = participant.Stats.Win;
    var champ = (Champion) participant.ChampionId;
    var k = participant.Stats.Kills;
    var d = participant.Stats.Deaths;
    var a = participant.Stats.Assists;
    var kda = (k + a) / (float) d;

    // Print #, win/loss, champion.
    Console.WriteLine("{0,3}) {1,-4} ({2})", i + 1, win ? "Win" : "Loss", champ.Name());
    // Print champion, K/D/A
    Console.WriteLine("     K/D/A {0}/{1}/{2} ({3:0.00})", k, d, a, kda);
}
```

Output (2019-02-19):
```
Match history for LugnutsK:
  1) Win  (Zyra)
     K/D/A 2/3/11 (4.33)
  2) Win  (Zyra)
     K/D/A 5/1/13 (18.00)
  3) Loss (Zyra)
     K/D/A 2/5/1 (0.60)
  4) Win  (Sona)
     K/D/A 1/13/23 (1.85)
  5) Win  (Zyra)
     K/D/A 3/1/5 (8.00)
  6) Win  (Zyra)
     K/D/A 6/3/16 (7.33)
  7) Win  (Zyra)
     K/D/A 2/4/7 (2.25)
  8) Loss (Zyra)
     K/D/A 1/10/8 (0.90)
  9) Loss (Zyra)
     K/D/A 0/11/5 (0.45)
 10) Win  (Zyra)
     K/D/A 4/5/15 (3.80)
```
-->

### API Reference

[Automatically generated API Reference](http://www.mingweisamuel.com/Camille/)

## Source Code

Projects are located in [`src/`](https://github.com/MingweiSamuel/Camille/tree/release/3.x.x/src).

### Generated Classes

The majority of the code in Camille is automatically generated. The generated sources are not commited to
the master branch, but can be viewed [in the `gh-pages` branch](https://github.com/MingweiSamuel/Camille/tree/gh-pages/v/3.x.x/_gen)
or when building locally.

The actual code for generating these classes is in the
[`srcgen` folder](https://github.com/MingweiSamuel/Camille/tree/release/3.x.x/srcgen).
The C#-generating code is in `*.cs.dt` files and is written in NodeJS, using
[doT.js templates](https://olado.github.io/doT/index.html).
