using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace Models
{
    [Obfuscation(Exclude = true)]
    public struct PlayerProgress
    {
        public int accountLevel;
        public int accountExp;
        public int playerArenaId;
        public int maxPlayerArenaId;
        public int playerRating;
        public int maxPlayerRating;
        public Dictionary<int, HeroState> heroes;
        public Dictionary<int, int> heroShards;
        public Dictionary<int, long> currencies;
        public List<int> emoji;
        public List<int> skins;
        public List<int> taunts;
        public List<int> stances;
        public List<int> weapons;

        public PlayerProgress(int accountLevel, int accountExp, int playerArenaId, int maxPlayerArenaId,
            int playerRating, int maxPlayerRating, Dictionary<int, HeroState> heroes,
            Dictionary<int, int> heroShards, Dictionary<int, long> currencies, List<int> emoji, List<int> skins,
            List<int> taunts, List<int> stances, List<int> weapons)
        {
            this.accountLevel = accountLevel;
            this.accountExp = accountExp;
            this.playerArenaId = playerArenaId;
            this.maxPlayerArenaId = maxPlayerArenaId;
            this.playerRating = playerRating;
            this.maxPlayerRating = maxPlayerRating;
            this.heroes = heroes;
            this.heroShards = heroShards;
            this.currencies = currencies;
            this.emoji = emoji;
            this.skins = skins;
            this.taunts = taunts;
            this.stances = stances;
            this.weapons = weapons;
        }

        public override int GetHashCode()
        {
            var part = System.HashCode.Combine(accountLevel, accountExp, playerArenaId, maxPlayerArenaId, playerRating,
                maxPlayerRating, heroes);
            return System.HashCode.Combine(part, heroShards, currencies, emoji, skins, taunts, stances, weapons);
        }


        public void AssertPlayerProgressIsValid(int iterations = 100, [CallerMemberName] string member = null)
        {
            Assert.IsTrue(accountLevel == 1, "playerProgress.accountLevel == 1, " + member);
            Assert.IsTrue(accountExp == 100, "playerProgress.accountExp == 100, " + member);
            Assert.IsTrue(playerArenaId == 123, "playerProgress.playerArenaId == 123, " + member);
            Assert.IsTrue(maxPlayerArenaId == 200, "playerProgress.maxPlayerArenaId == 200, " + member);
            Assert.IsTrue(playerRating == 1500, "playerProgress.playerRating == 1500, " + member);
            Assert.IsTrue(maxPlayerRating == 2000, "playerProgress.maxPlayerRating == 2000, " + member);
            Assert.IsTrue(heroes.Count == iterations,
                $"playerProgress.heroes.Count == {heroes.Count}, " + member);
            Assert.IsTrue(heroShards.Count == iterations,
                $"playerProgress.heroShards.Count == {heroShards.Count}, " + member);
            Assert.IsTrue(currencies.Count == iterations,
                $"playerProgress.currencies.Count == {currencies.Count}, " + member);
            Assert.IsTrue(emoji.Count == iterations,
                $"playerProgress.emoji.Count == {emoji.Count}, " + member);
            Assert.IsTrue(skins.Count == iterations,
                $"playerProgress.skins.Count == {skins.Count}, " + member);
            Assert.IsTrue(taunts.Count == iterations,
                $"playerProgress.taunts.Count == {taunts.Count}, " + member);
            Assert.IsTrue(stances.Count == iterations,
                $"playerProgress.stances.Count == {stances.Count}, " + member);
            Assert.IsTrue(weapons.Count == iterations,
                $"playerProgress.weapons.Count == {weapons.Count}, " + member);
        }
    }

    public struct HeroState
    {
        public int Level;
        public int Exp;
        public int TalentLevel;
        public int Duplicates;

        public HeroState(int level, int exp, int talentLevel, int duplicates)
        {
            Level = level;
            Exp = exp;
            TalentLevel = talentLevel;
            Duplicates = duplicates;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Level, Exp, TalentLevel, Duplicates);
        }
    }
}