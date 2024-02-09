namespace DefaultNamespace
{
    public static class JsSourceProvider
    {
        public static string Get() => @"

function CalcFib(n) {
    if (n <= 1) {
        return n;
    }
    return CalcFib(n - 1) + CalcFib(n - 2);
}

function GetPlayerProgress() {
    function PlayerProgress() {
        this.accountLevel = 0;
        this.accountExp = 0;
        this.playerArenaId = 0;
        this.maxPlayerArenaId = 0;
        this.playerRating = 0;
        this.maxPlayerRating = 0;
        this.heroes = {};
        this.heroShards = {};
        this.currencies = {};
        this.emoji = [];
        this.skins = [];
        this.taunts = [];
        this.stances = [];
        this.weapons = [];
    }

    function HeroState(level, exp, talentLevel, duplicates) {
        this.Level = level;
        this.Exp = exp;
        this.TalentLevel = talentLevel;
        this.Duplicates = duplicates;
    }

    let playerProgress = new PlayerProgress();

    playerProgress.accountLevel = 1;
    playerProgress.accountExp = 100;
    playerProgress.playerArenaId = 123;
    playerProgress.maxPlayerArenaId = 200;
    playerProgress.playerRating = 1500;
    playerProgress.maxPlayerRating = 2000;

    for (let i = 0; i < 100; i++) {
        playerProgress.heroes[i] = new HeroState(1, 0, 1, 0);
        playerProgress.heroShards[i] = i * 5;
        playerProgress.currencies[i] = i * 100;
        playerProgress.emoji.push(i);
        playerProgress.skins.push(i + 1);
        playerProgress.taunts.push(i + 2);
        playerProgress.stances.push(i + 3);
        playerProgress.weapons.push(i + 4);
    }

    return playerProgress;
}

function SetPlayerProgress(playerProgress) {
    calculateHash(playerProgress);
}

function calculateHash(playerProgress) {
    let hash = BigInt(0);

    // Add numeric values of properties
    hash += BigInt(playerProgress.accountLevel);
    hash += BigInt(playerProgress.accountExp);
    hash += BigInt(playerProgress.playerArenaId);
    hash += BigInt(playerProgress.maxPlayerArenaId);
    hash += BigInt(playerProgress.playerRating);
    hash += BigInt(playerProgress.maxPlayerRating);

    // Iterate over heroes and add their properties
    Object.values(playerProgress.heroes).forEach(hero => {
        hash += BigInt(hero.Level) + BigInt(hero.Exp) + BigInt(hero.TalentLevel) + BigInt(hero.Duplicates);
    });

    // Iterate over heroShards, currencies, emoji, skins, taunts, stances, and weapons and add their values
    hash += sumArray(Object.values(playerProgress.heroShards));
    hash += sumArray(Object.values(playerProgress.currencies));
    hash += sumArray(playerProgress.emoji);
    hash += sumArray(playerProgress.skins);
    hash += sumArray(playerProgress.taunts);
    hash += sumArray(playerProgress.stances);
    hash += sumArray(playerProgress.weapons);

    return hash.toString(); // Convert BigInt to string for compatibility
}

function sumArray(array) {
    let sum = BigInt(0);
    array.forEach(value => {
        sum += BigInt(value);
    });
    return sum;
}

";
    }
}