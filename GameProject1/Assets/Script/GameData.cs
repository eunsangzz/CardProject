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
    public int IronCard;//
    public int GoldCard;//
    public int TreeCard;
    public int BananaTreeCard;
    public int BananaCard;
    public int HouseCard;
    public int ForgeCard;
    public int TimberCard;
    public int MineCard;
    public int gold;
    public int RockCard;
    public int PanelCard;//
    public int BrickCard;//
    public int IronIngotCard;//
    public int GoldIngotCard;//

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

    

    public GameObject[] Card = new GameObject[2]; // 카드 프리팹 저장
    public int storeUpgrade; // 상점 단계에 따라 나오는 재료카드 많아짐
    public List<GameObject> BasicCardList;
    public List<GameObject> CraftCardList;
}
