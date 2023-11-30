using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

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

    public List<GameObject> Card;
    public int storeUpgrade; // 상점 단계에 따라 나오는 재료카드 많아짐

    public void addCardCount(int i)
    {
        if (i == 0) DataController.instance.gameData.WoodCard += 1;
        if (i == 1) DataController.instance.gameData.StoneCard += 1;
        if (i == 2) DataController.instance.gameData.TreeCard += 1;
        if (i == 3) DataController.instance.gameData.RockCard += 1;
        if (i == 4) DataController.instance.gameData.BananaTreeCard += 1;
        if (i == 5) DataController.instance.gameData.BananaCard += 1;
        if (i == 6) DataController.instance.gameData.StrawBerryCard += 1;
        if (i == 7) DataController.instance.gameData.StrawBerryTreeCard += 1;
        if (i == 8) DataController.instance.gameData.IronCard += 1;
        if (i == 9) DataController.instance.gameData.GoldCard += 1;
        if (i == 10) DataController.instance.gameData.BranchCard += 1;
        if (i == 11) DataController.instance.gameData.IronIngotCard += 1;
        if (i == 12) DataController.instance.gameData.GoldIngotCard += 1;
        if (i == 13) DataController.instance.gameData.BrickCard += 1;
        if (i == 14) DataController.instance.gameData.PanelCard += 1;
        if (i == 15) DataController.instance.gameData.HouseCard += 1;
        if (i == 16) DataController.instance.gameData.ForgeCard += 1;
        if (i == 17) DataController.instance.gameData.TimberCard += 1;
        if (i == 18) DataController.instance.gameData.MineCard += 1;
        if (i == 19) DataController.instance.gameData.KitchenCard += 1;
        if (i == 20) DataController.instance.gameData.PlayerCount += 1;
    }

    public void stdCardCount(int i)
    {
        if (i == 0) DataController.instance.gameData.WoodCard -= 1;
        if (i == 1) DataController.instance.gameData.StoneCard -= 1;
        if (i == 2) DataController.instance.gameData.TreeCard -= 1;
        if (i == 3) DataController.instance.gameData.RockCard -= 1;
        if (i == 4) DataController.instance.gameData.BananaTreeCard -= 1;
        if (i == 5) DataController.instance.gameData.BananaCard -= 1;
        if (i == 6) DataController.instance.gameData.StrawBerryCard -= 1;
        if (i == 7) DataController.instance.gameData.StrawBerryTreeCard -= 1;
        if (i == 8) DataController.instance.gameData.IronCard -= 1;
        if (i == 9) DataController.instance.gameData.GoldCard -= 1;
        if (i == 10) DataController.instance.gameData.BranchCard -= 1;
        if (i == 11) DataController.instance.gameData.IronIngotCard -= 1;
        if (i == 12) DataController.instance.gameData.GoldIngotCard -= 1;
        if (i == 13) DataController.instance.gameData.BrickCard -= 1;
        if (i == 14) DataController.instance.gameData.PanelCard -= 1;
        if (i == 15) DataController.instance.gameData.HouseCard -= 1;
        if (i == 16) DataController.instance.gameData.ForgeCard -= 1;
        if (i == 17) DataController.instance.gameData.TimberCard -= 1;
        if (i == 18) DataController.instance.gameData.MineCard -= 1;
        if (i == 19) DataController.instance.gameData.KitchenCard -= 1;
        if (i == 20) DataController.instance.gameData.PlayerCount -= 1;
    }
}
