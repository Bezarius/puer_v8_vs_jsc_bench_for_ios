const {$ref, $unref, $generic, $promise, $typeof} = puer;

export function CalcFib(n) {
    if (n <= 1) {
        return n;
    }
    return CalcFib(n - 1) + CalcFib(n - 2);
}

export function GetPlayerProgress() {

    let playerProgress = new CS.Models.PlayerProgress();

    playerProgress.accountLevel = 1;
    playerProgress.accountExp = 100;
    playerProgress.playerArenaId = 123;
    playerProgress.maxPlayerArenaId = 200;
    playerProgress.playerRating = 1500;
    playerProgress.maxPlayerRating = 2000;

    let List = $generic(CS.System.Collections.Generic.List$1, CS.System.Int32);
    let IntDict = $generic(CS.System.Collections.Generic.Dictionary$2, CS.System.Int32, CS.System.Int32);
    let LongDict = $generic(CS.System.Collections.Generic.Dictionary$2, CS.System.Int32, CS.System.Int64);
    let HeroStateDict = $generic(CS.System.Collections.Generic.Dictionary$2, CS.System.Int32, CS.Models.HeroState);

    playerProgress.heroes = new HeroStateDict;
    playerProgress.heroShards = new IntDict();
    playerProgress.currencies = new LongDict();
    playerProgress.emoji = new List();
    playerProgress.skins = new List();
    playerProgress.taunts = new List();
    playerProgress.stances = new List();
    playerProgress.weapons = new List();

    for (let i = 0; i < 100; i++) {
        playerProgress.heroes.Add(i, new CS.Models.HeroState(1, 0, 1, 0));
        playerProgress.heroShards.Add(i, i * 5);
        playerProgress.currencies.Add(i, i * 100);
        playerProgress.emoji.Add(i);
        playerProgress.skins.Add(i + 1);
        playerProgress.taunts.Add(i + 2);
        playerProgress.stances.Add(i + 3);
        playerProgress.weapons.Add(i + 4);
    }
    return playerProgress;
}

export function SetPlayerProgress(playerProgress) {
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
    for (let i = 0; i < playerProgress.heroes.Count; i++) {
        let hero = playerProgress.heroes.get_Item(i);
        hash += BigInt(hero.Level) + BigInt(hero.Exp) + BigInt(hero.TalentLevel) + BigInt(hero.Duplicates);
    }

    // Iterate over heroShards and currencies and add their values
    for (let i = 0; i < playerProgress.heroShards.Count; i++) {
        hash += BigInt(playerProgress.heroShards.get_Item(i));
    }
    for (let i = 0; i < playerProgress.currencies.Count; i++) {
        hash += BigInt(playerProgress.currencies.get_Item(i));
    }

    // Iterate over emoji, skins, taunts, stances, and weapons and add their values
    hash += sumArray(playerProgress.emoji);
    hash += sumArray(playerProgress.skins);
    hash += sumArray(playerProgress.taunts);
    hash += sumArray(playerProgress.stances);
    hash += sumArray(playerProgress.weapons);

    return hash.toString(); // Convert BigInt to string for compatibility
}

function sumArray(array) {
    let sum = BigInt(0);
    for (let i = 0; i < array.Count; i++) {
        sum += BigInt(array.get_Item(i));
    }
    return sum;
}