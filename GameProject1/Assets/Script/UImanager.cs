using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    public GameObject Function;

    public GameObject craftUi;
    public GameObject craftListUi;
    public GameObject craftUiBtn;
    public GameObject menuUi;
    public GameObject menuUiBtn;
    public GameObject buyBtn;
    public GameObject SellUi;
    public GameObject SellBtn;
    public GameObject cardInfoUi;
    public GameObject cardSkillUi;
    public GameObject treeSkillBtn;
    public GameObject rockSkillBtn;
    public GameObject TimberSkillBtn;
    public GameObject ForgeSkillBtn;
    public GameObject MineSkillBtn;
    public GameObject WoodSkillBtn;
    public GameObject HouseSkillBtn;
    public GameObject bananaTreeBtn;
    public GameObject StoreUpBtn;
    public GameObject StrawBerryTreeBtn;

    public GameObject tutoInfoUi;
    public GameObject tutoDayUi;
    public GameObject tutoBtnUi;
    public GameObject tutoBuy;
    public TextMeshProUGUI tutoBuyText;
    public GameObject tutoSell;
    public TextMeshProUGUI tutoSellText;
    public GameObject tutoCraft;
    public TextMeshProUGUI tutoCraftText;
    public GameObject tutoDay;
    public TextMeshProUGUI tutoDayText;
    public GameObject tutoStoreUp;
    public TextMeshProUGUI tutoStoreUpText;
    bool tutoday;
    bool tutocraft;
    bool StoreUp;
    public GameObject tutoStart;

    public TextMeshProUGUI WoodCountText;
    public TextMeshProUGUI StoneCountText;
    public TextMeshProUGUI IronCountText;
    public TextMeshProUGUI GoldCountText;
    public TextMeshProUGUI PanelCountText;
    public TextMeshProUGUI BrickCountText;
    public TextMeshProUGUI IronIngotCountText;
    public TextMeshProUGUI GoldIngotCountText;
    public TextMeshProUGUI BranchCountText;

    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI FoodCount;
    public TextMeshProUGUI WokerCount;

    public TextMeshProUGUI CardInfoText;
    public TextMeshProUGUI CardNameText;
    public TextMeshProUGUI CardCountText;
    public TextMeshProUGUI DayText;
    public TextMeshProUGUI StoreUpText;

    public GameObject ErrorMessage;
    public TextMeshProUGUI ErrorMessageText;
    public GameObject GameOverMessage;
    public TextMeshProUGUI StoreOver;
    public TextMeshProUGUI gameOver;
    public GameObject tutoEndUI;
    public TextMeshProUGUI tutoEndText;

    public TextMeshProUGUI GoalText;
    public TextMeshProUGUI StartText;
    public TextMeshProUGUI QuestText;
    public TextMeshProUGUI QuestNumText;

    public Slider slTimer;
    float fSliderBarTime;

    float delayQuest;

    int feedplayer;
    int notfeedplayer;
    bool feed = false;
    bool Over = false; 
    bool foodfull = false;

    int craftsibling;

    private void Start()
    {
        cardInfoUi.SetActive(false);
        slTimer.value = 120.0f;
        tutoday = false;
        tutocraft = false;
        Time.timeScale = 0f;
        craftsibling = craftUiBtn.transform.GetSiblingIndex();
        tutoInfoUi.SetActive(true);
        tutoStart.SetActive(true);
    }

    private void Update()
    {
        //목표
        if (DataController.instance.gameData.GoldIngotCard == 10)
        {
            tutoEndUI.SetActive(true);
        }

        //씬 저장
        Scene scene = SceneManager.GetActiveScene();
        //튜토 실행
        if(scene.name == "Tuto") DataController.instance.gameData.tuto = true;
        else DataController.instance.gameData.tuto = false;
        //퀘스트 1번 음식
        if(DataController.instance.gameData.QusetNum == 1)
        {
            if (DataController.instance.gameData.FoodCount >= 3) 
            {
                delayQuest += Time.deltaTime;
                if(delayQuest >= 2)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
            }
        }
        //음식이 충분할 때 다음날 음식 삭제
        if(foodfull == true && slTimer.value != 0.0f)
        {
            for(int i = 0; i < DataController.instance.gameData.PlayerCount; i++)
            {
                Function.GetComponent<CardManager>().removeCard(5);
                if (i == DataController.instance.gameData.PlayerCount - 1)
                {
                    foodfull = false;
                }
            }
        }
        //주민 카드가0개일때 실패
        if(DataController.instance.gameData.PlayerCount == 0)
        {
            Fail();
        }

        //시간이 흐를때
        if (slTimer.value > 0.0f && DataController.instance.gameData.endDay == false && feed == false)
        {
            slTimer.value -= 1.0f * Time.deltaTime; //시간 감소
            feedplayer = 0; //밥을 먹은 주민 수
            DataController.instance.gameData.PlayerCount = DataController.instance.gameData.PlayerCount - notfeedplayer; //음식을 안먹은 플레이어가 있다면 주민수 감소?
            notfeedplayer = 0; //밥을 못먹은 주민수 0으로 초기화
            if (DataController.instance.gameData.Sell == true) SellUi.SetActive(true);
            else if (DataController.instance.gameData.Sell == false) SellUi.SetActive(false);

            if (craftUi.activeSelf == true)
            {
                SellBtn.SetActive(false);
                buyBtn.SetActive(false);
                craftUiBtn.SetActive(false);
            }
            else if (craftUi.activeSelf == false)
            {
                SellBtn.SetActive(true);
                buyBtn.SetActive(true);
                craftUiBtn.SetActive(true);
            }

            if (DataController.instance.gameData.Skill == false)
            {
                cardSkillUi.SetActive(false);
            }
            CardInfo();
            CardSkillUI();
            TutoInfoOff();
        }
        
        //시간이 흐르지 않을 때 밤
        else if (slTimer.value == 0.0f && Over == false)
        {
            //튜토리얼 재생
            if(DataController.instance.gameData.tuto == true && tutoday == false) 
            {
                tutoday = true;
                Time.timeScale = 0;
                tutoInfoUi.SetActive(true);
                tutoDayUi.SetActive(true);
                tutoDay.SetActive(true);
            }
            craftUi.SetActive(false);
            cardInfoUi.SetActive(false);
            cardSkillUi.SetActive(false);
            //주민이 0명이 아니라면
            if (DataController.instance.gameData.PlayerCount != 0)
            {
                //하루끝 활성화
                DataController.instance.gameData.endDay = true;
                //밥을 먹은 주민과 주민수가 같지않을때 
                if (feedplayer != DataController.instance.gameData.PlayerCount)
                {
                    //밥먹기 활성화
                    feed = true;
                }

                //카드의 수가 제한량 보다 많을때
                if (DataController.instance.gameData.CardCount > DataController.instance.gameData.CardLimit)
                {
                    //판매 활성화
                    DataController.instance.gameData.Sell = true;
                    cardInfoUi.SetActive(false);
                    SellBtn.SetActive(false);
                    buyBtn.SetActive(false);
                    craftUiBtn.SetActive(false);
                    SellUi.SetActive(true);

                    //밥을 먹은 주민이 더 적고 밥먹기가 활성화 되어있을때
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true)
                    {
                        //음식이 필요량 보다 많을때
                        if (DataController.instance.gameData.FoodCount >= DataController.instance.gameData.PlayerCount)
                        {
                            foodfull = true; //음식 충분
                            feed = false; // 밥먹기 비활성화
                        }
                        else
                        {
                            //음식이 부족한데 밥을 못먹은 플레이어가 있다면
                            if (feedplayer != DataController.instance.gameData.PlayerCount)
                            {
                                if (DataController.instance.gameData.FoodCount > 0) //음식이 한개라도 있다
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        Function.GetComponent<CardManager>().removeCard(5);
                                        feedplayer++;
                                    }
                                    else SceneManager.LoadScene("MainScene");
                                }
                                else //음식이 하나도없다
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0) //주민이 있다면
                                    {
                                        Function.GetComponent<CardManager>().removeCard(20); //주민을 죽인다
                                        feedplayer++;
                                        notfeedplayer++;
                                    }
                                    else SceneManager.LoadScene("MainScene");
                                }
                            }
                        }
                    }
                    if (feedplayer == DataController.instance.gameData.PlayerCount)
                    {
                        feed = false;
                    }
                }
                else  //카드가 제한량보다 적을때
                {
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true)
                    {
                        if (DataController.instance.gameData.FoodCount >= DataController.instance.gameData.PlayerCount) //음식이 더 많다면
                        {
                            foodfull = true;
                            feed = false;
                        }
                        else
                        {
                            if (feedplayer != DataController.instance.gameData.PlayerCount)
                            {
                                if (DataController.instance.gameData.FoodCount >= 1)
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        Function.GetComponent<CardManager>().removeCard(5);
                                        feedplayer++;
                                    }
                                }
                                else
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        Function.GetComponent<CardManager>().removeCard(20);
                                        feedplayer++;
                                        notfeedplayer++;
                                    }
                                }
                            }
                        }
                    }
                    if (feedplayer == DataController.instance.gameData.PlayerCount)
                    {
                        feed = false;
                    }
                    if (feed == false)
                    {
                        Debug.Log("next");
                        DataController.instance.gameData.endDay = false;
                        DataController.instance.gameData.Day += 1;
                        SellBtn.SetActive(true);
                        buyBtn.SetActive(true);
                        craftUiBtn.SetActive(true);
                        slTimer.value = 120.0f;
                    }
                }
            }

        }
    }


    public void CraftUiBtn()
    {
        if(DataController.instance.gameData.tuto == true && tutocraft == false)
        {
            tutoInfoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoCraft.SetActive(true);
            craftUiBtn.transform.SetAsLastSibling();
            Time.timeScale =0;
            tutocraft = true;
        }
        craftUi.SetActive(true);
        craftListUi.SetActive(true);
        buyBtn.SetActive(false);
        SellBtn.SetActive(false);
        cardInfoUi.SetActive(false);
    }

    public void CraftUiCloseBtn()
    {
        craftUi.SetActive(false);
        craftUiBtn.SetActive(true);
        buyBtn.SetActive(true);
    }

    public void CardSkillCloseBtn()
    {
        cardSkillUi.SetActive(false);
    }

    public void MenuUiBtn()
    {
        menuUi.SetActive(true);
        menuUiBtn.SetActive(false);
    }

    public void MenuUiCloseBtn()
    {
        menuUi.SetActive(false);
        menuUiBtn.SetActive(true);
    }

    public void ErrorMessageClose()
    {
        ErrorMessage.SetActive(false);
    }

    private void CardInfo()
    {
        if (Input.GetMouseButton(0))
        {
            if (DataController.instance.gameData.Sell == false)
            {
                cardInfoUi.SetActive(false);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touch = hit.transform.gameObject;
                    cardInfoUi.SetActive(true);

                    if (touch.name == "Wood(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "목재";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "가장 기본재료 나무를 벌목해 얻는다. 여러가지 제작품에 재료로 사용가능하다.";
                    }
                    else if (touch.name == "Stone(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "석재";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "가장 기본재료 암석을 채광해 얻는다. 여러가지 제작품에 재료로 사용가능하다. ";
                    }
                    else if (touch.name == "Tree(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "나무";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "벌목하면 목재를 얻을 수 있다. 지금은 아무것도 아니지";
                    }
                    else if (touch.name == "Rock(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "암석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "채광하면 석재를 얻을 수 있다. 지금은 너무 무겁지";
                    }
                    else if (touch.name == "House(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "집";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "카드의 한도를 늘려준다. 플레이어의 수를 늘릴수있다.";
                    }
                    else if (touch.name == "Banana(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "바나나";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "기본음식 그냥도 먹을 수 있지만 요리해서 먹으면 더욱 배부르다.";
                    }
                    else if (touch.name == "BananaTree(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "바나나나무";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "채집을 하면 바나나를 얻을 수 있다.";
                    }
                    else if (touch.name == "Brick(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "벽돌";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "돌을 가공해 만든 벽돌 튼튼하다.";
                    }
                    else if (touch.name == "Forge(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "용광로";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "철과 금을 제련할 수 있다.";
                    }
                    else if (touch.name == "Gold(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "금광석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "제련을 통해 빛나는 금괴로 만들 수 있다.";
                    }
                    else if (touch.name == "GoldIngot(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "금괴";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "비싸게 팔리는 금괴 다른 역할은?";
                    }
                    else if (touch.name == "Iron(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "철광석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "제련을 통해 단단한 철괴를 만들 수 있다.";
                    }
                    else if (touch.name == "IronIngot(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "철괴";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "많은 것을 만들 수 있는 기본이면서 최강의 제료";
                    }
                    else if (touch.name == "Panel(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "판자";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 가공해 만드는 판때기 집만들때 사용한다";
                    }
                    else if (touch.name == "Player(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "주민";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "주민이 없으면 게임은 끝나버린다. 배가 고프지";
                    }
                    else if (touch.name == "Mine(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "벽돌공장";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "돌을 가공해 벽돌을 만드는 공장";
                    }
                    else if (touch.name == "Timber(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "제재소";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 가공해 판자를 만드는 공장";
                    }
                    else if (touch.name == "Branch(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "나뭇가지";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 손질해 얻은 나뭇가지" + System.Environment.NewLine + "화로의 연료로 사용한다.";
                    }
                    else if (touch.name == "StrawBerryTree(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "딸기나무";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "채집을 하면 딸기를 얻을 수 있다.";
                    }
                    else if (touch.name == "StrawBerry(Clone)")
                    {
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "딸기";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "기본음식 그냥도 먹을 수 있지만 요리해서 먹으면 더욱 배부르다.";
                    }
                }
            }
        }
    }

    private void TutoInfoOff()
    {
        if(tutoInfoUi.activeSelf == true)
        {
            if(Input.GetMouseButton(0))
            {
                tutoInfoUi.SetActive(false);
                tutoBuy.SetActive(false);
                tutoCraft.SetActive(false);
                tutoSell.SetActive(false);
                tutoDay.SetActive(false);
                tutoDayUi.SetActive(false);
                tutoBtnUi.SetActive(false);
                tutoStart.SetActive(false);
                tutoStoreUp.SetActive(false);
                craftUiBtn.transform.SetSiblingIndex(craftsibling);
                Time.timeScale =1;
            }
        }
    }

    private void CardSkillUI()
    {
        if (Input.GetMouseButton(0))
        {
            if (DataController.instance.gameData.Sell == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touch = hit.transform.gameObject;
                    DataController.instance.gameData.Skill = true;

                    if (touch.name == "Tree(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(true);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if (touch.name == "Rock(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        rockSkillBtn.SetActive(true);
                        WoodSkillBtn.SetActive(false);
                        treeSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if (touch.name == "BananaTree(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        bananaTreeBtn.SetActive(true);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        treeSkillBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if(touch.name == "StrawBerryTree(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        StrawBerryTreeBtn.SetActive(true);
                        bananaTreeBtn.SetActive(false);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        treeSkillBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                    }
                    if (touch.name == "Wood(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(false);
                        WoodSkillBtn.SetActive(true);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if(touch.name == "Timber(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(false);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(true);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if(touch.name == "Mine(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(false);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(true);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if(touch.name == "House(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(false);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(true);
                        ForgeSkillBtn.SetActive(false);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                    if(touch.name == "Forge(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        treeSkillBtn.SetActive(false);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        ForgeSkillBtn.SetActive(true);
                        StrawBerryTreeBtn.SetActive(false);
                    }
                }
            }
        }
    }

    public void StoreUpgrade()
    {
        if(DataController.instance.gameData.tuto == true && StoreUp == false)
        {
            tutoInfoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoStoreUp.SetActive(true);
            Time.timeScale =0;
            StoreUp = true;
        }
        if (DataController.instance.gameData.gold >= 100 && DataController.instance.gameData.storeUpgrade == 0 
            && Time.timeScale != 0)
        {
            if(DataController.instance.gameData.QusetNum == 5)
            {
                DataController.instance.gameData.QusetNum += 1;
            }
            DataController.instance.gameData.storeUpgrade += 1;
            DataController.instance.gameData.gold -= 100;
            StoreUpBtn.SetActive(false);
        }
        else if (DataController.instance.gameData.gold < 100 && Time.timeScale != 0) 
        {
            ErrorMessage.SetActive(true);
            ErrorMessageText.GetComponent<TextMeshProUGUI>().text = "상점을 업그레이드" + System.Environment.NewLine + "하려면 100골드가 필요합니다!";
        }
    }

    private void LateUpdate()
    {
        WoodCountText.GetComponent<TextMeshProUGUI>().text = "목재 : " + DataController.instance.gameData.WoodCard;
        StoneCountText.GetComponent<TextMeshProUGUI>().text = "석제 : " + DataController.instance.gameData.StoneCard;
        IronCountText.GetComponent<TextMeshProUGUI>().text = "철광석 : " + DataController.instance.gameData.IronCard;
        GoldCountText.GetComponent<TextMeshProUGUI>().text = "금광석 : " + DataController.instance.gameData.GoldCard;
        GoldIngotCountText.GetComponent<TextMeshProUGUI>().text = "금괴 : " + DataController.instance.gameData.GoldIngotCard;
        IronIngotCountText.GetComponent<TextMeshProUGUI>().text = "철괴 : " + DataController.instance.gameData.IronIngotCard;
        BrickCountText.GetComponent<TextMeshProUGUI>().text = "벽돌 : " + DataController.instance.gameData.BrickCard;
        PanelCountText.GetComponent<TextMeshProUGUI>().text = "판자 : " + DataController.instance.gameData.PanelCard;
        BranchCountText.GetComponent<TextMeshProUGUI>().text = "나뭇가지 : " + DataController.instance.gameData.BranchCard;

        GoldText.GetComponent<TextMeshProUGUI>().text = "골드 : " + DataController.instance.gameData.gold;
        CardCountText.GetComponent<TextMeshProUGUI>().text = "카드제한 : " + DataController.instance.gameData.CardLimit + "/" + DataController.instance.gameData.CardCount;
        DayText.GetComponent<TextMeshProUGUI>().text = "생존일 : " + DataController.instance.gameData.Day;
        FoodCount.GetComponent<TextMeshProUGUI>().text = "음식 : " + DataController.instance.gameData.FoodCount + "/" + DataController.instance.gameData.PlayerCount;
        GoalText.GetComponent<TextMeshProUGUI>().text = "목표 : 금괴 10 / " + DataController.instance.gameData.GoldIngotCard;
        WokerCount.GetComponent<TextMeshProUGUI>().text = "일꾼 : " + DataController.instance.gameData.Woker + " / " + DataController.instance.gameData.PlayerCount;

        tutoBuyText.GetComponent<TextMeshProUGUI>().text = "3골드로 카드를 구매 가능합니다.";
        tutoCraftText.GetComponent<TextMeshProUGUI>().text = "재료를 모아 제작이 가능합니다..";
        tutoDayText.GetComponent<TextMeshProUGUI>().text = "밤이 되면 제한된 카드에 맞추어 카드를 팔아야합니다." + System.Environment.NewLine + "또한 주민은 음식을 필요로 하고" +
            System.Environment.NewLine + "음식이 부족한 만큼 주민이 죽게됩니다.";
        tutoSellText.GetComponent<TextMeshProUGUI>().text = "상단 판매가"+ System.Environment.NewLine + "활성화 되어있으면" + System.Environment.NewLine + "카드를 클릭해" 
        + System.Environment.NewLine + "팔 수 있습니다";
        tutoStoreUpText.GetComponent<TextMeshProUGUI>().text = "100골드로 상점을 업그레이드 할수있습니다. 새로운 재료가 나옵니다.";

        //StoreOver.GetComponent<TextMeshProUGUI>().text = "100골드가 필요합니다!";
        gameOver.GetComponent<TextMeshProUGUI>().text = "게임오버!" + System.Environment.NewLine + "모든 주민이" + System.Environment.NewLine  + "음식이 없어 굶어 죽었습니다.";
        StartText.GetComponent<TextMeshProUGUI>().text = "목적을 달성했습니다." + System.Environment.NewLine + "이제 본게임으로";

        if (DataController.instance.gameData.QusetNum == 0)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "구매 버튼을 눌러 카드를 구매하세요";
        }
        if(DataController.instance.gameData.QusetNum == 1)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "목재카드 선택 후 나뭇가지를 만드세요";
        }
        if(DataController.instance.gameData.QusetNum == 2)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "나뭇가지를 판매하고 바나나를 채집하세요";
        }
        if(DataController.instance.gameData.QusetNum == 3)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "제작에서 제제소 또는 벽돌공장을 만드세요";
        }
        if(DataController.instance.gameData.QusetNum == 4)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "벽돌 또는 판자를 만드세요";
        }
        if(DataController.instance.gameData.QusetNum == 5)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "100골드를 모아 상점을 업그레이드 하세요";
        }
        if(DataController.instance.gameData.QusetNum == 6)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "화로를 만들어 금괴를 만드세요";
        }
        if(DataController.instance.gameData.QusetNum == 7)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "퀘스트 완료! 목적을 달성하세요";
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MainSecne()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void StartBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void TutoBtn()
    {
        SceneManager.LoadScene("Tuto");
    }

    public void Fail()
    {
        tutoInfoUi.SetActive(false);
        tutoBuy.SetActive(false);
        tutoCraft.SetActive(false);
        tutoSell.SetActive(false);
        tutoDay.SetActive(false);
        tutoDayUi.SetActive(false);
        tutoBtnUi.SetActive(false);
        tutoStart.SetActive(false);
        tutoStoreUp.SetActive(false);
        GameOverMessage.SetActive(true);
        Over = true;
        slTimer.value = 0.0f;
    }

    public void tutoEndBtn()
    {
        tutoEndUI.SetActive(false);
    }
}
