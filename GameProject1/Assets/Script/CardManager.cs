using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    public GameObject PlayerCard;
    public GameObject[] BasicCardSet = new GameObject[8];
    public GameObject[] IntermediatCardSet = new GameObject[5];
    public GameObject playerPre;
    public GameObject SellUI;

    public GameObject tutoSell;
    public GameObject tutoBuy;
    public GameObject tutoUi;
    public GameObject tutoBtnUi;

    bool mine1 = false;
    int mine2 = 0;

    bool ironforge1 = false;
    int ironforge2 = 0;

    bool goldforge1 = false;
    int goldforge2 = 0;

    bool Timber1 = false;
    int Timber2 = 0;

    bool tutobuy;
    bool tutosell;

    float delayTime = 0;
    bool tree;
    bool rock;
    bool bananatree;
    bool ironforge;
    bool goldforge;
    bool timber;
    bool mine;
    bool wood;
    bool house;

    int treecre = 0;
    int rockcre = 0;
    int bananacre = 0;
    int woodcre = 0;

    int Woker = 0;

    public GameObject wokerError;
    //카드 구매버튼 눌렀을때 저장해둔 프리팹중에 랜덤으로 하나 생성 
    //구매 버튼 업그레이드 적용해서 1단계 나무 돌 2단계 철 금 등등 으로 세팅

    private void Start()
    {
        tutobuy = false;
        tutosell = false;
        GameObject _Card = Instantiate(PlayerCard, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void Update()
    {
        DataController.instance.gameData.Woker = DataController.instance.gameData.PlayerCount - Woker;
        if(tree == true || rock == true || bananatree == true || ironforge ==true || goldforge == true || timber == true || mine == true || wood == true || house == true)
        {
            delayTime += Time.deltaTime;
        }        
        if(tree == true && delayTime >= 2)
        {
            if(treecre == 0)
            {
                CreatCard(1);
                treecre += 1;
            }
            if(delayTime >=4)
            {
                if(DataController.instance.gameData.QusetNum == 1)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                CreatCard(1);
                delayTime = 0;
                Woker = -1;
                tree = false;
                treecre = 0;
            }
        }
        if(rock == true && delayTime >= 2)
        {
            if(rockcre == 0)
            {
                CreatCard(2);
                rockcre += 1;
            };
            if (delayTime >= 4 && rockcre == 1) 
            {
                CreatCard(2);
                delayTime = 0;
                Woker = -1;
                rock = false;
                rockcre = 0;
            }
        }
        if (bananatree == true && delayTime >= 2)
        {
            if(bananacre == 0)
            {
                CreatCard(3);
                bananacre += 1;
            }
            if(delayTime >= 4 && bananacre == 1)
            {
                CreatCard(3);
                bananacre += 1;
            }
            if(delayTime >= 6 && bananacre ==2)
            {
                if(DataController.instance.gameData.QusetNum == 2)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                CreatCard(3);
                bananatree = false;
                Woker = -1;
                bananacre = 0;
                delayTime = 0;
            }
        }
        if(goldforge == true && delayTime >= 15)
        {
            if(DataController.instance.gameData.QusetNum == 6)
            {
                DataController.instance.gameData.QusetNum += 1;
            }
            goldforge = false;
            CreatCard(4);
            Woker = -1;
            delayTime = 0;
        }
        if (ironforge == true && delayTime >= 15) 
        {
            ironforge = false;
            CreatCard(5);
            Woker = -1;
            delayTime = 0;
        }
        if(timber == true && delayTime >= 5)
        {
            timber = false;
            CreatCard(6);
            Woker = -1;
            delayTime = 0;
        }
        if (mine == true && delayTime >= 5)
        {
            mine = false;
            CreatCard(7);
            Woker = -1;
            delayTime = 0;
        }
        if (wood == true && delayTime >= 2)
        {
            if(woodcre == 0)
            {
                CreatCard(8);
                woodcre += 1;
            }
            if(delayTime >=4 && woodcre == 1)
            {
                CreatCard(8);
                woodcre += 1;
            }    
            if(delayTime >= 6 && woodcre == 2)
            {
                if (DataController.instance.gameData.QusetNum == 1)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                CreatCard(8);
                Woker = -1;
                woodcre = 0;
                delayTime = 0;
                wood = false;
            }
        }
        if(house == true && delayTime >= 60)
        {
            house = false;
            Woker = -1;
            CreatCard(9);
            delayTime = 0;
        }

        SellCard();
        if(mine1 == true && mine2 < 2) // mine
        {
            mine2 += 1;
            GameObject _delCard1 = GameObject.Find("Stone(Clone)");
            Destroy(_delCard1);
            if(mine2 == 2)
            {
                mine1 = false;
            }
        }
        if (ironforge1 == true && ironforge2 < 2)
        {
            if(ironforge2 == 0)
            {
                GameObject _delCard1 = GameObject.Find("Branch(Clone)");
                Destroy(_delCard1);
                GameObject _delCard2 = GameObject.Find("Iron(Clone)");
                Destroy(_delCard2);
            }
            GameObject _delCard3 = GameObject.Find("Wood(Clone)");
            Destroy(_delCard3);
            if(ironforge2 == 1)
            {
                ironforge1 = false;
            }
            ironforge2 += 1;
        }
        if (goldforge1 == true && goldforge2 < 2)
        {
            if (goldforge2 == 0)
            {
                GameObject _delCard1 = GameObject.Find("Branch(Clone)");
                Destroy(_delCard1);
                GameObject _delCard2 = GameObject.Find("Gold(Clone)");
                Destroy(_delCard2);
            }
            GameObject _delCard3 = GameObject.Find("Wood(Clone)");
            Destroy(_delCard3);
            if (goldforge2 == 1)
            {
                goldforge1 = false;
            }
            goldforge2 += 1;
        }
        if(Timber1 == true && Timber2 < 2)
        {
            if(Timber2 == 0)
            {
                GameObject _delCard1 = GameObject.Find("Branch(Clone)");
                Destroy(_delCard1);
                GameObject _delCard3 = GameObject.Find("Wood(Clone)");
                Destroy(_delCard3);
            }
            if(Timber2 == 1)
            {
                GameObject _delCard3 = GameObject.Find("Wood(Clone)");
                Destroy(_delCard3);
            }
            Timber2 += 1;
        }
    }

    public void CardBuy()//카드 살때
    {
        if(DataController.instance.gameData.tuto == true && tutobuy == false)
        {
            Time.timeScale =0;
            tutoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoBuy.SetActive(true);
            tutobuy = true; 
        }
        if (DataController.instance.gameData.storeUpgrade == 0 && DataController.instance.gameData.gold >= 3 && Time.timeScale != 0 )//업그레이드 없음
        {
            if(DataController.instance.gameData.PlayerCount == 1 && DataController.instance.gameData.QusetNum != 0)
            {
                int rand1 = Random.Range(0,10);
                if(rand1 > 9)
                {
                    float randPosX = Random.Range(-5.0f, 5.0f);
                    float randPosY = Random.Range(-4.0f, 2.0f);
                    GameObject _Card = Instantiate(playerPre, new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                    DataController.instance.gameData.PlayerCount += 1;
                }
                else
                {
                    int rand = Random.Range(0, 5);
                    float randPosX = Random.Range(-5.0f, 5.0f);
                    float randPosY = Random.Range(-4.0f, 2.0f);
                    GameObject _Card = Instantiate(BasicCardSet[rand], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                    if (rand == 0) DataController.instance.gameData.WoodCard += 1;
                    if (rand == 1) DataController.instance.gameData.StoneCard += 1;
                    if (rand == 2) DataController.instance.gameData.TreeCard += 1;
                    if (rand == 3) DataController.instance.gameData.RockCard += 1;
                    if (rand == 4) DataController.instance.gameData.BananaTreeCard += 1;
                    if (rand == 5) DataController.instance.gameData.BananaCard += 1;
                    DataController.instance.gameData.gold -= 3;
                }
            }
            else if(DataController.instance.gameData.QusetNum != 1 && DataController.instance.gameData.QusetNum != 0)
            {
            int rand = Random.Range(0, 5);
            float randPosX = Random.Range(-5.0f, 5.0f);
            float randPosY = Random.Range(-4.0f, 2.0f);
            GameObject _Card = Instantiate(BasicCardSet[rand], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BasicCardList.Add(_Card);
            if (rand == 0) DataController.instance.gameData.WoodCard += 1;
            if (rand == 1) DataController.instance.gameData.StoneCard += 1;
            if (rand == 2) DataController.instance.gameData.TreeCard += 1;
            if (rand == 3) DataController.instance.gameData.RockCard += 1;
            if (rand == 4) DataController.instance.gameData.BananaTreeCard += 1;
            if (rand == 5) DataController.instance.gameData.BananaCard += 1;
            DataController.instance.gameData.gold -= 3;
            }
            if (DataController.instance.gameData.QusetNum == 0)
            {
                DataController.instance.gameData.QusetNum += 1;
                float randPosX = Random.Range(-5.0f, 5.0f);
                float randPosY = Random.Range(-4.0f, 2.0f);
                GameObject _Card = Instantiate(BasicCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            }
        }
        if (DataController.instance.gameData.storeUpgrade == 1 && DataController.instance.gameData.gold >= 3 && DataController.instance.gameData.QusetNum != 0)//업그레이드 없음
        {
            int rand = Random.Range(0, 5);
            float randPosX = Random.Range(-5.0f, 5.0f);
            float randPosY = Random.Range(-4.0f, 2.0f);
            GameObject _Card = Instantiate(BasicCardSet[rand], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BasicCardList.Add(_Card);
            if (rand == 0) DataController.instance.gameData.WoodCard += 1;
            if (rand == 1) DataController.instance.gameData.StoneCard += 1;
            if (rand == 2) DataController.instance.gameData.TreeCard += 1;
            if (rand == 3) DataController.instance.gameData.RockCard += 1;
            if (rand == 4) DataController.instance.gameData.BananaTreeCard += 1;
            if (rand == 5) DataController.instance.gameData.BananaCard += 1;
            if (rand == 6) DataController.instance.gameData.IronCard += 1;
            if (rand == 7) DataController.instance.gameData.GoldCard += 1;
            DataController.instance.gameData.gold -= 3;
        }

    }

    public void Cheat()
    {
        float randPosX = Random.Range(-5.0f, 5.0f);
        float randPosY = Random.Range(-4.0f, 2.0f);
        GameObject _Card = Instantiate(IntermediatCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        DataController.instance.gameData.BasicCardList.Add(_Card);
        DataController.instance.gameData.GoldIngotCard += 1;
    }

    public void SellActive()
    {
        if(DataController.instance.gameData.tuto == true && tutosell == false)
        {
            Time.timeScale =0;
            tutoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoSell.SetActive(true);
            tutosell = true;
        }
        if (DataController.instance.gameData.Sell == false && Time.timeScale != 0)
        {
            DataController.instance.gameData.Sell = true;
        }
        else if (DataController.instance.gameData.Sell == true)
        {
            DataController.instance.gameData.Sell = false;
        }
    }

    public void endDaySellCard()
    {
        if (DataController.instance.gameData.endDay == true)
        {
            DataController.instance.gameData.Sell = true;
        }
    }

    private void SellCard()
    {
        if (Input.GetMouseButton(0))
        {
            if (DataController.instance.gameData.Sell == true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touch = hit.transform.gameObject;
                    Destroy(touch);
                    if (touch.name == "Wood(Clone)")
                    {
                        DataController.instance.gameData.gold += 2;
                        DataController.instance.gameData.WoodCard -= 1;
                    }
                    if (touch.name == "Stone(Clone)")
                    {
                        DataController.instance.gameData.gold += 2;
                        DataController.instance.gameData.StoneCard -= 1;
                    }
                    if (touch.name == "Tree(Clone)")
                    {
                        DataController.instance.gameData.gold += 2;
                        DataController.instance.gameData.TreeCard -= 1;
                    }
                    if (touch.name == "Rock(Clone)")
                    {
                        DataController.instance.gameData.gold += 2;
                        DataController.instance.gameData.RockCard -= 1;
                    }
                    if (touch.name == "BananaTree(Clone)")
                    {
                        DataController.instance.gameData.gold += 2;
                        DataController.instance.gameData.BananaTreeCard -= 1;
                    }
                    if (touch.name == "Banana(Clone)")
                    {
                        DataController.instance.gameData.gold += 1;
                        DataController.instance.gameData.BananaCard -= 1;
                    }
                    if (touch.name == "House(Clone)")
                    {
                        DataController.instance.gameData.gold += 15;
                        DataController.instance.gameData.HouseCard -= 1;
                    }
                    if(touch.name == "Player(Clone)")
                    {
                        DataController.instance.gameData.gold += 5;
                        DataController.instance.gameData.PlayerCount -= 1;
                    }
                    if(touch.name == "Branch(Clone)")
                    {
                        DataController.instance.gameData.BranchCard -= 1;
                        DataController.instance.gameData.gold += 3;
                    }
                    if (touch.name == "Brick(Clone)")
                    {
                        DataController.instance.gameData.BrickCard -= 1;
                        DataController.instance.gameData.gold += 5;
                    }
                    if (touch.name == "Forge(Clone)")
                    {
                        DataController.instance.gameData.ForgeCard -= 1;
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "Gold(Clone)")
                    {
                        DataController.instance.gameData.GoldCard -= 1;
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "GoldIngot(Clone)")
                    {
                        DataController.instance.gameData.GoldIngotCard -= 1;
                        DataController.instance.gameData.gold += 20;
                    }
                    if (touch.name == "IronIngot(Clone)")
                    {
                        DataController.instance.gameData.IronIngotCard -= 1;
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "Iron(Clone)")
                    {
                        DataController.instance.gameData.IronCard -= 1;
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Mine(Clone)")
                    {
                        DataController.instance.gameData.MineCard -= 1;
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Timber(Clone)")
                    {
                        DataController.instance.gameData.TimberCard -= 1;
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Panel(Clone)")
                    {
                        DataController.instance.gameData.PanelCard -= 1;
                        DataController.instance.gameData.gold += 6;
                    }
                }
            }
        }
    }
    void CreatCard(int i)
    {
        if (i == 1)//목재
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(BasicCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 2)//석재
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(BasicCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 3)//바나나
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(BasicCardSet[5], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 4)//금괴
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 5)//은괴
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[2], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 6)//판자
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 7) //벽돌
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 8)//나뭇가지
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[4], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
        if (i == 9)//주민
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(playerPre, new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        }
    }

    public void CardSkill()
    {
        string Btn = EventSystem.current.currentSelectedGameObject.name;
        if(DataController.instance.gameData.Woker == 0)
        {
            wokerError.SetActive(true);
        }
        if (Btn == "Tree" && DataController.instance.gameData.TreeCard >= 1 && DataController.instance.gameData.Woker != 0)
        {
            tree = true;
            GameObject _delCard1 = GameObject.Find("Tree(Clone)");
            Destroy(_delCard1);

            Woker += 1;
            DataController.instance.gameData.WoodCard += 2;
            DataController.instance.gameData.TreeCard -= 1;
            DataController.instance.gameData.Skill = false;
        }

        if (Btn == "Rock" && DataController.instance.gameData.RockCard >= 1 && DataController.instance.gameData.Woker != 0)
        {
            rock = true;
            GameObject _delRockCard1 = GameObject.Find("Rock(Clone)");
            Destroy(_delRockCard1);

            Woker += 1;
            DataController.instance.gameData.StoneCard += 2;
            DataController.instance.gameData.RockCard -= 1;
            DataController.instance.gameData.Skill = false;
        }

        if (Btn == "BananaTree" && DataController.instance.gameData.BananaTreeCard >= 1 && DataController.instance.gameData.Woker != 0) 
        {
            bananatree = true;
            GameObject _delCard1 = GameObject.Find("BananaTree(Clone)");
            Destroy(_delCard1);

            Woker += 1;
            DataController.instance.gameData.BananaCard += 3;
            DataController.instance.gameData.BananaTreeCard -= 1;
            DataController.instance.gameData.Skill = false;
        }
        if (Btn == "Timber" && DataController.instance.gameData.WoodCard >= 2 && DataController.instance.gameData.BranchCard >= 1) 
        {
            Debug.Log("timber");
            if (DataController.instance.gameData.gold >= 1)
            {
                Debug.Log("timber");
                if (DataController.instance.gameData.QusetNum == 3)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                DataController.instance.gameData.gold -= 1;
                timber = true;
                Timber1 = true;
                Timber2 = 0;
                DataController.instance.gameData.WoodCard -= 2;
                DataController.instance.gameData.BranchCard -= 1;
                DataController.instance.gameData.PanelCard += 1;
                DataController.instance.gameData.Skill = false;
            }
        }
        if(Btn == "Mine" && DataController.instance.gameData.StoneCard >= 2)
        {
            Debug.Log("timber");
            if (DataController.instance.gameData.gold >= 1)
            {
                Debug.Log("mine");
                if (DataController.instance.gameData.QusetNum == 3)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                DataController.instance.gameData.gold -= 1;
                mine = true;
                mine1 = true;
                mine2 = 0;
                DataController.instance.gameData.StoneCard -= 2;
                DataController.instance.gameData.BrickCard += 1;
                DataController.instance.gameData.Skill = false;
            }
        }
        if (Btn == "ForgeIron" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.IronCard >= 1
            && DataController.instance.gameData.BranchCard >= 1)
        {
            ironforge = true;

            ironforge1 = true;
            ironforge2 = 0;
            DataController.instance.gameData.WoodCard -= 2;
            DataController.instance.gameData.IronCard -= 1;
            DataController.instance.gameData.IronIngotCard += 1;
            DataController.instance.gameData.Skill = false;
        }
        if(Btn == "ForgeGold" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.GoldCard >= 1
            && DataController.instance.gameData.BranchCard >= 1)
        {
            goldforge = true;

            goldforge1 = true;
            goldforge2 = 0;
            DataController.instance.gameData.WoodCard -= 2;
            DataController.instance.gameData.GoldCard -= 1;
            DataController.instance.gameData.GoldIngotCard += 1;
            DataController.instance.gameData.Skill = false;
        }
        if(Btn == "Wood" && DataController.instance.gameData.Woker != 0)
        {
            wood = true;
            GameObject _delCard1 = GameObject.Find("Wood(Clone)");
            Destroy(_delCard1);

            Woker += 1;
            DataController.instance.gameData.WoodCard -= 1;
            DataController.instance.gameData.BranchCard += 2;
            DataController.instance.gameData.Skill = false;
        }
        if (Btn == "House" && DataController.instance.gameData.PlayerCount >= 2 && DataController.instance.gameData.gold > 15 && DataController.instance.gameData.Woker >= 2) 
        {
            house = true;

            Woker += 2;
            DataController.instance.gameData.PlayerCount += 1;
            DataController.instance.gameData.gold -= 15;
            DataController.instance.gameData.Skill = false;
        }
    }

    public void WokerErrorClose()
    {
        wokerError.SetActive(false);
    }

    public void StoreUpgrade()
    {
        if (DataController.instance.gameData.storeUpgrade == 0 && DataController.instance.gameData.gold >= 30)
        {
            DataController.instance.gameData.storeUpgrade += 1;
        }
        if (DataController.instance.gameData.storeUpgrade == 1 && DataController.instance.gameData.gold >= 60)
        {
            DataController.instance.gameData.storeUpgrade += 1;
        }
    }
}
