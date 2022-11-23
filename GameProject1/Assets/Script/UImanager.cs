using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
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

    public TextMeshProUGUI GoalText;
    public TextMeshProUGUI StartText;

    public Slider slTimer;
    float fSliderBarTime;

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
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Tuto") DataController.instance.gameData.tuto = true;
        else DataController.instance.gameData.tuto = false;

        if(foodfull == true)
        {
            Debug.Log("foodtrue");
            for(int i = 0; i < DataController.instance.gameData.PlayerCount; i++)
            {
                GameObject food = GameObject.FindGameObjectWithTag("Food");
                Destroy(food);
                DataController.instance.gameData.BananaCard -=1;
                if(i == DataController.instance.gameData.PlayerCount - 1)
                {
                    foodfull = false;
                }
            }
        }

        if(DataController.instance.gameData.PlayerCount == 0)
        {
            GameOverMessage.SetActive(true);
            Over = true;
            slTimer.value = 0.0f;
        }

        if (slTimer.value > 0.0f && DataController.instance.gameData.endDay == false && feed == false) //�ð��� �带��
        {
            slTimer.value -= 1.0f * Time.deltaTime;
            feedplayer = 0;
            DataController.instance.gameData.PlayerCount = DataController.instance.gameData.PlayerCount - notfeedplayer;
            notfeedplayer = 0;
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
        else if (slTimer.value == 0.0f && Over == false) // �ð��� ��������
        {
            if(DataController.instance.gameData.tuto == true && tutoday == false) 
            {
                tutoday = true;
                Time.timeScale = 0;
                tutoInfoUi.SetActive(true);
                tutoDayUi.SetActive(true);
                tutoDay.SetActive(true);
            }
            if (DataController.instance.gameData.PlayerCount != 0)
            {
                DataController.instance.gameData.endDay = true;
                if (feedplayer != DataController.instance.gameData.PlayerCount)
                {
                    feed = true;
                }
                if (DataController.instance.gameData.CardCount >= DataController.instance.gameData.CardLimit) //ī�尡 ������
                {
                    DataController.instance.gameData.Sell = true;
                    cardInfoUi.SetActive(false);
                    SellBtn.SetActive(false);
                    buyBtn.SetActive(false);
                    craftUiBtn.SetActive(false);
                    SellUi.SetActive(true);
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true) //���� ������ �÷��̾� ī�尡 ������
                    {
                        if (DataController.instance.gameData.FoodCount >= DataController.instance.gameData.PlayerCount )
                        {
                            foodfull = true;
                            feed = false;
                        }
                        else
                        {
                            if (feedplayer != DataController.instance.gameData.PlayerCount)
                            {
                                if (DataController.instance.gameData.FoodCount > 1)
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        GameObject food = GameObject.FindGameObjectWithTag("Food");
                                        Destroy(food);
                                        DataController.instance.gameData.FoodCount -= 1;
                                        feedplayer++;
                                    }
                                    else SceneManager.LoadScene("MainScene");
                                }
                                else
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        GameObject cantfeedplayer1 = GameObject.FindGameObjectWithTag("Player");
                                        Destroy(cantfeedplayer1);
                                        DataController.instance.gameData.PlayerCount -=1;
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
                        Debug.Log("3");
                        feed = false;
                    }
                }
                else //ī�尡 ������
                {
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true) //���� ������ �÷��̾� ī�尡 ������
                    {
                        if (DataController.instance.gameData.FoodCount >= DataController.instance.gameData.PlayerCount)
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
                                        GameObject food = GameObject.FindGameObjectWithTag("Food");
                                        Destroy(food);
                                        DataController.instance.gameData.FoodCount -= 1;
                                        feedplayer++;
                                    }
                                }
                                else
                                {
                                    if (DataController.instance.gameData.PlayerCount != 0)
                                    {
                                        GameObject cantfeedplayer1 = GameObject.FindGameObjectWithTag("Player");
                                        Destroy(cantfeedplayer1);
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

                    if (touch.name == "Wood(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "목재";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "가장 기본재료 나무를 벌목해 얻는다. 여러가지 제작품에 재료로 사용가능하다.";
                    }
                    else if (touch.name == "Stone(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "석재";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "가장 기본재료 암석을 채광해 얻는다. 여러가지 제작품에 재료로 사용가능하다. ";
                    }
                    else if (touch.name == "Tree(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "나무";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "벌목하면 목재를 얻을 수 있다. 지금은 아무것도 아니지";
                    }
                    else if (touch.name == "Rock(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "암석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "채광하면 석재를 얻을 수 있다. 지금은 너무 무겁지";
                    }
                    else if (touch.name == "House(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "집";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "카드의 한도를 늘려준다. 플레이어의 수를 늘릴수있다.";
                    }
                    else if (touch.name == "Banana(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "바나나";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "기본음식 그냥도 먹을 수 있지만 요리해서 먹으면 더욱 배부르다.";
                    }
                    else if (touch.name == "BananaTree(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "바나나나무";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "채집을 하면 바나나와 목재를 얻을수있다.";
                    }
                    else if (touch.name == "Brick(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "벽돌";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "돌을 가공해 만든 벽돌 튼튼하다.";
                    }
                    else if (touch.name == "Forge(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "용광로";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "철과 금을 제련할 수 있다.";
                    }
                    else if (touch.name == "Gold(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "금광석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "제련을 통해 빛나는 금괴로 만들 수 있다.";
                    }
                    else if (touch.name == "GoldIngot(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "금괴";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "비싸게 팔리는 금괴 다른 역할은?";
                    }
                    else if (touch.name == "Iron(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "철광석";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "제련을 통해 단단한 철괴를 만들 수 있다.";
                    }
                    else if (touch.name == "IronIngot(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "철괴";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "많은 것을 만들 수 있는 기본이면서 최강의 제료";
                    }
                    else if (touch.name == "Panel(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "판자";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 가공해 만드는 판때기 집만들때 사용한다";
                    }
                    else if (touch.name == "Player(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "주민";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "주민이 없으면 게임은 끝나버린다. 배가 고프지";
                    }
                    else if (touch.name == "Mine(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "벽돌공장";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "돌을 가공해 벽돌을 만드는 공장";
                    }
                    else if (touch.name == "Timber(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "제재소";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 가공해 판자를 만드는 공장";
                    }
                    else if (touch.name == "Branch(Clone)") 
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "나뭇가지";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "목재를 손질해 얻은 나뭇가지" + System.Environment.NewLine + "화로의 연료로 사용한다.";
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
                        DataController.instance.gameData.Skill = true;
                    }
                    else if (touch.name == "Rock(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        rockSkillBtn.SetActive(true);
                        WoodSkillBtn.SetActive(false);
                        treeSkillBtn.SetActive(false);
                        bananaTreeBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        DataController.instance.gameData.Skill = true;
                    }
                    else if (touch.name == "BananaTree(Clone)")
                    {
                        cardSkillUi.SetActive(true);
                        bananaTreeBtn.SetActive(true);
                        WoodSkillBtn.SetActive(false);
                        rockSkillBtn.SetActive(false);
                        treeSkillBtn.SetActive(false);
                        TimberSkillBtn.SetActive(false);
                        MineSkillBtn.SetActive(false);
                        HouseSkillBtn.SetActive(false);
                        DataController.instance.gameData.Skill = true;
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
                        DataController.instance.gameData.Skill = true;
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
                        DataController.instance.gameData.Skill = true;
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
                        DataController.instance.gameData.Skill = true;
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
                        DataController.instance.gameData.Skill = true;
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
            && Time.timeScale != 0 && DataController.instance.gameData.tuto == false)
        {
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
        FoodCount.GetComponent<TextMeshProUGUI>().text = "음식 : " + DataController.instance.gameData.FoodCount + "/" + (DataController.instance.gameData.PlayerCount * 3);
        StoreUpText.GetComponent<TextMeshProUGUI>().text = "상점 레벨 : " + DataController.instance.gameData.storeUpgrade;
        GoalText.GetComponent<TextMeshProUGUI>().text = "목표 : 금괴 10 / " + DataController.instance.gameData.GoldIngotCard;

        tutoBuyText.GetComponent<TextMeshProUGUI>().text = "3골드로 카드를 구매할수있다.";
        tutoCraftText.GetComponent<TextMeshProUGUI>().text = "재료를 모아 제작할 수 있다.";
        tutoDayText.GetComponent<TextMeshProUGUI>().text = "밤이 되었습니다. 제한된 카드보다 소유한 카드가 많다면 카드를 팔아야합니다." + System.Environment.NewLine + "또한 하루가 지날때마다 주민에게 음식을 줘야합니다." +
            System.Environment.NewLine + "음식이 부족하면 주민이 굶어 죽습니다.";
        tutoSellText.GetComponent<TextMeshProUGUI>().text = "카드를 팔 수 있습니다. 화면 위쪽 판매가 횔성화 되어있는지 확인할수있습니다. 조심하세요 카드를 누르면 판매됩니다.";
        tutoStoreUpText.GetComponent<TextMeshProUGUI>().text = "100골드로 상점을 업그레이드 할수있습니다. 새로운 재료가 나와요!";

        //StoreOver.GetComponent<TextMeshProUGUI>().text = "100골드가 필요합니다!";
        gameOver.GetComponent<TextMeshProUGUI>().text = "게임오버!" + System.Environment.NewLine + "모든 주민이 죽었습니다.";
        StartText.GetComponent<TextMeshProUGUI>().text = "목적을 달성했습니다." + System.Environment.NewLine + "이제 본게임으로";
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

}
