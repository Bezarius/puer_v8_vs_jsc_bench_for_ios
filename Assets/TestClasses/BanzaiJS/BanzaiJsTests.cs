using System;
using System.Collections.Generic;
using System.Diagnostics;
using JSBridge;
using JsTools;
using Models;
using Newtonsoft.Json.Shims;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class BanzaiJsTests : IScriptTest, IDisposable
    {
        private readonly EngineTag _engineTag;
        private readonly int _iterations;
        private IJsEngine _banzaiJsEnv;
        private readonly Stopwatch _sw = new();

        public BanzaiJsTests(EngineTag engineTag, int iterations = 1000)
        {
            _engineTag = engineTag;
            _iterations = iterations;
        }

        public void Initialize()
        {
            _banzaiJsEnv = new JsEngineFactory(new NativeJsFactory())
                .CreateJsEngine(_engineTag, 31, 31, 31);
            var ok = _banzaiJsEnv.Execute(JsSourceProvider.Get());
            Debug.Log("script is ok: " + ok);
        }

        public long TestGetPlayerProgress()
        {
            _sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                using var result = _banzaiJsEnv.CallFunction("GetPlayerProgress", null, null);
                var progress = new PlayerProgress
                {
                    accountLevel = result["accountLevel"].AsInteger(),
                    accountExp = result["accountExp"].AsInteger(),
                    playerArenaId = result["playerArenaId"].AsInteger(),
                    maxPlayerArenaId = result["maxPlayerArenaId"].AsInteger(),
                    playerRating = result["playerRating"].AsInteger(),
                    maxPlayerRating = result["maxPlayerRating"].AsInteger(),
                    heroes = new Dictionary<int, HeroState>(),
                    heroShards = new Dictionary<int, int>(),
                    currencies = new Dictionary<int, long>(),
                    emoji = new List<int>(),
                    skins = new List<int>(),
                    taunts = new List<int>(),
                    stances = new List<int>(),
                    weapons = new List<int>()
                };

                using var heroes = result["heroes"];
                using var heroShards = result["heroShards"];
                using var currencies = result["currencies"];
                for (int j = 0; j < 100; j++)
                {
                    using var hero = heroes[j];
                    var heroState = new HeroState
                    {
                        Level = hero["Level"].AsInteger(),
                        Exp = hero["Exp"].AsInteger(),
                        TalentLevel = hero["TalentLevel"].AsInteger(),
                        Duplicates = hero["Duplicates"].AsInteger()
                    };
                    progress.heroes[j] = heroState;
                    progress.heroShards[j] = heroShards[j].AsInteger();
                    progress.currencies[j] = currencies[j].AsInteger();
                }

                using var emoji = result["emoji"];
                using var skins = result["skins"];
                using var taunts = result["taunts"];
                using var stances = result["stances"];
                using var weapons = result["weapons"];

                for (int j = 0; j < 100; j++)
                {
                    progress.emoji.Add(emoji[j].AsInteger());
                    progress.skins.Add(skins[j].AsInteger());
                    progress.taunts.Add(taunts[j].AsInteger());
                    progress.stances.Add(stances[j].AsInteger());
                    progress.weapons.Add(weapons[j].AsInteger());
                }

                _sw.Stop();
                progress.AssertPlayerProgressIsValid();
                _sw.Start();
            }

            var ms = _sw.ElapsedMilliseconds;
            _sw.Reset();
            return ms;
        }

        public long TestSetPlayerProgress()
        {
            _sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                _sw.Stop();
                var progress = TestDataProvider.GetTestPlayerProgress();
                _sw.Start();
                using var result = _banzaiJsEnv.CallFunction("SetPlayerProgress", null, new object[]
                {
                    progress
                });
            }

            var ms = _sw.ElapsedMilliseconds;
            _sw.Reset();
            return ms;
        }

        [Preserve] private int _fibResult;

        public long TestFibonacci(int n)
        {
            _sw.Start();
            for (int i = 0; i < _iterations; i++)
            {
                using var result = _banzaiJsEnv.CallFunction("CalcFib", null, new object[] { n });
                _fibResult = result.AsInteger();
            }
            var ms = _sw.ElapsedMilliseconds;
            _sw.Reset();
            return ms;
        }

        public void Dispose()
        {
            _banzaiJsEnv.Dispose();
        }
    }
}