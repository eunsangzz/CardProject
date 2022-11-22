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

    bool HouseCraft = false;
    bool ForgeCraft = false;
    bool TimberCraft = false;
    bool MineCraft = false;
    int CraftNum = 0;

    void Update()
    {
        if(CraftUI.activeSelf == false)
        {
            CraftNum = 0;
        }
        if(HouseCraft == true && CraftNum < 2)
        {
            CraftDelete(3);
            CraftDelete(4);
            if(CraftNum == 1)
            {
                HouseCraft = false;
            }
            CraftNum += 1;
        }
        if(ForgeCraft == true && CraftNum < 2)
        {
            CraftDelete(2);
            if(CraftNum == 1)
            {
                ForgeCraft = false;
            }
            if(CraftNum == 0)
            {
                CraftDelete(1);
            }
            CraftNum += 1;
        }
        if(TimberCraft == true && CraftNum < 2)
        {
            CraftDelete(1);
            if(CraftNum == 1)
            {
                CraftDelete(1);
                TimberCraft = false;
            }
            if(CraftNum == 0)
            {
                CraftDelete(2);
            }
            CraftNum += 1;
        }
        if(MineCraft == true && CraftNum < 2)
        {
            CraftDelete(2);
            if(CraftNum == 1)
            {
                CraftDelete(2);
                MineCraft = false;
            }
            if(CraftNum == 0)
            {
                CraftDelete(1);
            }
            CraftNum += 1;
        }
    }

    public void CardCraft()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        if(clickObject.name == "HouseCraft")
        {
            if (DataController.instance.gameData.WoodCard >= 2 && 
                DataController.instance.gameData.StoneCard >= 1)
            {
                float randPosX = Random.Range(-5, 5);
                float randPosY = Random.Range(-4, 4);
                GameObject _Card = Instantiate(CraftCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                HouseCraft = true;

                DataController.instance.gameData.WoodCard -= 2;
                DataController.instance.gameData.StoneCard -= 1;
                DataController.instance.gameData.HouseCard += 1;
                DataController.instance.gameData.CardLimit += 3;

                CraftUI.SetActive(false);
                HouseCraftUI.SetActive(false);
            }
            else ErrorUi.SetActive(true);
        }
        if(clickObject.name == "ForgeCraft")
        {
            if (DataController.instance.gameData.WoodCard >= 1 && 
                DataController.instance.gameData.StoneCard >= 2)
            {
                float randPosX = Random.Range(-5, 5);
                float randPosY = Random.Range(-4, 4);
                GameObject _Card = Instantiate(CraftCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                ForgeCraft = true;

                DataController.instance.gameData.WoodCard -= 1;
                DataController.instance.gameData.StoneCard -= 2;
                DataController.instance.gameData.ForgeCard += 1;

                CraftUI.SetActive(false);
                ForgeCraftUi.SetActive(false);
            }
            else ErrorUi.SetActive(true);
        }
        if(clickObject.name == "TimberCraft")
        {
            if (DataController.instance.gameData.WoodCard >= 3 && 
                DataController.instance.gameData.StoneCard >= 1)
            {
                float randPosX = Random.Range(-5, 5);
                float randPosY = Random.Range(-4, 4);
                GameObject _Card = Instantiate(CraftCardSet[2], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                TimberCraft = true;

                DataController.instance.gameData.WoodCard -= 3;
                DataController.instance.gameData.StoneCard -= 1;
                DataController.instance.gameData.TimberCard += 1;

                CraftUI.SetActive(false);
                TimberCraftUi.SetActive(false);
            }
            else ErrorUi.SetActive(true);
        }
        if(clickObject.name == "MineCraft")
        {
            if (DataController.instance.gameData.WoodCard >= 1 && 
                DataController.instance.gameData.StoneCard >= 3)
            {
                float randPosX = Random.Range(-5, 5);
                float randPosY = Random.Range(-4, 4);
                GameObject _Card = Instantiate(CraftCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                MineCraft = true;

                DataController.instance.gameData.WoodCard -= 1;
                DataController.instance.gameData.StoneCard -= 3;
                DataController.instance.gameData.MineCard += 1;

                CraftUI.SetActive(false);
                MineCraftUi.SetActive(false);
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
        }
        if(clickObject.name == "Forge")
        {
            CraftList.SetActive(false);
            ForgeCraftUi.SetActive(true);
        }
        if(clickObject.name == "Timber")
        {
            CraftList.SetActive(false);
            TimberCraftUi.SetActive(true);
        }
        if(clickObject.name == "Mine")
        {
            CraftList.SetActive(false);
            MineCraftUi.SetActive(true);
        }
    }

    public void ErrorClose()
    {
        ErrorUi.SetActive(false);
    }

    void CraftDelete(int CardNum)
    {
        switch(CardNum)
        {
            case 1:
                GameObject _delWoodCard1 = GameObject.Find("Wood(Clone)");
                Debug.Log("1");
                Destroy(_delWoodCard1);
                break;
            case 2:
                GameObject _delStoneCard = GameObject.Find("Stone(Clone)");
                Destroy(_delStoneCard);
                break;
            case 3:
                GameObject _delPanelCard = GameObject.Find("Panel(Clone)");
                Destroy(_delPanelCard);
                break;
            case 4:
                GameObject _delBrickCard = GameObject.Find("Brick(Clone)");
                Destroy(_delBrickCard);
                break;
        }
    }

}
