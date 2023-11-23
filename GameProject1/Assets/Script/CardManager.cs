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


    bool tutobuy;
    bool tutosell;

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
        SellCard();
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
            if(DataController.instance.gameData.PlayerCount == 1 && DataController.instance.gameData.QusetNum > 2)
            {
                int rand1 = Random.Range(0,10);
                if(rand1 > 7)
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
            else if(DataController.instance.gameData.QusetNum != 1 && DataController.instance.gameData.QusetNum > 2)
            {
            int rand = Random.Range(0, 6);
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
            if (DataController.instance.gameData.QusetNum == 2)
            {
                DataController.instance.gameData.BananaTreeCard += 1;
                DataController.instance.gameData.gold -= 3;
                float randPosX = Random.Range(-5.0f, 5.0f);
                float randPosY = Random.Range(-4.0f, 2.0f);
                GameObject _Card = Instantiate(BasicCardSet[4], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            }
            if (DataController.instance.gameData.QusetNum == 0)
            {
                DataController.instance.gameData.QusetNum += 1;
                DataController.instance.gameData.gold -= 3;
                DataController.instance.gameData.WoodCard += 1;
                float randPosX = Random.Range(-5.0f, 5.0f);
                float randPosY = Random.Range(-4.0f, 2.0f);
                GameObject _Card = Instantiate(BasicCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            }
        }
        if (DataController.instance.gameData.storeUpgrade == 1 && DataController.instance.gameData.gold >= 3 && DataController.instance.gameData.QusetNum != 0)//업그레이드 없음
        {
            int rand = Random.Range(2, 10);
            float randPosX = Random.Range(-5.0f, 5.0f);
            float randPosY = Random.Range(-4.0f, 2.0f);
            GameObject _Card = Instantiate(BasicCardSet[rand], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BasicCardList.Add(_Card);
            if (rand == 2) DataController.instance.gameData.TreeCard += 1;
            if (rand == 3) DataController.instance.gameData.RockCard += 1;
            if (rand == 4) DataController.instance.gameData.BananaTreeCard += 1;
            if (rand == 5) DataController.instance.gameData.BananaCard += 1;
            if (rand == 6) DataController.instance.gameData.StrawBerryCard += 1;
            if (rand == 7) DataController.instance.gameData.StrawBerryTreeCard += 1;
            if (rand == 8) DataController.instance.gameData.IronCard += 1;
            if (rand == 9) DataController.instance.gameData.GoldCard += 1;
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

    public void CardSkill()
    {
        string Btn = EventSystem.current.currentSelectedGameObject.name;

        if (DataController.instance.gameData.Woker == 0)
        {
            wokerError.SetActive(true);
        }

        if (Btn == "Tree" && DataController.instance.gameData.TreeCard >= 1 && DataController.instance.gameData.Woker != 0)
        {
            StartCoroutine(delay(1));
        }

        if (Btn == "Rock" && DataController.instance.gameData.RockCard >= 1 && DataController.instance.gameData.Woker != 0)
        {
            StartCoroutine(delay(2));
        }

        if (Btn == "BananaTree" && DataController.instance.gameData.BananaTreeCard >= 1 && DataController.instance.gameData.Woker != 0) 
        {
            StartCoroutine(delay(3));
            
        }

        if (Btn == "Timber" && DataController.instance.gameData.WoodCard >= 2 && DataController.instance.gameData.BranchCard >= 1) //판자
        {
            if (DataController.instance.gameData.gold >= 1)
            {
                if (DataController.instance.gameData.QusetNum == 4)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                removeCard(1);
                StartCoroutine(delay(6));
            }
        }

        if (Btn == "Mine" && DataController.instance.gameData.StoneCard >= 2)
        {
            if (DataController.instance.gameData.gold >= 1)
            {
                if (DataController.instance.gameData.QusetNum == 4)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                removeCard(2);
                StartCoroutine(delay(7));
            }
        }

        if (Btn == "ForgeIron" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.IronCard >= 1
            && DataController.instance.gameData.BranchCard >= 2)
        {
            removeCard(1);
            StartCoroutine(delay(4));
        }

        if(Btn == "ForgeGold" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.GoldCard >= 1
            && DataController.instance.gameData.BranchCard >= 1)
        {
            removeCard(1);
            StartCoroutine(delay(5));
        }

        if(Btn == "Wood" && DataController.instance.gameData.Woker != 0)
        {
            StartCoroutine(delay(8));
        }

        if (Btn == "House" && DataController.instance.gameData.PlayerCount >= 2 && DataController.instance.gameData.gold > 15 && DataController.instance.gameData.Woker >= 2) 
        {
            StartCoroutine(delay(9));
            DataController.instance.gameData.gold -= 15;
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

    void removeCard(int i)
    {
        switch(i)
        {
            case 1: //나무 삭제
                GameObject _delCard1 = GameObject.Find("Wood(Clone)");
                Destroy(_delCard1);
                break;
            case 2:
                GameObject _delCard2 = GameObject.Find("Stone(Clone)");
                Destroy(_delCard2);
                break;
            case 3:
                GameObject _delCard3 = GameObject.Find("Tree(Clone)");
                Destroy(_delCard3);
                break;
            case 4:
                GameObject _delCard4 = GameObject.Find("Rock(Clone)");
                Destroy(_delCard4);
                break;
            case 5:
                GameObject _delCard5 = GameObject.Find("BananaTree(Clone)");
                Destroy(_delCard5);
                break;
            case 6:
                GameObject _delCard6 = GameObject.Find("Iron(Clone)");
                Destroy(_delCard6);
                break;
            case 7:
                GameObject _delCard7 = GameObject.Find("Gold(Clone)");
                Destroy(_delCard7);
                break;
            case 8:
                GameObject _delCard8 = GameObject.Find("Branch(Clone)");
                Destroy(_delCard8);
                break;
        }

    }

    IEnumerator delay(int i)
    {
        DataController.instance.gameData.Skill = false;
        if (i < 8)
        { Woker += 1;}
        else
        { Woker += 2; }

        if (i == 1) //나무
        {
            removeCard(3);
            DataController.instance.gameData.TreeCard -= 1;

            yield return new WaitForSeconds(2.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);

            GameObject _Card1 = Instantiate(BasicCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.WoodCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);

            _Card1 = Instantiate(BasicCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.WoodCard += 1;
        }

        if (i == 2) // 돌
        {
            removeCard(4);

            DataController.instance.gameData.RockCard -= 1;

            yield return new WaitForSeconds(2.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);

            GameObject _Card1 = Instantiate(BasicCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.StoneCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);

            _Card1 = Instantiate(BasicCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.StoneCard += 1;
        }

        if (i == 3) //바나나
        {
            removeCard(5);
            DataController.instance.gameData.BananaTreeCard -= 1;

            yield return new WaitForSeconds(2.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);

            GameObject _Card1 = Instantiate(BasicCardSet[5], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BananaCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);

            _Card1 = Instantiate(BasicCardSet[5], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BananaCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);

            _Card1 = Instantiate(BasicCardSet[5], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BananaCard += 1;

            if (DataController.instance.gameData.QusetNum == 2)
            {
                DataController.instance.gameData.QusetNum += 1;
            }
        }

        if (i == 4) //은
        {
            removeCard(1);
            removeCard(6);
            removeCard(8);
            DataController.instance.gameData.WoodCard -= 2;
            DataController.instance.gameData.BranchCard -= 1;
            DataController.instance.gameData.IronIngotCard -= 1;

            yield return new WaitForSeconds(5.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[2], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.IronCard += 1;
        }

        if (i == 5) //금
        {
            removeCard(1);
            removeCard(7);
            removeCard(8);

            DataController.instance.gameData.WoodCard -= 2;
            DataController.instance.gameData.BranchCard -= 1;
            DataController.instance.gameData.GoldCard -= 1;

            yield return new WaitForSeconds(5.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.GoldIngotCard += 1;
        }

        if (i == 6) //판자
        {
            removeCard(1);
            removeCard(8);
            DataController.instance.gameData.WoodCard -= 2;
            DataController.instance.gameData.BranchCard -= 1;

            if (DataController.instance.gameData.QusetNum == 6)
            {
                DataController.instance.gameData.QusetNum += 1;
            }

            yield return new WaitForSeconds(5.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.PanelCard += 1;
        }
        
        if (i == 7) //벽돌
        {
            removeCard(2);
            DataController.instance.gameData.StoneCard -= 2;

            yield return new WaitForSeconds(5.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BrickCard += 1;
        }

        if (i == 8) //나뭇가지
        {
            removeCard(1);
            DataController.instance.gameData.WoodCard -= 2;
            yield return new WaitForSeconds(2.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(IntermediatCardSet[4], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BranchCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);
            _Card1 = Instantiate(IntermediatCardSet[4], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BranchCard += 1;

            yield return new WaitForSeconds(2.0f);

            randPosX = Random.Range(-5f, 5f);
            randPosY = Random.Range(-4f, 2f);
            _Card1 = Instantiate(IntermediatCardSet[4], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.BranchCard += 1;

            if (DataController.instance.gameData.QusetNum == 1)
            {
                DataController.instance.gameData.QusetNum += 1;
            }
        }

        if (i == 9)//주민
        {
            yield return new WaitForSeconds(60.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            GameObject _Card1 = Instantiate(playerPre, new Vector3(randPosX, randPosY, 0), Quaternion.identity);
            DataController.instance.gameData.PlayerCount += 1;
        }
        if (i < 8) { Woker -= 1; }
        else { Woker -= 2; }
        StopCoroutine(delay(0));
        yield return null;
    }
}
