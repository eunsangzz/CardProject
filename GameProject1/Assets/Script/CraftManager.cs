using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftManager : MonoBehaviour
{
    public GameObject[] CraftCardSet = new GameObject[4];
    public GameObject ErrorUi;
    public GameObject CraftUI;

    public GameObject CraftList;
    public GameObject HouseCraftUI;
    public GameObject ForgeCraftUi;
    public GameObject TimberCraftUi;
    public GameObject MineCraftUi;

    void Update()
    {
       
    }

    public void CardCraft()
    {
        string clickObject = EventSystem.current.currentSelectedGameObject.name;

        if (clickObject == "HouseCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.PanelCard >= 3 && 
                DataController.instance.gameData.BrickCard >= 3)
            {
                StartCoroutine(delay(1));
                DataController.instance.gameData.HouseCard += 1;
            }
            else ErrorUi.SetActive(true);
        }

        if(clickObject == "ForgeCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.BranchCard >= 1 && 
                DataController.instance.gameData.BrickCard >= 2)
            {
                if(DataController.instance.gameData.QusetNum == 3)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                StartCoroutine(delay(3));
                DataController.instance.gameData.ForgeCard += 1;
            }
            else ErrorUi.SetActive(true);
        }

        if(clickObject == "TimberCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.WoodCard >= 3 && 
                DataController.instance.gameData.StoneCard >= 1)
            {
                if (DataController.instance.gameData.QusetNum == 3)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                StartCoroutine(delay(4));
                DataController.instance.gameData.TimberCard += 1;
            }
            else ErrorUi.SetActive(true);
        }

        if(clickObject == "MineCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.WoodCard >= 1 && 
                DataController.instance.gameData.StoneCard >= 3)
            {
                if (DataController.instance.gameData.QusetNum == 3)
                {
                    DataController.instance.gameData.QusetNum += 1;
                }
                StartCoroutine(delay(2));
                DataController.instance.gameData.MineCard += 1;
            }
            else ErrorUi.SetActive(true);
        }

        if(clickObject == "Kitchen")
        {
            if(DataController.instance.gameData.WoodCard >= 2 &&
                DataController.instance.gameData.StoneCard >= 2&&
                DataController.instance.gameData.IronIngotCard >= 2)
            {
                createCard(5);
                DataController.instance.gameData.KitchenCard += 1;
            }
            else ErrorUi.SetActive(true);
        }
    }

    public void CraftUi()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        if(clickObject.name == "House")
        {
            CraftList.SetActive(false);
            HouseCraftUI.SetActive(true);
            ForgeCraftUi.SetActive(false);
            TimberCraftUi.SetActive(false);
            MineCraftUi.SetActive(false);
        }
        if(clickObject.name == "Forge")
        {
            CraftList.SetActive(false);
            ForgeCraftUi.SetActive(true);
            HouseCraftUI.SetActive(false);
            TimberCraftUi.SetActive(false);
            MineCraftUi.SetActive(false);
        }
        if(clickObject.name == "Timber")
        {
            CraftList.SetActive(false);
            TimberCraftUi.SetActive(true);
            HouseCraftUI.SetActive(false);
            ForgeCraftUi.SetActive(false);
            MineCraftUi.SetActive(false);
        }
        if(clickObject.name == "Mine")
        {
            CraftList.SetActive(false);
            MineCraftUi.SetActive(true);
            HouseCraftUI.SetActive(false);
            ForgeCraftUi.SetActive(false);
            TimberCraftUi.SetActive(false);
            
        }
    }

    public void ErrorClose()
    {
        ErrorUi.SetActive(false);
    }


    void createCard(int i)
    {
        float randPosX = Random.Range(-5f, 5f);
        float randPosY = Random.Range(-4f, 2f);
        DataController.instance.gameData.Card.Add(Instantiate(CraftCardSet[i], new Vector3(randPosX, randPosY, 0), Quaternion.identity));
    }

    IEnumerator delay(int i)
    {
        Debug.Log("startCraft");
        if (i < 10) { DataController.instance.gameData.Woker -= 1; }
        CraftUI.SetActive(false);
        MineCraftUi.SetActive(false);

        if (i == 1)//집
        {
            for (int u = 0; u < 3; u++)
            {
                removeCard(13);
                removeCard(14);
            }
            yield return new WaitForSeconds(60.0f);

            createCard(0);
        }

        if (i == 2)//채석장
        {
            removeCard(0);
            for (int u = 0; u < 3; u++)
            {
                removeCard(1);
            }
            yield return new WaitForSeconds(30.0f);

            createCard(3);

        }

        if (i == 3)//용광로
        {
            removeCard(10);
            for (int u = 0; u < 2; u++)
            {
                removeCard(13);
            }
            yield return new WaitForSeconds(30.0f);

            createCard(1);

        }

        if (i == 4)//제재소
        {
            removeCard(1);
            for (int u = 0; u < 3; u++)
            {
                removeCard(0);
            }
            yield return new WaitForSeconds(5.0f);

            createCard(2);
        }

        if (i == 4)//주방
        {
            for (int u = 0; u < 2; u++)
            {
                removeCard(0);
                removeCard(1);
                removeCard(11);
            }
            yield return new WaitForSeconds(30.0f);

            createCard(4);

        }

        if (i < 10) { DataController.instance.gameData.Woker += 1; }

        StopCoroutine(delay(0));
        yield return null;
    }

    public void backspaceBtn()
    {
        CraftList.SetActive(true);
        MineCraftUi.SetActive(false);
        HouseCraftUI.SetActive(false);
        ForgeCraftUi.SetActive(false);
        TimberCraftUi.SetActive(false);
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
                        Debug.Log("wooddel");
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
