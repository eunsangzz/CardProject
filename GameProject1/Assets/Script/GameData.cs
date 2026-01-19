using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class GameData
{
    public int WoodCard;
    public int StoneCard;
    public int IronCard;
    public int GoldCard;
    public int TreeCard;
    public int BananaTreeCard;
    public int BananaCard;
    public int StrawBerryCard;
    public int StrawBerryTreeCard;
    public int HouseCard;
    public int ForgeCard;
    public int TimberCard;
    public int MineCard;
    public int RockCard;
    public int PanelCard;
    public int BrickCard;
    public int IronIngotCard;
    public int GoldIngotCard;
    public int BranchCard;
    public int KitchenCard;


    public int gold;
    public int CardCount;
    public int CardLimit;
    public int PlayerCount;
    public int WoodSwordCount;
    public int StoneSwordCount;
    public int IronSwordCount;
    public int FoodCount;


    public bool endDay;
    public int Day;
    public int Stage;
    public bool BossStage;
    public int Boss1Hp;
    public int Boss2Hp;


    public bool Sell;
    public bool Skill;
    public bool Fight;
    public bool PlayerAttack;
    public bool EnemyAttack;
    public bool Attack;
    public bool tuto;

    public int Woker;
    public int QusetNum;

    [NonSerialized] public List<GameObject> Card;

    public int storeUpgrade; // 상점 단계에 따라 나오는 재료카드 많아짐

    public void EnsureRuntimeDefaults()
    {
        if (Card == null) Card = new List<GameObject>();
    }






    public void AddGold(int amount)
    {
        if (amount == 0) return;
        gold += amount;
        gold = Mathf.Max(0, gold);
        GameEvents.OnGoldChanged?.Invoke(gold);
    }

    public void SetGold(int value)
    {
        gold = Mathf.Max(0, value);
        GameEvents.OnGoldChanged?.Invoke(gold);
    }

    public void AddFood(int amount)
    {
        if (amount == 0) return;
        FoodCount += amount;
        FoodCount = Mathf.Max(0, FoodCount);
        GameEvents.OnFoodChanged?.Invoke(FoodCount);
    }

    public void SetFood(int value)
    {
        FoodCount = Mathf.Max(0, value);
        GameEvents.OnFoodChanged?.Invoke(FoodCount);
    }

    public void AddPlayer(int amount)
    {
        if (amount == 0) return;
        PlayerCount += amount;
        PlayerCount = Mathf.Max(0, PlayerCount);
        GameEvents.OnPlayerChanged?.Invoke(PlayerCount);
    }

    public void SetPlayer(int value)
    {
        PlayerCount = Mathf.Max(0, value);
        GameEvents.OnPlayerChanged?.Invoke(PlayerCount);
    }

    public void AddWorker(int amount)
    {
        if (amount == 0) return;
        Woker += amount;
        Woker = Mathf.Max(0, Woker);
        GameEvents.OnWorkerChanged?.Invoke(Woker);
    }

    public void SetWorker(int value)
    {
        Woker = Mathf.Max(0, value);
        GameEvents.OnWorkerChanged?.Invoke(Woker);
    }

    public void AddQuest(int amount)
    {
        if (amount == 0) return;
        QusetNum += amount;
        QusetNum = Mathf.Max(0, QusetNum);
        GameEvents.OnQuestChanged?.Invoke(QusetNum);
    }

    public void SetQuest(int value)
    {
        QusetNum = Mathf.Max(0, value);
        GameEvents.OnQuestChanged?.Invoke(QusetNum);
    }

    public void SetCardCountAndLimit(int current, int limit)
    {
        CardCount = Mathf.Max(0, current);
        CardLimit = Mathf.Max(0, limit);
        GameEvents.OnCardCountChanged?.Invoke(CardLimit, CardCount);
    }

    public void SetCardCount(int current)
    {
        CardCount = Mathf.Max(0, current);
        GameEvents.OnCardCountChanged?.Invoke(CardLimit, CardCount);
    }

    public void SetCardLimit(int limit)
    {
        CardLimit = Mathf.Max(0, limit);
        GameEvents.OnCardCountChanged?.Invoke(CardLimit, CardCount);
    }

    public void NextDay()
    {
        Day++;
        GameEvents.OnDayChanged?.Invoke(Day);
    }

    public void SetDay(int day)
    {
        Day = Mathf.Max(0, day);
        GameEvents.OnDayChanged?.Invoke(Day);
    }





    public void RaiseAllEvents()
    {
        GameEvents.OnGoldChanged?.Invoke(gold);
        GameEvents.OnFoodChanged?.Invoke(FoodCount);
        GameEvents.OnPlayerChanged?.Invoke(PlayerCount);
        GameEvents.OnCardCountChanged?.Invoke(CardLimit, CardCount);
        GameEvents.OnDayChanged?.Invoke(Day);
        GameEvents.OnWorkerChanged?.Invoke(Woker);
        GameEvents.OnQuestChanged?.Invoke(QusetNum);
        GameEvents.OnInventoryChanged?.Invoke();
    }

    public void addCardCount(int i) => AddByIndex(i, 1);
    public void stdCardCount(int i) => AddByIndex(i, -1);

    private void AddByIndex(int i, int delta)
    {
        switch (i)
        {
            case 0: WoodCard += delta; break;
            case 1: StoneCard += delta; break;
            case 2: TreeCard += delta; break;
            case 3: RockCard += delta; break;
            case 4: BananaTreeCard += delta; break;
            case 5: BananaCard += delta; break;
            case 6: StrawBerryCard += delta; break;
            case 7: StrawBerryTreeCard += delta; break;
            case 8: IronCard += delta; break;
            case 9: GoldCard += delta; break;
            case 10: BranchCard += delta; break;
            case 11: IronIngotCard += delta; break;
            case 12: GoldIngotCard += delta; break;
            case 13: BrickCard += delta; break;
            case 14: PanelCard += delta; break;
            case 15: HouseCard += delta; break;
            case 16: ForgeCard += delta; break;
            case 17: TimberCard += delta; break;
            case 18: MineCard += delta; break;
            case 19: KitchenCard += delta; break;
            case 20: PlayerCount += delta; break;
            default: break;
        }

        ClampNonNegative();
        GameEvents.OnInventoryChanged?.Invoke();
    }

    private void ClampNonNegative()
    {
        WoodCard = Mathf.Max(0, WoodCard);
        StoneCard = Mathf.Max(0, StoneCard);
        IronCard = Mathf.Max(0, IronCard);
        GoldCard = Mathf.Max(0, GoldCard);
        TreeCard = Mathf.Max(0, TreeCard);
        BananaTreeCard = Mathf.Max(0, BananaTreeCard);
        BananaCard = Mathf.Max(0, BananaCard);
        StrawBerryCard = Mathf.Max(0, StrawBerryCard);
        StrawBerryTreeCard = Mathf.Max(0, StrawBerryTreeCard);
        HouseCard = Mathf.Max(0, HouseCard);
        ForgeCard = Mathf.Max(0, ForgeCard);
        TimberCard = Mathf.Max(0, TimberCard);
        MineCard = Mathf.Max(0, MineCard);
        RockCard = Mathf.Max(0, RockCard);
        PanelCard = Mathf.Max(0, PanelCard);
        BrickCard = Mathf.Max(0, BrickCard);
        IronIngotCard = Mathf.Max(0, IronIngotCard);
        GoldIngotCard = Mathf.Max(0, GoldIngotCard);
        BranchCard = Mathf.Max(0, BranchCard);
        KitchenCard = Mathf.Max(0, KitchenCard);

        PlayerCount = Mathf.Max(0, PlayerCount);
    }

    public enum CardType
    {
        Wood, Stone, Tree, Rock, BananaTree, Banana, StrawBerry, StrawBerryTree,
        Iron, Gold, Branch, IronIngot, GoldIngot, Brick, Panel,
        House, Forge, Timber, Mine, Kitchen
    }

    public void Add(CardType type, int amount = 1)
    {
        if (amount == 0) return;

        switch (type)
        {
            case CardType.Wood: WoodCard += amount; break;
            case CardType.Stone: StoneCard += amount; break;
            case CardType.Tree: TreeCard += amount; break;
            case CardType.Rock: RockCard += amount; break;
            case CardType.BananaTree: BananaTreeCard += amount; break;
            case CardType.Banana: BananaCard += amount; break;
            case CardType.StrawBerry: StrawBerryCard += amount; break;
            case CardType.StrawBerryTree: StrawBerryTreeCard += amount; break;
            case CardType.Iron: IronCard += amount; break;
            case CardType.Gold: GoldCard += amount; break;
            case CardType.Branch: BranchCard += amount; break;
            case CardType.IronIngot: IronIngotCard += amount; break;
            case CardType.GoldIngot: GoldIngotCard += amount; break;
            case CardType.Brick: BrickCard += amount; break;
            case CardType.Panel: PanelCard += amount; break;
            case CardType.House: HouseCard += amount; break;
            case CardType.Forge: ForgeCard += amount; break;
            case CardType.Timber: TimberCard += amount; break;
            case CardType.Mine: MineCard += amount; break;
            case CardType.Kitchen: KitchenCard += amount; break;
            default: break;
        }

        ClampNonNegative();
        GameEvents.OnInventoryChanged?.Invoke();
    }
}
