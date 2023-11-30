using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    bool sell;

    public GameObject wokerError;
    //카드 구매버튼 눌렀을때 저장해둔 프리팹중에 랜덤으로 하나 생성 
    //구매 버튼 업그레이드 적용해서 1단계 나무 돌 2단계 철 금 등등 으로 세팅

    private void Start()
    {
        tutobuy = false;
        tutosell = false;
        DataController.instance.gameData.Card.Add(Instantiate(PlayerCard, new Vector3(0, 0, 0), Quaternion.identity));
    }

    private void Update()
    {
        SellCard();
    }


    public void CardBuy()//카드 살때
    {
        if (DataController.instance.gameData.tuto == true && tutobuy == false) //튜토리얼
        {
            Time.timeScale = 0;
            tutoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoBuy.SetActive(true);
            tutobuy = true;
        }

        if (DataController.instance.gameData.storeUpgrade == 0 && DataController.instance.gameData.gold >= 3 && Time.timeScale != 0)//업그레이드 없음
        {
            if (DataController.instance.gameData.PlayerCount == 1 && DataController.instance.gameData.QusetNum > 2) //필드 내 플레이어가 1명일때
            {
                int rand1 = Random.Range(0, 10);
                if (rand1 > 7) // 랜덤값이 7초과면 플레이어 카드 생성
                {
                    float randPosX = Random.Range(-5.0f, 5.0f);
                    float randPosY = Random.Range(-4.0f, 2.0f);
                    DataController.instance.gameData.Card.Add(Instantiate(playerPre, new Vector3(randPosX, randPosY, 0), Quaternion.identity));
                    DataController.instance.gameData.PlayerCount += 1;
                    DataController.instance.gameData.Woker += 1;
                }
                else // 랜덤값이 7이하일때 
                {
                    int rand = Random.Range(0, 5);
                    CreateCard(0, rand);
                    DataController.instance.gameData.addCardCount(rand);
                    DataController.instance.gameData.gold -= 3;
                }
            }

            else if (DataController.instance.gameData.QusetNum != 1 && DataController.instance.gameData.QusetNum > 2) //필드내 플레이어가 2명이상 일때
            {
                int rand = Random.Range(0, 8); //랜덤값으로 카드 생성
                CreateCard(0, rand);
                DataController.instance.gameData.addCardCount(rand);
                DataController.instance.gameData.gold -= 3;
            }

            if (DataController.instance.gameData.QusetNum == 2)
            {
                DataController.instance.gameData.BananaTreeCard += 1;
                DataController.instance.gameData.gold -= 3;
                CreateCard(0, 4);
            }

            if (DataController.instance.gameData.QusetNum == 0)
            {
                DataController.instance.gameData.QusetNum += 1;
                DataController.instance.gameData.gold -= 3;
                DataController.instance.gameData.WoodCard += 1;
                CreateCard(0, 0);
            }
        }
        if (DataController.instance.gameData.storeUpgrade == 1 && DataController.instance.gameData.gold >= 3 && DataController.instance.gameData.QusetNum != 0)//업그레이드 없음
        {
            int rand = Random.Range(2, 10);
            CreateCard(0, rand);
            DataController.instance.gameData.addCardCount(rand);
            DataController.instance.gameData.gold -= 3;
        }

    }

    public void Cheat()
    {
        float randPosX = Random.Range(-5.0f, 5.0f);
        float randPosY = Random.Range(-4.0f, 2.0f);
        GameObject _Card = Instantiate(IntermediatCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
        DataController.instance.gameData.GoldIngotCard += 1;
    }

    public void SellActive()
    {
        if (DataController.instance.gameData.tuto == true && tutosell == false)
        {
            Time.timeScale = 0;
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
        if (DataController.instance.gameData.Sell == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touch = hit.transform.gameObject;
                    if (touch.name == "Wood(Clone)")
                    {
                        removeCard(0);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "Stone(Clone)")
                    {
                        removeCard(1);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "Tree(Clone)")
                    {
                        removeCard(2);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "Rock(Clone)")
                    {
                        removeCard(3);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "BananaTree(Clone)")
                    {
                        removeCard(4);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "Banana(Clone)")
                    {
                        removeCard(5);
                        DataController.instance.gameData.gold += 1;
                    }
                    if (touch.name == "StrawBerry(Clone)")
                    {
                        removeCard(6);
                        DataController.instance.gameData.gold += 1;
                    }
                    if (touch.name == "StrawBerryTree(Clone)")
                    {
                        removeCard(7);
                        DataController.instance.gameData.gold += 2;
                    }
                    if (touch.name == "Iron(Clone)")
                    {
                        removeCard(8);
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Gold(Clone)")
                    {
                        removeCard(9);
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "Branch(Clone)")
                    {
                        Debug.Log("sell branch");
                        removeCard(10);
                        DataController.instance.gameData.gold += 3;
                    }
                    if (touch.name == "IronIngot(Clone)")
                    {
                        removeCard(11);
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "GoldIngot(Clone)")
                    {
                        removeCard(12);
                        DataController.instance.gameData.gold += 20;
                    }
                    if (touch.name == "Brick(Clone)")
                    {
                        removeCard(13);
                        DataController.instance.gameData.gold += 5;
                    }
                    if (touch.name == "Panel(Clone)")
                    {
                        removeCard(14);
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "House(Clone)")
                    {
                        removeCard(15);
                        DataController.instance.gameData.gold += 15;
                    }
                    if (touch.name == "Forge(Clone)")
                    {
                        removeCard(16);
                        DataController.instance.gameData.gold += 6;
                    }
                    if (touch.name == "Timber(Clone)")
                    {
                        removeCard(17);
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Mine(Clone)")
                    {
                        removeCard(18);
                        DataController.instance.gameData.gold += 8;
                    }
                    if (touch.name == "Kitchen(Clone)")
                    {
                        removeCard(19);
                        DataController.instance.gameData.gold += 5;
                    }
                    if (touch.name == "Player(Clone)")
                    {
                        removeCard(20);
                        DataController.instance.gameData.gold += 5;
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
        else
        {
            if (Btn == "Tree" && DataController.instance.gameData.Woker != 0)
            {
                StartCoroutine(delay(1));
            }

            if (Btn == "Wood" && DataController.instance.gameData.Woker != 0)
            {
                StartCoroutine(delay(8));
            }

            if (Btn == "Rock" && DataController.instance.gameData.Woker != 0)
            {
                StartCoroutine(delay(2));
            }

            if (Btn == "BananaTree" && DataController.instance.gameData.Woker != 0)
            {
                StartCoroutine(delay(3));
                if (DataController.instance.gameData.QusetNum == 2)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
            }

            if (Btn == "StrawBerryTree" && DataController.instance.gameData.Woker != 0)
            {
                StartCoroutine(delay(9));
            }

            if (Btn == "Timber" && DataController.instance.gameData.WoodCard >= 2 && DataController.instance.gameData.BranchCard >= 1
                && DataController.instance.gameData.Woker != 0) //판자
            {
                if (DataController.instance.gameData.gold >= 1)
                {
                    if (DataController.instance.gameData.QusetNum == 4)
                    {
                        DataController.instance.gameData.QusetNum += 1;
                    }
                    StartCoroutine(delay(6));
                }
            }

            if (Btn == "Mine" && DataController.instance.gameData.StoneCard >= 2 && DataController.instance.gameData.Woker != 0)
            {
                if (DataController.instance.gameData.gold >= 1)
                {
                    if (DataController.instance.gameData.QusetNum == 4)
                    {
                        DataController.instance.gameData.QusetNum += 1;
                    }
                    StartCoroutine(delay(7));
                }
            }

            if (Btn == "ForgeIron" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.IronCard >= 1
                && DataController.instance.gameData.BranchCard >= 2)
            {
                StartCoroutine(delay(4));
            }

            if (Btn == "ForgeGold" && DataController.instance.gameData.WoodCard >= 2 & DataController.instance.gameData.GoldCard >= 1
                && DataController.instance.gameData.BranchCard >= 1)
            {
                StartCoroutine(delay(5));
            }

            if (Btn == "House" && DataController.instance.gameData.PlayerCount >= 2 && DataController.instance.gameData.gold > 15
                && DataController.instance.gameData.Woker >= 2)
            {
                StartCoroutine(delay(10));
                DataController.instance.gameData.gold -= 15;
            }
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

    IEnumerator delay(int i)
    {
        DataController.instance.gameData.Skill = false;

        if (i < 10) { DataController.instance.gameData.Woker -= 1; }
        else { DataController.instance.gameData.Woker -= 2; }

        if (i == 1) //나무
        {
            removeCard(2);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 0);
            DataController.instance.gameData.addCardCount(0);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 0);
            DataController.instance.gameData.addCardCount(0);
        }

        if (i == 2) // 돌
        {
            removeCard(3);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 1);
            DataController.instance.gameData.addCardCount(1);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 1);
            DataController.instance.gameData.addCardCount(1);
        }

        if (i == 3) //바나나
        {
            removeCard(4);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 5);
            DataController.instance.gameData.addCardCount(5);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 5);
            DataController.instance.gameData.addCardCount(5);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 5);
            DataController.instance.gameData.addCardCount(5);
        }

        if (i == 4) //은
        {
            removeCard(1);
            removeCard(6);
            removeCard(8);

            yield return new WaitForSeconds(5.0f);

            CreateCard(1, 2);
            DataController.instance.gameData.addCardCount(11);
        }

        if (i == 5) //금
        {
            removeCard(1);
            removeCard(1);
            removeCard(7);
            removeCard(8);

            yield return new WaitForSeconds(5.0f);

            CreateCard(1, 3);
            DataController.instance.gameData.addCardCount(12);
        }

        if (i == 6) //판자
        {
            for (int u = 0; u < 2; u++)
            {
                removeCard(1);
            }
            removeCard(8);

            if (DataController.instance.gameData.QusetNum == 6)
            {
                DataController.instance.gameData.QusetNum += 1;
            }

            yield return new WaitForSeconds(5.0f);

            CreateCard(1, 0);
            DataController.instance.gameData.addCardCount(14);
        }

        if (i == 7) //벽돌
        {
            for (int u = 0; u < 2; u++)
            {
                removeCard(2);
            }

            yield return new WaitForSeconds(5.0f);

            CreateCard(1, 1);
            DataController.instance.gameData.addCardCount(13);
        }

        if (i == 8) //나뭇가지
        {
            removeCard(0);

            yield return new WaitForSeconds(2.0f);

            CreateCard(1, 4);
            DataController.instance.gameData.addCardCount(10);

            yield return new WaitForSeconds(2.0f);

            CreateCard(1, 4);
            DataController.instance.gameData.addCardCount(10);

            yield return new WaitForSeconds(2.0f);

            CreateCard(1, 4);
            DataController.instance.gameData.addCardCount(10);

            if (DataController.instance.gameData.QusetNum == 1)
            {
                DataController.instance.gameData.QusetNum += 1;
            }
        }

        if (i == 10)//주민
        {
            yield return new WaitForSeconds(60.0f);

            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            DataController.instance.gameData.Card.Add(Instantiate(playerPre, new Vector3(randPosX, randPosY, 0), Quaternion.identity));
            DataController.instance.gameData.PlayerCount += 1;
            DataController.instance.gameData.Woker += 1;
        }

        if (i == 9) //딸기
        {
            removeCard(9);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 6);
            DataController.instance.gameData.addCardCount(6);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 6);
            DataController.instance.gameData.addCardCount(6);

            yield return new WaitForSeconds(2.0f);

            CreateCard(0, 6);
            DataController.instance.gameData.addCardCount(6);
        }


        if (i < 10) { DataController.instance.gameData.Woker += 1; }
        else { DataController.instance.gameData.Woker -= 2; }

        StopCoroutine(delay(0));
        yield return null;
    }

    void CreateCard(int i, int j)
    {
        if (i == 0)
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);

            DataController.instance.gameData.Card.Add(Instantiate(BasicCardSet[j], new Vector3(randPosX, randPosY, 0), Quaternion.identity));
        }
        if (i == 1)
        {
            float randPosX = Random.Range(-5f, 5f);
            float randPosY = Random.Range(-4f, 2f);
            DataController.instance.gameData.Card.Add(Instantiate(IntermediatCardSet[j], new Vector3(randPosX, randPosY, 0), Quaternion.identity));
        }
    }

    void removeCard(int i)
    {
        switch (i)
        {
            case 0: //통나무 삭제
                foreach (GameObject wood in DataController.instance.gameData.Card)
                {
                    if (wood.name == "Wood(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(wood);
                        Destroy(wood);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 1://돌삭제
                foreach (GameObject stone in DataController.instance.gameData.Card)
                {
                    if (stone.name == "Stone(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(stone);
                        Destroy(stone);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 2://나무 삭제
                foreach (GameObject tree in DataController.instance.gameData.Card)
                {
                    if (tree.name == "Tree(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(tree);
                        Destroy(tree);
                        DataController.instance.gameData.stdCardCount(3);
                        break;
                    }
                };
                break;
            case 3://암석삭제
                foreach (GameObject rock in DataController.instance.gameData.Card)
                {
                    if (rock.name == "Rock(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(rock);
                        Destroy(rock);
                        DataController.instance.gameData.stdCardCount(3);
                        break;
                    }
                }
                break;
            case 4://바나나나무
                foreach (GameObject bananatree in DataController.instance.gameData.Card)
                {
                    if (bananatree.name == "BananaTree(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(bananatree);
                        Destroy(bananatree);
                        DataController.instance.gameData.stdCardCount(4);
                        break;
                    }
                }
                break;
            case 5: //바나나
                foreach (GameObject banana in DataController.instance.gameData.Card)
                {
                    if (banana.name == "Banana(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(banana);
                        Destroy(banana);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 6: //딸기
                foreach (GameObject berry in DataController.instance.gameData.Card)
                {
                    if (berry.name == "StrawBerry(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(berry);
                        Destroy(berry);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 7://딸기나무
                foreach (GameObject berry in DataController.instance.gameData.Card)
                {
                    if (berry.name == "StrawBerryTree(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(berry);
                        Destroy(berry);
                        DataController.instance.gameData.stdCardCount(7);
                        break;
                    }
                }
                break;
            case 8://철광석
                foreach (GameObject iron in DataController.instance.gameData.Card)
                {
                    if (iron.name == "Iron(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(iron);
                        Destroy(iron);
                        DataController.instance.gameData.stdCardCount(8);
                        break;
                    }
                };
                break;
            case 9://금광석
                foreach (GameObject gold in DataController.instance.gameData.Card)
                {
                    if (gold.name == "Gold(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(gold);
                        Destroy(gold);
                        DataController.instance.gameData.stdCardCount(9);
                        break;
                    }
                }
                break;
            case 10://나뭇가지
                foreach (GameObject branch in DataController.instance.gameData.Card)
                {
                    if (branch.name == "Branch(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(branch);
                        Destroy(branch);
                        DataController.instance.gameData.stdCardCount(10);
                        break;
                    }
                }
                break;
            case 11: //은괴
                foreach (GameObject iron in DataController.instance.gameData.Card)
                {
                    if (iron.name == "IronIngot(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(iron);
                        Destroy(iron);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 12: //금괴
                foreach (GameObject gold in DataController.instance.gameData.Card)
                {
                    if (gold.name == "GoldIngot(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(gold);
                        Destroy(gold);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 13: //벽돌
                foreach (GameObject brick in DataController.instance.gameData.Card)
                {
                    if (brick.name == "Brick(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(brick);
                        Destroy(brick);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 14: //판자
                foreach (GameObject panel in DataController.instance.gameData.Card)
                {
                    if (panel.name == "Panel(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(panel);
                        Destroy(panel);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 15: //집
                foreach (GameObject house in DataController.instance.gameData.Card)
                {
                    if (house.name == "House(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(house);
                        Destroy(house);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 16: //용광로
                foreach (GameObject forge in DataController.instance.gameData.Card)
                {
                    if (forge.name == "Forge(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(forge);
                        Destroy(forge);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 17: //제재소
                foreach (GameObject timber in DataController.instance.gameData.Card)
                {
                    if (timber.name == "Timber(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(timber);
                        Destroy(timber);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 18: //벽돌공장
                foreach (GameObject mine in DataController.instance.gameData.Card)
                {
                    if (mine.name == "Mine(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(mine);
                        Destroy(mine);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 19: //주방
                foreach (GameObject kitchen in DataController.instance.gameData.Card)
                {
                    if (kitchen.name == "Kitchen(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(kitchen);
                        Destroy(kitchen);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
            case 20://주민
                foreach (GameObject player in DataController.instance.gameData.Card)
                {
                    if (player.name == "Player(Clone)")
                    {
                        DataController.instance.gameData.Card.Remove(player);
                        Destroy(player);
                        DataController.instance.gameData.stdCardCount(1);
                        break;
                    }
                }
                break;
        }

    }
}
