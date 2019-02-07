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

API methods are divided up into respective endpoints, corresponding to the left bar of the [API reference](https://developer.riotgames.com/api-methods/).

### Example

#### Print Summoner's Top Champions

```c#
// Get summoners by name synchronously. (using async is faster).
var summoners = new[]
{
    riotApi.SummonerV4.GetBySummonerName(Region.NA, "jAnna kendrick"),
    riotApi.SummonerV4.GetBySummonerName(Region.NA, "lug nuts k")
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
