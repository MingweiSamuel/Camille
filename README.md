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

Type the following in the **Package Manager** console:

    Install-Package Camille.RiotGames -IncludePrerelease
    
Or use the **.NET CLI** to reference the **latest nightly build**:

    dotnet add package Camille.RiotGames --version 3.0.0-nightly-*
    
You can also reference the **latest nightly build** using a **PackageReference**:

    <PackageReference Include="Camille.RiotGames" Version="3.0.0-nightly-*" />

## Usage

All API interactions are done using a `RiotApi` instance.
`RiotApi.NewInstance` takes either just an API key (for default settings) or a `IRiotApiConfig` instance (for custom settings).

```c#
var riotApi = RiotApi.NewInstance("RGAPI-12345678-abcd-1234-abcd-123456abcdef");
// OR
var riotApi = RiotApi.NewInstance(
    new RiotApiConfig.Builder("RGAPI-12345678-abcd-1234-abcd-123456abcdef")
    {
        MaxConcurrentRequests = 200,
        Retries = 10,
        // ...
    }.Build()
);
```
You can find all configuration options [here](https://github.com/MingweiSamuel/Camille/blob/gh-pages/_gen/RiotApiConfig.cs#L7-L27).

API methods are divided up into respective endpoints, corresponding to the left bar of the [API reference](https://developer.riotgames.com/api-methods/).

### Example

#### Print Summoner's Top Champions

```c#
// Get summoners by name synchronously. (using async is faster).
var summoners = new[]
{
    riotApi.SummonerV4().GetBySummonerName(Region.NA, "jAnna kendrick"),
    riotApi.SummonerV4().GetBySummonerName(Region.NA, "lug nuts k")
};

foreach (var summoner in summoners)
{
    Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

    var masteries =
        riotApi.ChampionMasteryV4().GetAllChampionMasteries(Region.NA, summoner.Id);

    for (var i = 0; i < 10; i++)
    {
        var mastery = masteries[i];
        // Get champion for this mastery.
        var champ = (Champion) mastery.ChampionId;
        // print i, champ id, champ mastery points, and champ level
        Console.WriteLine("{0,3}) {1,-16} {2,10:N0} ({3})", i + 1, champ.Name(),
            mastery.ChampionPoints, mastery.ChampionLevel);
    }
    Console.WriteLine();
}
```

Output (2019-02-06):
```
Janna Kendrick's Top 10 Champs:
  1) Ekko              1,280,476 (7)
  2) Master Yi            89,871 (7)
  3) Jinx                 59,238 (6)
  4) Yasuo                58,625 (7)
  5) Poppy                52,140 (7)
  6) Maokai               46,567 (6)
  7) Ezreal               44,604 (6)
  8) Lulu                 42,794 (6)
  9) Kennen               42,500 (7)
 10) Zilean               41,710 (6)

LugnutsK's Top 10 Champs:
  1) Zyra                548,939 (7)
  2) Soraka               73,675 (6)
  3) Morgana              59,828 (5)
  4) Sona                 50,001 (6)
  5) Nami                 44,775 (6)
  6) Brand                42,108 (5)
  7) Janna                41,923 (5)
  8) Taric                37,916 (6)
  9) Ekko                 35,837 (5)
 10) Poppy                31,457 (5)
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

Source code is located in the
[`src/` directory](https://github.com/MingweiSamuel/Camille/tree/master/Camille.RiotGames/src), corresponding
to the `MingweiSamuel.Camille` namespace.

[`RiotApi.cs`](https://github.com/MingweiSamuel/Camille/blob/master/Camille.RiotGames/src/RiotApi.cs) is the main
point of entry for interfacing with Camille, however this source file is only a partial class. The remainder
of the class is automatically generated and not commited to the `master` branch, but is viewable as described
below in the "Generated Classes" section.

Files in the
[`src/util` directory](https://github.com/MingweiSamuel/Camille/tree/master/Camille.RiotGames/src/Util)/`MingweiSamuel.Camille.Util` namespace
are internal classes used for sending requests and handling rate limits.

### Generated Classes

The majority of the code in Camille is automatically generated. The generated sources are not commited to
the master branch, but
[are available in the `gh-pages` branch in the `_gen/` directory](https://github.com/MingweiSamuel/Camille/tree/gh-pages/_gen).

[GeneratedClasses.cs](https://github.com/MingweiSamuel/Camille/blob/gh-pages/_gen/GeneratedClasses.cs) contains all endpoints,
and therefore endpoint methods, as well as all the data transfer objects corresponding to the JSON returned by the Riot API.

[Champion.cs](https://github.com/MingweiSamuel/Camille/blob/gh-pages/_gen/Champion.cs) is an enum for working with champion IDs.

[RiotApiConfig.cs](https://github.com/MingweiSamuel/Camille/blob/gh-pages/_gen/RiotApiConfig.cs) is the configuration builder
code for creating `RiotApi` instnaces with custom settings.

The actual code for generating these classes is in the
[`src/gen` folder](https://github.com/MingweiSamuel/Camille/tree/master/Camille.RiotGames/gen).
The C#-generating code is in `*.cs.dt` files and is written in NodeJS, using
[doT.js templates](https://olado.github.io/doT/index.html).
