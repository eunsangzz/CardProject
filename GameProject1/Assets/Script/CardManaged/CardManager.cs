using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private CardSpawner spawner;

    private static readonly CardId[] _stdIndexToCardId =
    {
        CardId.Wood, CardId.Stone, CardId.Tree, CardId.Rock,
    CardId.BananaTree, CardId.Banana, CardId.StrawBerry, CardId.StrawBerryTree,
    CardId.Iron, CardId.Gold, CardId.Branch, CardId.IronIngot, CardId.GoldIngot,
    CardId.Brick, CardId.Panel, CardId.House, CardId.Forge, CardId.Timber,
    CardId.Mine, CardId.Kitchen, CardId.Player, CardId.Armory
    };

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

    private Dictionary<CardId, (int removeIndex, int gold)> _sellMapById;
    private Dictionary<string, ICardCommand> _commands;
    private GameObject _selectedCard;


    //프리팹 basename 매핑
    private static readonly string[] _indexToName =
    {
        "Wood","Stone","Tree","Rock","BananaTree","Banana","StrawBerry","StrawBerryTree",
        "Iron","Gold","Branch","IronIngot","GoldIngot","Brick","Panel","House",
        "Forge","Timber","Mine","Kitchen","Player","Armory"
    };

    private void Awake()
    {
        _sellMapById = new Dictionary<CardId, (int, int)>
        {
            { CardId.Wood, (0,2) },
            { CardId.Stone, (1,2) },
            { CardId.Tree, (2,2) },
            { CardId.Rock, (3,2) },
            { CardId.BananaTree, (4,2) },
            { CardId.Banana, (5,1) },
            { CardId.StrawBerry, (6,1) },
            { CardId.StrawBerryTree, (7,2) },
            { CardId.Iron, (8,8) },
            { CardId.Gold, (9,6) },
            { CardId.Branch, (10,3) },
            { CardId.IronIngot, (11,6) },
            { CardId.GoldIngot, (12,20) },
            { CardId.Brick, (13,5) },
            { CardId.Panel, (14,6) },
            { CardId.House, (15,15) },
            { CardId.Forge, (16,6) },
            { CardId.Timber, (17,8) },
            { CardId.Mine, (18,8) },
            { CardId.Kitchen, (19,5) },
            { CardId.Player, (20,5) },
            { CardId.Armory, (21,20) },
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
            case "Armory": id = CardId.Armory; return true;

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

        var cardObj = hit.collider.gameObject;
        if (CardWorkService.IsLocked(cardObj)) return;

        if (!cardObj.TryGetComponent<CardIdentity>(out var ident)) return;

        if (!_sellMapById.TryGetValue(ident.cardId, out var sellInfo)) return;

        gd.Card.Remove(cardObj);
        spawner.Despawn(cardObj);

        gd.stdCardCount(sellInfo.removeIndex);
        gd.AddGold(sellInfo.gold);

        RecalcSafe();
        
    }

    public void CardSkill()
    {
        var gd = DataController.instance.gameData;

        string Btn = EventSystem.current.currentSelectedGameObject.name;

        if (_commands == null || !_commands.TryGetValue(Btn, out var cmd)) return;

        StartCoroutine(RunCommand(cmd));
    }

    public bool TryStartCommandFromStack(IList<GameObject> stackCards)
    {
        if (stackCards == null || stackCards.Count == 0)
            return false;

        if (_commands == null)
            _commands = CardCommandRegistry.Bulid();

        var gd = DataController.instance.gameData;
        Dictionary<CardId, int> counts = CountCardsById(stackCards);
        List<GameObject> residents = FindCardsById(stackCards, CardId.Player);

        for (int i = 0; i < stackCards.Count; i++)
        {
            GameObject target = stackCards[i];
            if (target == null ||
                !target.activeInHierarchy ||
                CardWorkService.IsLocked(target) ||
                !target.TryGetComponent(out CardIdentity targetIdentity) ||
                targetIdentity.cardId == CardId.Player)
            {
                continue;
            }

            string[] commandKeys = GetCommandKeysForTarget(targetIdentity.cardId);
            for (int k = 0; k < commandKeys.Length; k++)
            {
                if (!_commands.TryGetValue(commandKeys[k], out ICardCommand command))
                    continue;

                if (residents.Count != command.WorkerCost ||
                    gd.Woker < command.WorkerCost ||
                    !command.CanExecute(gd) ||
                    !DoesStackExactlyMatchCommand(counts, targetIdentity.cardId, command))
                {
                    continue;
                }

                if (!TryTakeCommandMaterialsFromStack(
                    stackCards,
                    target,
                    command,
                    out List<GameObject> materialCards,
                    out List<int> materialIndexes))
                {
                    continue;
                }

                var workCards = new List<GameObject> { target };
                workCards.AddRange(materialCards);

                bool canCancel = !IsUncancelableBasicCommand(targetIdentity.cardId, command);
                Coroutine routine = null;
                CardWorkService.WorkHandle handle;
                System.Action onCancel = () =>
                {
                    if (routine != null)
                        StopCoroutine(routine);

                    gd.AddWorker(command.WorkerCost);
                    RecalcSafe();
                };

                if (!CardWorkService.TryBeginStackWork(
                    residents,
                    workCards,
                    command.Duration,
                    canCancel,
                    canCancel ? onCancel : null,
                    out handle))
                {
                    continue;
                }

                gd.Skill = false;
                gd.AddWorker(-command.WorkerCost);
                routine = StartCoroutine(RunStackCommand(
                    command,
                    target,
                    residents,
                    materialCards,
                    materialIndexes,
                    handle));

                return true;
            }
        }

        return false;
    }

    public void SetSelectedCard(GameObject card)
    {
        _selectedCard = card;
    }

    public GameObject GetSelectedCard()
    {
        return _selectedCard;
    }

    private IEnumerator RunCommand(ICardCommand cmd)
    {
        var gd = DataController.instance.gameData;
        GameObject target = _selectedCard;

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

        if (target == null ||
            !target.activeInHierarchy ||
            CardWorkService.IsLocked(target))
        {
            wokerError?.SetActive(true);
            yield break;
        }

        if (!TryReserveCommandMaterials(
            cmd,
            target,
            out List<GameObject> materialCards,
            out List<int> materialIndexes))
        {
            wokerError?.SetActive(true);
            yield break;
        }

        var workCards = new List<GameObject> { target };
        workCards.AddRange(materialCards);

        if (!CardWorkService.TryBeginWork(
            workCards,
            cmd.WorkerCost,
            cmd.Duration,
            out CardWorkService.WorkHandle workHandle))
        {
            workerError.SetActive(true);
            yield break;
        }

        gd.Skill = false;

        gd.AddWorker(-cmd.WorkerCost);
        yield return StartCoroutine(cmd.Execute(this, gd));

        for (int i = 0; i < materialCards.Count; i++)
            RemoveSpecificCard(materialCards[i], materialIndexes[i]);

        if (cmd.ConsumeTarget &&
            TryGetCardIndex(target, out int targetIndex))
        {
            RemoveSpecificCard(target, targetIndex);
        }

        gd.AddWorker(cmd.WorkerCost);
        CardWorkService.EndResidentWork(workHandle);

        RecalcSafe();
    }

    private IEnumerator RunStackCommand(
        ICardCommand command,
        GameObject target,
        List<GameObject> residents,
        List<GameObject> materialCards,
        List<int> materialIndexes,
        CardWorkService.WorkHandle workHandle)
    {
        var gd = DataController.instance.gameData;

        yield return StartCoroutine(command.Execute(this, gd));

        if (workHandle != null && workHandle.IsCanceled)
            yield break;

        ApplyResidentUpgrade(command, residents);

        for (int i = 0; i < materialCards.Count; i++)
            RemoveSpecificCard(materialCards[i], materialIndexes[i]);

        if (command.ConsumeTarget &&
            TryGetCardIndex(target, out int targetIndex))
        {
            RemoveSpecificCard(target, targetIndex);
        }

        gd.AddWorker(command.WorkerCost);
        CardWorkService.EndResidentWork(workHandle);

        RecalcSafe();
    }

    private bool TryReserveCommandMaterials(
        ICardCommand command,
        GameObject target,
        out List<GameObject> cards,
        out List<int> indexes)
    {
        cards = new List<GameObject>();
        indexes = new List<int>();

        for (int i = 0; i < command.Requirements.Count; i++)
        {
            CardRequirement requirement = command.Requirements[i];
            List<GameObject> found = FindAvailableCardsByIndex(
                requirement.CardIndex,
                requirement.Count);

            for (int u = found.Count - 1; u >= 0; u--)
            {
                if (found[u] == target || cards.Contains(found[u]))
                    found.RemoveAt(u);
            }

            if (found.Count < requirement.Count)
                return false;

            for (int u = 0; u < requirement.Count; u++)
            {
                cards.Add(found[u]);
                indexes.Add(requirement.CardIndex);
            }
        }

        return true;
    }

    private bool TryTakeCommandMaterialsFromStack(
        IList<GameObject> stackCards,
        GameObject target,
        ICardCommand command,
        out List<GameObject> cards,
        out List<int> indexes)
    {
        cards = new List<GameObject>();
        indexes = new List<int>();

        for (int i = 0; i < command.Requirements.Count; i++)
        {
            CardRequirement requirement = command.Requirements[i];
            CardId id = _stdIndexToCardId[requirement.CardIndex];
            int taken = 0;

            for (int u = 0; u < stackCards.Count && taken < requirement.Count; u++)
            {
                GameObject card = stackCards[u];
                if (card == null ||
                    card == target ||
                    cards.Contains(card) ||
                    !DoesCardMatch(card, id, requirement.CardIndex))
                {
                    continue;
                }

                cards.Add(card);
                indexes.Add(requirement.CardIndex);
                taken++;
            }

            if (taken < requirement.Count)
                return false;
        }

        return true;
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
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();
        return CardSpawnPositionFinder.FindAvailablePosition(gd.Card);
    }

    private static string NormalizeName(string objectName)
    {
        const string clone = "(Clone)";
        return objectName.EndsWith(clone) ? objectName.Replace(clone, "") : objectName;
    }

    public void removeCard(int stdIndex)
    {
        if (stdIndex < 0 || stdIndex >= _stdIndexToCardId.Length) return;

        RemoveFirstCardById(_stdIndexToCardId[stdIndex], stdIndex);

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

            if (spawner != null) spawner.Despawn(card);
            else if (pool != null) pool.Release(card);
            else card.SetActive(false);

            gd.stdCardCount(stdCardCountIndex);
            return;
        }

    }

    private void RemoveFirstCardById(CardId id, int stdCardCountIndex)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        for (int idx = 0; idx < gd.Card.Count; idx++)
        {
            var card = gd.Card[idx];
            if (card == null) continue;

            if (card.TryGetComponent<CardIdentity>(out var ident) && ident.cardId == id)
            {
                gd.Card.RemoveAt(idx);

                FindObjectOfType<CardSpawner>()?.Despawn(card);

                gd.stdCardCount(stdCardCountIndex);
                return;
            }

            if(NormalizeName(card.name) == _indexToName[stdCardCountIndex])
            {
                gd.Card.RemoveAt(idx);
                FindObjectOfType<CardSpawner>()?.Despawn(card);
                gd.stdCardCount(stdCardCountIndex);
                return;
            }
        }
    }

    public List<GameObject> FindAvailableCardsByIndex(int stdIndex, int count)
    {
        var result = new List<GameObject>();
        if (stdIndex < 0 || stdIndex >= _stdIndexToCardId.Length || count <= 0)
            return result;

        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();
        CardId id = _stdIndexToCardId[stdIndex];

        for (int i = 0; i < gd.Card.Count && result.Count < count; i++)
        {
            GameObject card = gd.Card[i];
            if (card == null ||
                !card.activeInHierarchy ||
                CardWorkService.IsLocked(card) ||
                !DoesCardMatch(card, id, stdIndex))
            {
                continue;
            }

            result.Add(card);
        }

        return result;
    }

    public bool RemoveSpecificCard(GameObject card, int stdIndex)
    {
        if (card == null ||
            stdIndex < 0 ||
            stdIndex >= _stdIndexToCardId.Length)
        {
            return false;
        }

        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (!DoesCardMatch(card, _stdIndexToCardId[stdIndex], stdIndex))
            return false;

        if (!gd.Card.Remove(card))
            return false;

        if (spawner != null) spawner.Despawn(card);
        else if (pool != null) pool.Release(card);
        else card.SetActive(false);

        gd.stdCardCount(stdIndex);
        return true;
    }

    private static bool DoesCardMatch(GameObject card, CardId id, int stdIndex)
    {
        if (card == null) return false;

        if (card.TryGetComponent<CardIdentity>(out var identity))
            return identity.cardId == id;

        return stdIndex >= 0 &&
            stdIndex < _indexToName.Length &&
            NormalizeName(card.name) == _indexToName[stdIndex];
    }

    private static bool TryGetCardIndex(GameObject card, out int index)
    {
        index = -1;
        if (card == null ||
            !card.TryGetComponent(out CardIdentity identity))
        {
            return false;
        }

        for (int i = 0; i < _stdIndexToCardId.Length; i++)
        {
            if (_stdIndexToCardId[i] != identity.cardId)
                continue;

            index = i;
            return true;
        }

        return false;
    }

    public bool TryGetCardIndexForCard(GameObject card, out int index)
    {
        return TryGetCardIndex(card, out index);
    }

    private static Dictionary<CardId, int> CountCardsById(IList<GameObject> cards)
    {
        var counts = new Dictionary<CardId, int>();

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null ||
                !card.activeInHierarchy ||
                !card.TryGetComponent(out CardIdentity identity))
            {
                continue;
            }

            if (!counts.ContainsKey(identity.cardId))
                counts[identity.cardId] = 0;

            counts[identity.cardId]++;
        }

        return counts;
    }

    private static List<GameObject> FindCardsById(
        IList<GameObject> cards,
        CardId id)
    {
        var result = new List<GameObject>();

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card != null &&
                card.activeInHierarchy &&
                card.TryGetComponent(out CardIdentity identity) &&
                identity.cardId == id)
            {
                result.Add(card);
            }
        }

        return result;
    }

    private static bool DoesStackExactlyMatchCommand(
        Dictionary<CardId, int> actual,
        CardId targetId,
        ICardCommand command)
    {
        var expected = new Dictionary<CardId, int>
        {
            { CardId.Player, command.WorkerCost },
            { targetId, 1 }
        };

        for (int i = 0; i < command.Requirements.Count; i++)
        {
            CardRequirement requirement = command.Requirements[i];
            CardId id = _stdIndexToCardId[requirement.CardIndex];
            if (!expected.ContainsKey(id))
                expected[id] = 0;

            expected[id] += requirement.Count;
        }

        return CountsEqual(actual, expected);
    }

    private static bool CountsEqual(
        Dictionary<CardId, int> actual,
        Dictionary<CardId, int> expected)
    {
        if (actual.Count != expected.Count)
            return false;

        foreach (KeyValuePair<CardId, int> pair in expected)
        {
            if (!actual.TryGetValue(pair.Key, out int count) ||
                count != pair.Value)
            {
                return false;
            }
        }

        return true;
    }

    private static string[] GetCommandKeysForTarget(CardId targetId)
    {
        switch (targetId)
        {
            case CardId.Tree:
                return new[] { "Tree" };
            case CardId.Rock:
                return new[] { "Rock" };
            case CardId.BananaTree:
                return new[] { "BananaTree" };
            case CardId.StrawBerryTree:
                return new[] { "StrawBerryTree" };
            case CardId.Wood:
                return new[] { "Wood" };
            case CardId.Forge:
                return new[] { "ForgeIron", "ForgeGold" };
            case CardId.Timber:
                return new[] { "Timber" };
            case CardId.Mine:
                return new[] { "Mine" };
            case CardId.House:
                return new[] { "House" };
            case CardId.Armory:
                return new[] { "WoodArmor", "IronArmor" };
            default:
                return System.Array.Empty<string>();
        }
    }

    private static void ApplyResidentUpgrade(
        ICardCommand command,
        List<GameObject> residents)
    {
        if (!(command is IResidentUpgradeCommand upgrade) ||
            residents == null ||
            residents.Count == 0)
        {
            return;
        }

        ResidentCombatStats stats = residents[0] != null
            ? residents[0].GetComponent<ResidentCombatStats>()
            : null;
        if (stats != null)
            upgrade.ApplyToResident(stats);
    }

    private static bool IsUncancelableBasicCommand(
        CardId targetId,
        ICardCommand command)
    {
        return command.ConsumeTarget &&
            command.Requirements.Count == 0 &&
            (targetId == CardId.Wood ||
             targetId == CardId.Tree ||
             targetId == CardId.Rock ||
             targetId == CardId.BananaTree ||
             targetId == CardId.StrawBerryTree);
    }

    private void RecalcSafe()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();
    }

    public void CreateBasicCard(int index) => CreateCard(true, index);
    public void CreateIntermediateCard(int index) => CreateCard(false, index);

    public bool ForceSpawnCard(CardId id)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (spawner == null)
            spawner = FindObjectOfType<CardSpawner>();

        if (spawner == null)
            return false;

        GameObject go = spawner.Spawn((int)id);
        if (go == null)
            return false;

        go.transform.SetPositionAndRotation(GetRandomSpawnPos(), Quaternion.identity);
        go.name = id.ToString();
        gd.Card.Add(go);
        AddGameDataCountForForcedCard(gd, id);
        RecalcSafe();
        return true;
    }

    private static void AddGameDataCountForForcedCard(GameData gd, CardId id)
    {
        switch (id)
        {
            case CardId.Player:
                gd.AddPlayer(1);
                gd.AddWorker(1);
                return;
            case CardId.Wood: gd.Add(GameData.CardType.Wood, 1); return;
            case CardId.Stone: gd.Add(GameData.CardType.Stone, 1); return;
            case CardId.Iron: gd.Add(GameData.CardType.Iron, 1); return;
            case CardId.Gold: gd.Add(GameData.CardType.Gold, 1); return;
            case CardId.Tree: gd.Add(GameData.CardType.Tree, 1); return;
            case CardId.BananaTree: gd.Add(GameData.CardType.BananaTree, 1); return;
            case CardId.Banana: gd.Add(GameData.CardType.Banana, 1); return;
            case CardId.StrawBerryTree: gd.Add(GameData.CardType.StrawBerryTree, 1); return;
            case CardId.StrawBerry: gd.Add(GameData.CardType.StrawBerry, 1); return;
            case CardId.House: gd.Add(GameData.CardType.House, 1); return;
            case CardId.Forge: gd.Add(GameData.CardType.Forge, 1); return;
            case CardId.Timber: gd.Add(GameData.CardType.Timber, 1); return;
            case CardId.Mine: gd.Add(GameData.CardType.Mine, 1); return;
            case CardId.Rock: gd.Add(GameData.CardType.Rock, 1); return;
            case CardId.Panel: gd.Add(GameData.CardType.Panel, 1); return;
            case CardId.Brick: gd.Add(GameData.CardType.Brick, 1); return;
            case CardId.IronIngot: gd.Add(GameData.CardType.IronIngot, 1); return;
            case CardId.GoldIngot: gd.Add(GameData.CardType.GoldIngot, 1); return;
            case CardId.Branch: gd.Add(GameData.CardType.Branch, 1); return;
            case CardId.Kitchen: gd.Add(GameData.CardType.Kitchen, 1); return;
            case CardId.Armory: gd.Add(GameData.CardType.Armory, 1); return;
        }
    }

    public void RemoveCardByIndex(int i)
    {
        if (i < 0 || i >= _indexToName.Length) return;
        RemoveFirstCardByPrefabName(_indexToName[i], i);
    }

    public void SpawnPlayerCard() => SpawnPlayer();
}
