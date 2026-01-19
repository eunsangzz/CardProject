using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private CardSpawner spawner;

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
    private Dictionary<string, ICardCommand> _commands;


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

        if (spawner == null) spawner = FindObjectOfType<CardSpawner>();
    }

    private void Start()
    {
        tutobuy = false;
        tutosell = false;

        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults(); //card null방지

  
        if (spawner != null)
        {
            var go = spawner.Spawn((int)CardId.Player);
            if (go != null) 
            {
                go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                go.name = CardId.Player.ToString();
                gd.Card.Add(go);
            }
        }

        _commands = CardCommandRegistry.Bulid();
    }

    private void Update()
    {
        HandleSellClick();
    }

    private static bool TryNameToCardId(string prefabName, out CardId id)
    {
        switch (prefabName)
        {
            case "Wood": id = CardId.Wood; return true;
            case "Stone": id = CardId.Stone; return true;
            case "Tree": id = CardId.Tree; return true;
            case "Rock": id = CardId.Rock; return true;

            case "BananaTree": id = CardId.BananaTree; return true;
            case "Banana": id = CardId.Banana; return true;

            case "StrawBerryTree": id = CardId.StrawBerryTree; return true;
            case "StrawBerry": id = CardId.StrawBerry; return true;

            case "Iron": id = CardId.Iron; return true;
            case "Gold": id = CardId.Gold; return true;

            case "Branch": id = CardId.Branch; return true;
            case "IronIngot": id = CardId.IronIngot; return true;
            case "GoldIngot": id = CardId.GoldIngot; return true;

            case "Brick": id = CardId.Brick; return true;
            case "Panel": id = CardId.Panel; return true;

            case "House": id = CardId.House; return true;
            case "Forge": id = CardId.Forge; return true;
            case "Timber": id = CardId.Timber; return true;
            case "Mine": id = CardId.Mine; return true;
            case "Kitchen": id = CardId.Kitchen; return true;

            case "Player": id = CardId.Player; return true;
            default:
                id = CardId.Wood;
                return false;
        }
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

        var go = spawner.Spawn((int)CardId.GoldIngot);
        if (go != null)
        {
            go.transform.SetPositionAndRotation(pos, Quaternion.identity);
            go.name = CardId.GoldIngot.ToString();
            gd.Card.Add(go);
        }

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

        if (_commands == null || !_commands.TryGetValue(Btn, out var cmd)) return;

        StartCoroutine(RunCommand(cmd));
    }

    private IEnumerator RunCommand(ICardCommand cmd)
    {
        var gd = DataController.instance.gameData;

        if(gd.Woker < cmd.WorkerCost)
        {
            workerError.SetActive(true);
            yield break;
        }

        if(!cmd.CanExecute(gd))
        {
            wokerError?.SetActive(true);
            yield break;
        }

        gd.Skill = false;

        gd.AddWorker(-cmd.WorkerCost);
        yield return StartCoroutine(cmd.Execute(this, gd));
        gd.AddWorker(cmd.WorkerCost);

        RecalcSafe();
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


    private void CreateCard(bool isBasic, int index)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (spawner == null) return;

        Vector3 pos = GetRandomSpawnPos();

        GameObject prefab = isBasic ? BasicCardSet[index] : IntermediatCardSet[index];
        if (prefab == null) return;

        string prefabName = prefab.name;

        if (!TryNameToCardId(prefabName, out CardId id))
            return;
        
  
        var go = spawner.Spawn((int)id);
        if (go == null) return;

        go.transform.SetPositionAndRotation(pos, Quaternion.identity);
        go.name = prefabName;
        gd.Card.Add(go);

        RecalcSafe();
    }

    private void SpawnPlayer()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (spawner == null) return;

        Vector3 pos = GetRandomSpawnPos();

        var go = spawner.Spawn((int)CardId.Player);
        if (go == null) return;

        go.transform.SetPositionAndRotation(pos, Quaternion.identity);
        go.name = "Player";
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

            if (pool != null) pool.Release(card);
            else card.SetActive(false);

            gd.stdCardCount(stdCardCountIndex);
            return;
        }

    }

    private void RecalcSafe()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();
    }

    public void CreateBasicCard(int index) => CreateCard(true, index);
    public void CreateIntermediateCard(int index) => CreateCard(false, index);

    public void RemoveCardByIndex(int i)
    {
        if (i < 0 || i >= _indexToName.Length) return;
        RemoveFirstCardByPrefabName(_indexToName[i], i);
    }

    public void SpawnPlayerCard() => SpawnPlayer();
}
