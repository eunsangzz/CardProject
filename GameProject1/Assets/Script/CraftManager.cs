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
        if(HouseCraft == true && CraftNum < 3)
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
            CraftDelete(4);//벽돌제거
            if(CraftNum == 1)
            {
                ForgeCraft = false;
            }
            if(CraftNum == 0)
            {
                CraftDelete(5);//나뭇가지제거
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

        if(clickObject.name == "HouseCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.PanelCard >= 3 && 
                DataController.instance.gameData.BranchCard >= 2)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 2f);
                GameObject _Card = Instantiate(CraftCardSet[0], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                HouseCraft = true;
                CraftNum = 0;

                DataController.instance.gameData.PanelCard -= 3;
                DataController.instance.gameData.BranchCard -= 2;
                DataController.instance.gameData.HouseCard += 1;
                DataController.instance.gameData.CardLimit += 3;

                CraftUI.SetActive(false);
                HouseCraftUI.SetActive(false);
            }
            else ErrorUi.SetActive(true);
        }
        if(clickObject.name == "ForgeCraft" && DataController.instance.gameData.Woker != 0)
        {
            if (DataController.instance.gameData.WoodCard >= 1 && 
                DataController.instance.gameData.StoneCard >= 2)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 2f);
                GameObject _Card = Instantiate(CraftCardSet[1], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                ForgeCraft = true;
                CraftNum = 0;

                DataController.instance.gameData.BranchCard -= 1;
                DataController.instance.gameData.BrickCard -= 2;
                DataController.instance.gameData.ForgeCard += 1;

                CraftUI.SetActive(false);
                ForgeCraftUi.SetActive(false);
            }
            else ErrorUi.SetActive(true);
        }
        if(clickObject.name == "TimberCraft")
        {
            if (DataController.instance.gameData.WoodCard >= 3 && 
                DataController.instance.gameData.StoneCard >= 1 && DataController.instance.gameData.Woker != 0)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 2f);
                GameObject _Card = Instantiate(CraftCardSet[2], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                TimberCraft = true;
                CraftNum = 0;

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
                DataController.instance.gameData.StoneCard >= 3 && DataController.instance.gameData.Woker != 0)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 2f);
                GameObject _Card = Instantiate(CraftCardSet[3], new Vector3(randPosX, randPosY, 0), Quaternion.identity);
                DataController.instance.gameData.CraftCardList.Add(_Card);

                MineCraft = true;
                CraftNum = 0;

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

    void CraftDelete(int CardNum)
    {
        switch(CardNum)
        {
            case 1:
                GameObject _delWoodCard1 = GameObject.Find("Wood(Clone)");
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
            case 5:
                GameObject _delBranchCard = GameObject.Find("Branch(Clone)");
                Destroy(_delBranchCard);
                break;
        }
    }

    public void backspaceBtn()
    {
        CraftList.SetActive(true);
        MineCraftUi.SetActive(false);
        HouseCraftUI.SetActive(false);
        ForgeCraftUi.SetActive(false);
        TimberCraftUi.SetActive(false);
    }

}
