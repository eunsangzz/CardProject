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
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Tuto") DataController.instance.gameData.tuto = true;
        else DataController.instance.gameData.tuto = false;

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

        if(foodfull == true && slTimer.value > 0.0f)
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

        if (slTimer.value > 0.0f && DataController.instance.gameData.endDay == false && feed == false) //?????????????? ????????????
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
        else if (slTimer.value == 0.0f && Over == false) // ?????????????? ????????????????????????
        {
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
            if (DataController.instance.gameData.PlayerCount != 0)
            {
                DataController.instance.gameData.endDay = true;
                if (feedplayer != DataController.instance.gameData.PlayerCount)
                {
                    feed = true;
                }
                if (DataController.instance.gameData.CardCount >= DataController.instance.gameData.CardLimit) //???????? ??????????????????
                {
                    DataController.instance.gameData.Sell = true;
                    cardInfoUi.SetActive(false);
                    SellBtn.SetActive(false);
                    buyBtn.SetActive(false);
                    craftUiBtn.SetActive(false);
                    SellUi.SetActive(true);
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true) //???????????? ?????????????????? ???????????????? ???????? ??????????????????
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
                else //???????? ??????????????????
                {
                    if (feedplayer != DataController.instance.gameData.PlayerCount && feed == true) //???????????? ?????????????????? ???????????????? ???????? ??????????????????
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

                    if (touch.name == "Wood(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ???????????? ????????? ????????? ?????????. ???????????? ???????????? ????????? ??????????????????.";
                    }
                    else if (touch.name == "Stone(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ???????????? ????????? ????????? ?????????. ???????????? ???????????? ????????? ??????????????????. ";
                    }
                    else if (touch.name == "Tree(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "???????????? ????????? ?????? ??? ??????. ????????? ???????????? ?????????";
                    }
                    else if (touch.name == "Rock(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "???????????? ????????? ?????? ??? ??????. ????????? ?????? ?????????";
                    }
                    else if (touch.name == "House(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "???";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ????????????. ??????????????? ?????? ???????????????.";
                    }
                    else if (touch.name == "Banana(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "?????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "???????????? ????????? ?????? ??? ????????? ???????????? ????????? ?????? ????????????.";
                    }
                    else if (touch.name == "BananaTree(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "???????????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ?????? ???????????? ?????? ??? ??????.";
                    }
                    else if (touch.name == "Brick(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ????????? ?????? ?????? ????????????.";
                    }
                    else if (touch.name == "Forge(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "?????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ?????? ????????? ??? ??????.";
                    }
                    else if (touch.name == "Gold(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "?????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ?????? ????????? ????????? ?????? ??? ??????.";
                    }
                    else if (touch.name == "GoldIngot(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ?????? ?????? ??????????";
                    }
                    else if (touch.name == "Iron(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "?????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ?????? ????????? ????????? ?????? ??? ??????.";
                    }
                    else if (touch.name == "IronIngot(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ?????? ?????? ??? ?????? ??????????????? ????????? ??????";
                    }
                    else if (touch.name == "Panel(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ????????? ????????? ???????????? ????????????";
                    }
                    else if (touch.name == "Player(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "??????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ????????? ???????????????. ?????? ?????????";
                    }
                    else if (touch.name == "Mine(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "????????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "?????? ????????? ????????? ????????? ??????";
                    }
                    else if (touch.name == "Timber(Clone)")
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "?????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ????????? ????????? ??????";
                    }
                    else if (touch.name == "Branch(Clone)") 
                    {
                        cardInfoUi.SetActive(true);
                        CardNameText.GetComponent<TextMeshProUGUI>().text = "????????????";
                        CardInfoText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ?????? ????????????" + System.Environment.NewLine + "????????? ????????? ????????????.";
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
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
                        ForgeSkillBtn.SetActive(false);
                        DataController.instance.gameData.Skill = true;
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
            ErrorMessageText.GetComponent<TextMeshProUGUI>().text = "????????? ???????????????" + System.Environment.NewLine + "????????? 100????????? ???????????????!";
        }
    }

    private void LateUpdate()
    {
        WoodCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.WoodCard;
        StoneCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.StoneCard;
        IronCountText.GetComponent<TextMeshProUGUI>().text = "????????? : " + DataController.instance.gameData.IronCard;
        GoldCountText.GetComponent<TextMeshProUGUI>().text = "????????? : " + DataController.instance.gameData.GoldCard;
        GoldIngotCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.GoldIngotCard;
        IronIngotCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.IronIngotCard;
        BrickCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.BrickCard;
        PanelCountText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.PanelCard;
        BranchCountText.GetComponent<TextMeshProUGUI>().text = "???????????? : " + DataController.instance.gameData.BranchCard;

        GoldText.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.gold;
        CardCountText.GetComponent<TextMeshProUGUI>().text = "???????????? : " + DataController.instance.gameData.CardLimit + "/" + DataController.instance.gameData.CardCount;
        DayText.GetComponent<TextMeshProUGUI>().text = "????????? : " + DataController.instance.gameData.Day;
        FoodCount.GetComponent<TextMeshProUGUI>().text = "?????? : " + DataController.instance.gameData.FoodCount + "/" + DataController.instance.gameData.PlayerCount;
        StoreUpText.GetComponent<TextMeshProUGUI>().text = "?????? ?????? : " + DataController.instance.gameData.storeUpgrade;
        GoalText.GetComponent<TextMeshProUGUI>().text = "?????? : ?????? 1 / " + DataController.instance.gameData.GoldIngotCard;

        tutoBuyText.GetComponent<TextMeshProUGUI>().text = "3????????? ????????? ??????????????????.";
        tutoCraftText.GetComponent<TextMeshProUGUI>().text = "????????? ?????? ????????? ??? ??????.";
        tutoDayText.GetComponent<TextMeshProUGUI>().text = "?????? ???????????????. ????????? ???????????? ????????? ????????? ????????? ????????? ??????????????????." + System.Environment.NewLine + "?????? ????????? ??????????????? ???????????? ????????? ???????????????." +
            System.Environment.NewLine + "????????? ???????????? ????????? ?????? ????????????.";
        tutoSellText.GetComponent<TextMeshProUGUI>().text = "?????? ?????????"+ System.Environment.NewLine + "????????? ???????????????" + System.Environment.NewLine + "????????? ?????????" 
        + System.Environment.NewLine + "??? ??? ????????????";
        tutoStoreUpText.GetComponent<TextMeshProUGUI>().text = "100????????? ????????? ??????????????? ??????????????????. ????????? ????????? ?????????!";

        //StoreOver.GetComponent<TextMeshProUGUI>().text = "100????????? ???????????????!";
        gameOver.GetComponent<TextMeshProUGUI>().text = "????????????!" + System.Environment.NewLine + "?????? ?????????" + System.Environment.NewLine  + "????????? ?????? ?????? ???????????????.";
        StartText.GetComponent<TextMeshProUGUI>().text = "????????? ??????????????????." + System.Environment.NewLine + "?????? ???????????????";

        if (DataController.instance.gameData.QusetNum == 0)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "?????? ????????? ?????? ????????? ???????????????";
        }
        if(DataController.instance.gameData.QusetNum == 1)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "???????????? ?????? ??? ??????????????? ????????????";
        }
        if(DataController.instance.gameData.QusetNum == 2)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "??????????????? ???????????? ???????????? ???????????????";
        }
        if(DataController.instance.gameData.QusetNum == 3)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "???????????? ????????? ?????? ??????????????? ????????????";
        }
        if(DataController.instance.gameData.QusetNum == 4)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "?????? ?????? ????????? ????????????";
        }
        if(DataController.instance.gameData.QusetNum == 5)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "100????????? ?????? ????????? ??????????????? ?????????";
        }
        if(DataController.instance.gameData.QusetNum == 6)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "????????? ????????? ????????? ????????????";
        }
        if(DataController.instance.gameData.QusetNum == 7)
        {
            QuestText.GetComponent<TextMeshProUGUI>().text = "????????? ??????! ????????? ???????????????";
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

}
