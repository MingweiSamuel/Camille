# Camille

[![AppVeyor branch](https://img.shields.io/appveyor/ci/MingweiSamuel/Camille/master.svg?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/MingweiSamuel/Camille) [![NuGet Stable](https://img.shields.io/nuget/v/MingweiSamuel.Camille.svg?style=flat-square)]() [![NuGet Pre Release](https://img.shields.io/nuget/vpre/MingweiSamuel.Camille.svg?style=flat-square)]()

C# Library for the [Riot Games API](https://developer.riotgames.com/)

Camille's goals are _speed_ and _reliability_. Camille handles rate limits and large requests seamlessly. Data classes are automatically generated from the [Riot API Reference](https://developer.riotgames.com/api-methods/) ([Swagger](http://www.mingweisamuel.com/riotapi-schema/tool/)).

## Features

* Fast, asynchronous, thread-safe
* Automatically retries failed requests
* Highly-configurable
* Targets .NET Standard 1.1+ (.NET Core 1.0+, .NET Framework 4.5+)
* Riot API V4 Support

## Installation

[Install via NuGet](https://www.nuget.org/packages/MingweiSamuel.Camille/) (`MingweiSamuel.Camille`).

## Usage

All API interactions are done using a `RiotApi` instance.
`RiotApi.NewInstance` takes either just an API key (for default settings) or a `IRiotApiConfig` instance (for custom settings).

```c#
var riotApi = RiotApi.NewInstance("RGAPI-12345678-abcd-1234-abcd-123456abcdef");
```
```c#
var riotApi = RiotApi.NewInstance(
    new RiotApiConfig.Builder("RGAPI-12345678-abcd-1234-abcd-123456abcdef")
    {
        MaxConcurrentRequests = 200,
        Retries = 10,
        // ...
    }.Build()
);
```

API methods are divided up into respective endpoints, corresponding to the left bar of the [API reference](https://developer.riotgames.com/api-methods/).

### Examples

#### Print Summoner's Top Champions

```c#
// Get summoners by name synchronously. (using async is faster).
var summoners = new[]
{
    riotApi.SummonerV4.GetBySummonerName(Region.NA, "c9 sneaky"),
    riotApi.SummonerV4.GetBySummonerName(Region.NA, "double LIFT")
};

foreach (var summoner in summoners)
{
    Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

    var masteries =
        riotApi.ChampionMasteryV4.GetAllChampionMasteries(Region.NA, summoner.Id);

    for (var i = 0; i < 10; i++)
    {
        var mastery = masteries[i];
        // Get champion for this mastery.
        var champ = mastery.ChampionId.ToString();
        // print i, champ id, champ mastery points, and champ level
        Console.WriteLine("{0,3}) {1,-16} {2,7} ({3})", i + 1, champ,
            mastery.ChampionPoints, mastery.ChampionLevel);
    }
    Console.WriteLine();
}
```

Output (2017-01-18):
```
C9 Sneaky's Top 10 Champs:
  1) Jhin             268,866 (7)
  2) Lucian           195,541 (7)
  3) Ezreal           146,950 (7)
  4) Ashe             144,269 (7)
  5) Caitlyn          139,390 (7)
  6) Sivir             84,331 (7)
  7) Twitch            82,702 (7)
  8) Vayne             80,733 (7)
  9) Tristana          75,150 (6)
 10) Miss Fortune      70,757 (7)

Doublelift's Top 10 Champs:
  1) Jhin             126,291 (7)
  2) Caitlyn           97,410 (7)
  3) Vayne             79,420 (7)
  4) Lucian            77,254 (7)
  5) Kalista           43,572 (5)
  6) Ashe              36,408 (7)
  7) Ezreal            35,754 (6)
  8) Twitch            33,169 (6)
  9) Kog'Maw           22,459 (5)
 10) Tristana          20,582 (4)
 
 ```
