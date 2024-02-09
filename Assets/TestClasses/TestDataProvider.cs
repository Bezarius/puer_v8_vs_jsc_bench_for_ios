using System.Collections.Generic;
using Models;
using UnityEngine;

public static class TestDataProvider
{
    private static PlayerProgress[] _differentInstancesOfProgresses;

    public static PlayerProgress GetTestPlayerProgress()
    {
        if (_differentInstancesOfProgresses == null)
        {
            _differentInstancesOfProgresses = new PlayerProgress[100];
            for (int i = 0; i < 100; i++)
            {
                var playerProgress = new PlayerProgress
                {
                    accountLevel = 1,
                    accountExp = 100,
                    playerArenaId = 123,
                    maxPlayerArenaId = 200,
                    playerRating = 1500,
                    maxPlayerRating = 2000,
                    heroes = new Dictionary<int, HeroState>(),
                    heroShards = new Dictionary<int, int>(),
                    currencies = new Dictionary<int, long>(),
                    emoji = new List<int>(),
                    skins = new List<int>(),
                    taunts = new List<int>(),
                    stances = new List<int>(),
                    weapons = new List<int>()
                };

                for (int key = 0; key < 100; key++)
                {
                    playerProgress.heroes[key] = new HeroState(1, 0, 1, 0);
                    playerProgress.heroShards[key] = key * 5;
                    playerProgress.currencies[key] = key * 100;
                }

                for (int item = 0; item < 100; item++)
                {
                    playerProgress.emoji.Add(item);
                    playerProgress.skins.Add(item + 1);
                    playerProgress.taunts.Add(item + 2);
                    playerProgress.stances.Add(item + 3);
                    playerProgress.weapons.Add(item + 4);
                }

                _differentInstancesOfProgresses[i] = playerProgress;
            }
        }

        var randomIndex = Random.Range(0, 100);
        return _differentInstancesOfProgresses[randomIndex];
    }
}