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

    public GameObject workerError;

    private bool tutobuy;
    private bool tutosell;

    public GameObject wokerError;
    //카드 구매버튼 눌렀을때 저장해둔 프리팹중에 랜덤으로 하나 생성 
    //구매 버튼 업그레이드 적용해서 1단계 나무 돌 2단계 철 금 등등 으로 세팅

    private Dictionary<string, (int removeIndex, int gold)> _sellMap;


    //프리팹 basename 매핑
    private static readonly string[] _indexToName =
    {
        "Wood","Stone","Tree","Rock","BananaTree","Banana","StrawBerry","StrawBerryTree",
        "Iron","Gold","Branch","IronIngot","GoldIngot","Brick","Panel","House",
        "Forge","Timber","Mine","Kitchen","Player"
    };

    private void Awake()
    {
        _sellMap = new Dictionary<string, (int, int)>
        {
            {"Wood", (0,2) },
            { "Stone", (1, 2) },
            { "Tree", (2, 2) },
            { "Rock", (3, 2) },
            { "BananaTree", (4, 2) },
            { "Banana", (5, 1) },
            { "StrawBerry", (6, 1) },
            { "StrawBerryTree", (7, 2) },
            { "Iron", (8, 8) },
            { "Gold", (9, 6) },
            { "Branch", (10, 3) },
            { "IronIngot", (11, 6) },
            { "GoldIngot", (12, 20) },
            { "Brick", (13, 5) },
            { "Panel", (14, 6) },
            { "House", (15, 15) },
            { "Forge", (16, 6) },
            { "Timber", (17, 8) },
            { "Mine", (18, 8) },
            { "Kitchen", (19, 5) },
            { "Player", (20, 5) },
        };
    }

    private void Start()
    {
        tutobuy = false;
        tutosell = false;

        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults(); //card null방지

        var go = Instantiate(PlayerCard, Vector3.zero, Quaternion.identity);
        go.name = PlayerCard.name;
        gd.Card.Add(go);

    }

    private void Update()
    {
        HandleSellClick();
    }


    public void CardBuy()//카드 살때
    {
        var gd = DataController.instance.gameData;

        if (gd.tuto && !tutobuy) //튜토리얼
        {
            Time.timeScale = 0;
            tutoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoBuy.SetActive(true);
            tutobuy = true;
        }

        if (gd.storeUpgrade == 0 && gd.gold >= 3 && Time.timeScale != 0)//업그레이드 없음
        {
            if (gd.PlayerCount == 1 && gd.QusetNum > 2) //필드 내 플레이어가 1명일때
            {
                int rand1 = Random.Range(0, 10);
                if (rand1 > 7) // 랜덤값이 7초과면 플레이어 카드 생성
                {
                    SpawnPlayer();
                    gd.AddPlayer(1);
                    gd.AddWorker(1);
                }
                else // 랜덤값이 7이하일때 
                {
                    int rand = Random.Range(0, 5);
                    CreateCard(isBasic: true, index: rand);
                    gd.addCardCount(rand);
                    gd.AddGold(-3);
                }
            }

            else if (gd.QusetNum != 1 && gd.QusetNum > 2) //필드내 플레이어가 2명이상 일때
            {
                int rand = Random.Range(0, 8); //랜덤값으로 카드 생성
                if (rand > 6) //음식 생성
                {
                    int rand2 = 4;
                    CreateCard(isBasic: true, index: rand2);
                    gd.addCardCount(rand2);
                    gd.AddGold(-3);
                }
                else
                {
                    int rand3 = Random.Range(0, 4);
                    CreateCard(isBasic: true, index: rand3);
                    gd.addCardCount(rand3);
                    gd.AddGold(-3);
                };
            }

            if (gd.QusetNum == 2)
            {
                gd.Add(GameData.CardType.BananaTree, 1);
                gd.AddGold(-3);
                CreateCard(isBasic: true, index: 4);
            }

            if (gd.QusetNum == 0)
            {
                gd.AddQuest(1);
                gd.AddGold(-3);
                gd.Add(GameData.CardType.Wood, 1);
                CreateCard(isBasic: true, index: 0);
            }
        }
        if (gd.storeUpgrade == 1 && gd.gold >= 3 && gd.QusetNum != 0)//업그레이드 없음
        {
            int rand = Random.Range(2, 10);
            if (rand == 6) rand = 4;
            if (rand == 7) rand = 5;

            CreateCard(isBasic: true, index: rand);
            gd.addCardCount(rand);
            gd.AddGold(-3);
        }

    }

    public void Cheat()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        Vector3 pos = GetRandomSpawnPos();
        var go = Instantiate(IntermediatCardSet[3], pos, Quaternion.identity);
        go.name = IntermediatCardSet[3].name;
        gd.Card.Add(go);
        gd.Add(GameData.CardType.GoldIngot, 1);
    }

    public void SellActive()
    {
        var gd = DataController.instance.gameData;

        if (gd.tuto && !tutosell)
        {
            Time.timeScale = 0;
            tutoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoSell.SetActive(true);
            tutosell = true;
        }

        if (Time.timeScale == 0) return;

        gd.Sell = !gd.Sell;
    }

    public void endDaySellCard()
    {
        var gd = DataController.instance.gameData;
        if (gd.endDay == true) gd.Sell = true;
    }

    private void HandleSellClick()
    {
        var gd = DataController.instance.gameData;
        if (!gd.Sell) return;

        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        GameObject touch = hit.transform.gameObject;

        string key = NormalizeName(touch.name);

        if (_sellMap.TryGetValue(key, out var sellInfo))
        {
            removeCard(sellInfo.removeIndex);
            gd.AddGold(sellInfo.gold);

            RecalcSafe();
        }
    }

    public void CardSkill()
    {
        var gd = DataController.instance.gameData;

        string Btn = EventSystem.current.currentSelectedGameObject.name;

        if (gd.Woker == 0)
        {
            wokerError.SetActive(true);
            return;
        }
        else
        {
            if (Btn == "Tree") StartCoroutine(delay(1));

            if (Btn == "Wood") StartCoroutine(delay(8));

            if (Btn == "Rock") StartCoroutine(delay(2));

            if (Btn == "BananaTree" && gd.Woker != 0)
            {
                StartCoroutine(delay(3));
                if (gd.QusetNum == 2) gd.AddQuest(1);
            }

            if (Btn == "StrawBerryTree") StartCoroutine(delay(9));

            if (Btn == "Timber" && gd.WoodCard >= 2 && gd.BranchCard >= 1
                && gd.Woker != 0) //판자
            {
                if (gd.gold >= 1)
                {
                    if (gd.QusetNum == 4) gd.AddQuest(1);
                    StartCoroutine(delay(6));
                }
            }

            if (Btn == "Mine" && gd.StoneCard >= 2 && gd.Woker != 0)
            {
                if (gd.gold >= 1)
                {
                    if (gd.QusetNum == 4) gd.AddQuest(1);
                    StartCoroutine(delay(7));
                }
            }

            if (Btn == "ForgeIron" && gd.WoodCard >= 2 && gd.IronCard >= 1 && gd.BranchCard >= 2)
                StartCoroutine(delay(4));


            if (Btn == "ForgeGold" && gd.WoodCard >= 2 && gd.GoldCard >= 1 && gd.BranchCard >= 1)
                StartCoroutine(delay(5));


            if (Btn == "House" && gd.PlayerCount >= 2 && gd.gold > 15 && gd.Woker >= 2)
            { 
                StartCoroutine(delay(10));
                gd.AddGold(-15);
            }
        }
    }

    public void WokerErrorClose()
    {
        wokerError.SetActive(false);
    }

    public void StoreUpgrade()
    {
        var gd = DataController.instance.gameData;

        if (gd.storeUpgrade == 0 && gd.gold >= 30) gd.storeUpgrade += 1;
        if (gd.storeUpgrade == 1 && gd.gold >= 60) gd.storeUpgrade += 1;
    }

    IEnumerator delay(int i)
    {
        var gd = DataController.instance.gameData;

        gd.Skill = false;

        int workerCost = (i < 10) ? 1 : 2;

        gd.AddWorker(-workerCost);

        if (i == 1) // 나무 -> 목재 2개
        {
            removeCard(2);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 0); gd.addCardCount(0);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 0); gd.addCardCount(0);
        }
        else if (i == 2) // 암석 -> 석재 2개
        {
            removeCard(3);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 1); gd.addCardCount(1);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 1); gd.addCardCount(1);
        }
        else if (i == 3) // 바나나나무 -> 바나나 3개
        {
            removeCard(4);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 5); gd.addCardCount(5);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 5); gd.addCardCount(5);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 5); gd.addCardCount(5);
        }
        else if (i == 4) // 철괴(은이라고 주석인데 실제론 IronIngot)
        {
            removeCard(0); removeCard(0); removeCard(10); removeCard(8);
            yield return new WaitForSeconds(5f);

            CreateCard(false, 2);
            gd.addCardCount(11);
        }
        else if (i == 5) // 금괴
        {
            removeCard(0); removeCard(0); removeCard(10); removeCard(9);
            yield return new WaitForSeconds(5f);

            CreateCard(false, 3);
            gd.addCardCount(12);
        }
        else if (i == 6) // 판자
        {
            removeCard(0); removeCard(0);
            removeCard(10);

            if (gd.QusetNum == 6) gd.AddQuest(1);

            yield return new WaitForSeconds(5f);

            CreateCard(false, 0);
            gd.addCardCount(14);
        }
        else if (i == 7) // 벽돌
        {
            removeCard(1); removeCard(1);
            yield return new WaitForSeconds(5f);

            CreateCard(false, 1);
            gd.addCardCount(13);
        }
        else if (i == 8) // 나뭇가지 3개
        {
            removeCard(0);
            yield return new WaitForSeconds(2f);

            CreateCard(false, 4); gd.addCardCount(10);
            yield return new WaitForSeconds(2f);

            CreateCard(false, 4); gd.addCardCount(10);
            yield return new WaitForSeconds(2f);

            CreateCard(false, 4); gd.addCardCount(10);

            if (gd.QusetNum == 1) gd.AddQuest(1);
        }
        else if (i == 9) // 딸기 3개
        {
            removeCard(7);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 6); gd.addCardCount(6);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 6); gd.addCardCount(6);
            yield return new WaitForSeconds(2f);

            CreateCard(true, 6); gd.addCardCount(6);
        }
        else if (i == 10) // 주민(60초 후 생성)
        {
            yield return new WaitForSeconds(60f);

            SpawnPlayer();
            gd.AddPlayer(1);
            gd.AddWorker(1);
        }
        gd.AddWorker(workerCost);


        RecalcSafe();
        yield break;
    }

    private void CreateCard(bool isBasic, int index)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        Vector3 pos = GetRandomSpawnPos();

        if (isBasic)
        {
            var go = Instantiate(BasicCardSet[index], pos, Quaternion.identity);
            go.name = BasicCardSet[index].name;
            gd.Card.Add(go);
        }
        else
        {
            var go = Instantiate(IntermediatCardSet[index], pos, Quaternion.identity);
            go.name = IntermediatCardSet[index].name;
            gd.Card.Add(go);
        }

        RecalcSafe();
    }

    private void SpawnPlayer()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        Vector3 pos = GetRandomSpawnPos();
        var go = Instantiate(playerPre, pos, Quaternion.identity);
        go.name = playerPre.name;
        gd.Card.Add(go);
    }

    private Vector3 GetRandomSpawnPos()
    {
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-4f, 2f);
        return new Vector3(x, y, 0f);
    }

    private static string NormalizeName(string objectName)
    {
        const string clone = "(Clone)";
        return objectName.EndsWith(clone) ? objectName.Replace(clone, "") : objectName;
    }

    public void removeCard(int i)
    {
        if (i < 0 || i >= _indexToName.Length) return;
        RemoveFirstCardByPrefabName(_indexToName[i], i);

        RecalcSafe();
    }

    private void RemoveFirstCardByPrefabName(string prefabName, int stdCardCountIndex)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        for (int idx = 0; idx < gd.Card.Count; idx++)
        {
            GameObject card = gd.Card[idx];
            if (card == null) continue;

            if (NormalizeName(card.name) != prefabName) continue;

            gd.Card.RemoveAt(idx);
            Destroy(card);
            gd.stdCardCount(stdCardCountIndex);
            return;
        }

    }

    private void RecalcSafe()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();
    }
}
