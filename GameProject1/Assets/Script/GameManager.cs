using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        DataController.instance.gameData.BasicCardList = new List<GameObject>();
        DataController.instance.gameData.CraftCardList = new List<GameObject>();
        //게임실행시 그 전 데이터 초기화 출시시 삭제
        DataController.instance.gameData.storeUpgrade = 0; //나중에 삭제
        DataController.instance.gameData.WoodCard = 0;
        DataController.instance.gameData.StoneCard = 0;
        DataController.instance.gameData.HouseCard = 0;
        DataController.instance.gameData.BananaCard = 0;
        DataController.instance.gameData.BananaTreeCard = 0;
        DataController.instance.gameData.TreeCard = 0;
        DataController.instance.gameData.RockCard = 0;
        DataController.instance.gameData.gold = 100;
        DataController.instance.gameData.CardLimit =15;
        DataController.instance.gameData.CardCount = 0;
        DataController.instance.gameData.PlayerCount = 1;
        DataController.instance.gameData.Day = 0;
        DataController.instance.gameData.FoodCount = 0;
        DataController.instance.gameData.Stage =1;
        DataController.instance.gameData.BossStage = false;
        DataController.instance.gameData.storeUpgrade = 0;

        DataController.instance.gameData.Sell = false;
        DataController.instance.gameData.Skill = false;
        DataController.instance.gameData.endDay = false;
        DataController.instance.gameData.Fight = false;
        DataController.instance.gameData.Boss1Hp = 45;
        DataController.instance.gameData.Boss2Hp = 70;
    }

    // Update is called once per frame
    void Update()
    {
        GameOverScene();
        DataController.instance.gameData.CardCount = (DataController.instance.gameData.WoodCard + DataController.instance.gameData.StoneCard +
        DataController.instance.gameData.TreeCard + DataController.instance.gameData.RockCard + DataController.instance.gameData. BananaTreeCard +
        DataController.instance.gameData.IronCard + DataController.instance.gameData.GoldCard + DataController.instance.gameData.HouseCard +
        DataController.instance.gameData.GoldIngotCard + DataController.instance.gameData.IronIngotCard + DataController.instance.gameData.BrickCard +
        DataController.instance.gameData.PanelCard + DataController.instance.gameData.TimberCard+ DataController.instance.gameData.MineCard +
        DataController.instance.gameData.ForgeCard);

        DataController.instance.gameData.FoodCount = DataController.instance.gameData.BananaCard;

        if(DataController.instance.gameData.GoldIngotCard == 10)
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    void MainSecne()
    {
        SceneManager.LoadScene("MainScene");
    }

    void GameOverScene()
    {
        if(DataController.instance.gameData.PlayerCount == 0)
        {
            Debug.Log("end");
            SceneManager.LoadScene("GameOver");
        }
    }
}