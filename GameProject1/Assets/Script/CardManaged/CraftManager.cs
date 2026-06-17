using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private CardSpawner spawner;
    private class Req
    {
        public System.Func<int> CurrentCount { get; }
        public int Need { get; }
        public int RemoveIndex { get; }

        public Req(System.Func<int> currnetCount, int need, int removeIndex)
        {
            CurrentCount = currnetCount;
            Need = need;
            RemoveIndex = removeIndex;
        }
    }

    private class ReservedMaterial
    {
        public GameObject Card { get; }
        public int RemoveIndex { get; }

        public ReservedMaterial(GameObject card, int removeIndex)
        {
            Card = card;
            RemoveIndex = removeIndex;
        }
    }

    private class CraftRecipe
    {
        public int WorkerCost { get; }
        public float Duration { get; }
        public CardId OutputCardId { get; }
        public System.Action OnSuccess { get; }
        public System.Func<bool> QuestAdvanceIf { get; }
        public IReadOnlyList<Req> Requirements => _reqs;
        private readonly List<Req> _reqs;

        public CraftRecipe(int workerCost, float duration, CardId outputCardId,
            System.Action onSuccess,
            List<Req> requirements,
            System.Func<bool> questAdvanceIf = null)
        {
            WorkerCost = workerCost;
            Duration = duration;
            OutputCardId = outputCardId;
            OnSuccess = onSuccess;
            _reqs = requirements;
            QuestAdvanceIf = questAdvanceIf;
        }

        public bool CanCraft()
        {
            for (int i = 0; i < _reqs.Count; i++)
                if (_reqs[i].CurrentCount() < _reqs[i].Need)
                    return false;
            return true;
        }

        public List<ReservedMaterial> ReserveMaterials(CardManager cardManager)
        {
            var reserved = new List<ReservedMaterial>();

            for (int i = 0; i < _reqs.Count; i++)
            {
                List<GameObject> cards = cardManager.FindAvailableCardsByIndex(
                    _reqs[i].RemoveIndex,
                    _reqs[i].Need);

                if (cards.Count < _reqs[i].Need)
                    return null;

                for (int u = 0; u < cards.Count; u++)
                    reserved.Add(new ReservedMaterial(cards[u], _reqs[i].RemoveIndex));
            }

            return reserved;
        }

        public void ConsumeMaterials(
            CardManager cardManager,
            List<ReservedMaterial> reserved)
        {
            for (int i = 0; i < reserved.Count; i++)
                cardManager.RemoveSpecificCard(
                    reserved[i].Card,
                    reserved[i].RemoveIndex);
        }
    }


    [Header("Legacy Prefabs")]
    [SerializeField, HideInInspector] private GameObject[] CraftCardSet = new GameObject[4];

    [Header("UI")]
    [SerializeField] private GameObject ErrorUi;
    [SerializeField] private GameObject CraftUI;

    [SerializeField] private GameObject CraftList;
    [SerializeField] private GameObject HouseCraftUI;
    [SerializeField] private GameObject ForgeCraftUi;
    [SerializeField] private GameObject TimberCraftUi;
    [SerializeField] private GameObject MineCraftUi;
    [SerializeField] private GameObject ArmoryCraftUi;

    [Header("Dependency")]
    [SerializeField] private CardManager _cardManager; //인스펙터 연결

    private Dictionary<string, CraftRecipe> _recipes;
    private Dictionary<string, GameObject> _categoryPanels;

    private void Awake()
    {
        var gd = DataController.instance.gameData;

        if (spawner == null)
            spawner = FindObjectOfType<CardSpawner>();

        if (_cardManager == null)
            _cardManager = FindObjectOfType<CardManager>();

        _categoryPanels = new Dictionary<string, GameObject> //추후 카드 추가시 여기에 추가
        {
            {"House", HouseCraftUI },
            { "Forge", ForgeCraftUi },
            { "Timber", TimberCraftUi },
            { "Mine",  MineCraftUi },
            { "Armory", ArmoryCraftUi },
        };

        _recipes = new Dictionary<string, CraftRecipe> // 제작 카드 레시피
        {
            {"HouseCraft", new CraftRecipe(
                workerCost: 1,
                duration: 60f,
                outputCardId: CardId.House,
                onSuccess: () => gd.Add(GameData.CardType.House, 1),
                requirements: new List<Req>
                {
                    new Req(() => DataController.instance.gameData.PanelCard, 3, removeIndex: 14),
                    new Req(() => DataController.instance.gameData.BrickCard, 3, removeIndex: 13),
                })
             },

             { "MineCraft", new CraftRecipe(
                  workerCost: 1,
                  duration: 15f,
                  outputCardId: CardId.Mine,
                  onSuccess: () => gd.Add(GameData.CardType.Mine, 1),
                  requirements: new List<Req>
                  {
                    new Req(() => DataController.instance.gameData.WoodCard, 1, removeIndex: 0),
                    new Req(() => DataController.instance.gameData.StoneCard, 3, removeIndex: 1),
                  },
                  questAdvanceIf: () => DataController.instance.gameData.QusetNum == 3)
            },

            // ForgeCraft: Branch 1 + Brick 2, 30초 후 CraftCardSet[1], ForgeCard++
            {
            "ForgeCraft", new CraftRecipe(
              workerCost: 1,
              duration: 30f,
              outputCardId: CardId.Forge,
              onSuccess: () => gd.Add(GameData.CardType.Forge, 1),
              requirements: new List<Req>
              {
                    new Req(() => DataController.instance.gameData.BranchCard, 1, removeIndex: 10),
                    new Req(() => DataController.instance.gameData.BrickCard, 2, removeIndex: 13),
              },
              questAdvanceIf: () => DataController.instance.gameData.QusetNum == 3)
            },

            // TimberCraft: Wood 3 + Stone 1, 15초 후 CraftCardSet[2], TimberCard++
            {
            "TimberCraft", new CraftRecipe(
              workerCost: 1,
              duration: 15f,
              outputCardId: CardId.Timber,
              onSuccess: () => gd.Add(GameData.CardType.Timber, 1),
              requirements: new List<Req>
              {
                    new Req(() => DataController.instance.gameData.WoodCard, 3, removeIndex: 0),
                    new Req(() => DataController.instance.gameData.StoneCard, 1, removeIndex: 1),
              },
              questAdvanceIf: () => DataController.instance.gameData.QusetNum == 3)
            },
            {
            "ArmoryCraft", new CraftRecipe(
              workerCost: 1,
              duration: 45f,
              outputCardId: CardId.Armory,
              onSuccess: () => gd.Add(GameData.CardType.Armory, 1),
              requirements: new List<Req>
              {
                    new Req(() => DataController.instance.gameData.BrickCard, 2, removeIndex: 13),
                    new Req(() => DataController.instance.gameData.PanelCard, 2, removeIndex: 14),
                    new Req(() => DataController.instance.gameData.IronIngotCard, 1, removeIndex: 11),
              })
            },
        };

    }

    public void CardCraft()
    {
        var gd = DataController.instance.gameData;

        string btnt = EventSystem.current.currentSelectedGameObject.name;

        if (btnt == "Kitchen")
        {
            TryCraftKitchenInstant();
            return;
        }
        if (gd.Woker == 0)
        {
            ErrorUi.SetActive(true);
            return;
        }

        if (_cardManager == null)
        {
            ErrorUi.SetActive(true);
            return;
        }

        if (!_recipes.TryGetValue(btnt, out var recipe))
            return;

        if (!recipe.CanCraft())
        {
            ErrorUi.SetActive(true);
            return;
        }

        List<ReservedMaterial> reserved = recipe.ReserveMaterials(_cardManager);
        if (reserved == null || reserved.Count == 0)
        {
            ErrorUi.SetActive(true);
            return;
        }

        var workCards = new List<GameObject>();
        for (int i = 0; i < reserved.Count; i++)
            workCards.Add(reserved[i].Card);

        if (!CardWorkService.TryBeginWork(
            workCards,
            recipe.WorkerCost,
            recipe.Duration,
            out CardWorkService.WorkHandle workHandle))
        {
            ErrorUi.SetActive(true);
            return;
        }

        Vector3 outputPosition = workHandle.AnchorPosition;

        StartCoroutine(CraftRoutine(
            recipe,
            reserved,
            outputPosition,
            workHandle));
    }

    public bool TryStartWorkFromStack(IList<GameObject> stackCards)
    {
        if (stackCards == null || stackCards.Count == 0)
            return false;

        if (_recipes == null)
            return false;

        var gd = DataController.instance.gameData;
        Dictionary<CardId, int> counts = CountCardsById(stackCards);
        List<GameObject> residents = FindCardsById(stackCards, CardId.Player);

        foreach (KeyValuePair<string, CraftRecipe> pair in _recipes)
        {
            CraftRecipe recipe = pair.Value;
            if (residents.Count != recipe.WorkerCost ||
                gd.Woker < recipe.WorkerCost ||
                !DoesStackExactlyMatchRecipe(counts, recipe))
            {
                continue;
            }

            if (!TryTakeRecipeMaterialsFromStack(
                stackCards,
                recipe,
                out List<ReservedMaterial> reserved,
                out List<GameObject> workCards))
            {
                continue;
            }

            Coroutine routine = null;
            CardWorkService.WorkHandle handle;
            System.Action onCancel = () =>
            {
                if (routine != null)
                    StopCoroutine(routine);

                gd.AddWorker(recipe.WorkerCost);
                RecalcSafe();
            };

            if (!CardWorkService.TryBeginStackWork(
                residents,
                workCards,
                recipe.Duration,
                canCancel: true,
                onCancel,
                out handle))
            {
                continue;
            }

            Vector3 outputPosition = handle.AnchorPosition;

            gd.AddWorker(-recipe.WorkerCost);
            routine = StartCoroutine(CraftRoutine(
                recipe,
                reserved,
                outputPosition,
                handle,
                workerAlreadyReserved: true));

            return true;
        }

        return _cardManager != null &&
            _cardManager.TryStartCommandFromStack(stackCards);
    }

    private IEnumerator CraftRoutine(
        CraftRecipe recipe,
        List<ReservedMaterial> reserved,
        Vector3 outputPosition,
        CardWorkService.WorkHandle workHandle,
        bool workerAlreadyReserved = false)
    {
        var gd = DataController.instance.gameData;

        if (!workerAlreadyReserved)
            gd.AddWorker(-recipe.WorkerCost);

        CraftUI.SetActive(false);
        HideAllCategoryPanels();

        yield return new WaitForSeconds(recipe.Duration);

        if (workHandle != null && workHandle.IsCanceled)
            yield break;

        recipe.ConsumeMaterials(_cardManager, reserved);
        SpawnCraftOutput(recipe.OutputCardId, outputPosition);

        if (recipe.QuestAdvanceIf != null && recipe.QuestAdvanceIf())
            gd.AddQuest(1);

        recipe.OnSuccess?.Invoke();

        gd.AddWorker(recipe.WorkerCost);
        CardWorkService.EndResidentWork(workHandle);

        RecalcSafe();
        yield break;
    }

    public void CraftUi()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        CraftList.SetActive(false);
        HideAllCategoryPanels();

        if (_categoryPanels.TryGetValue(clickObject.name, out var panel) && panel != null)
            panel.SetActive(true);
    }

    public void backspaceBtn()
    {
        CraftList.SetActive(true);
        HideAllCategoryPanels();
    }

    public void ErrorClose()
    {
        ErrorUi.SetActive(false);
    }

    private void HideAllCategoryPanels()
    {
        HouseCraftUI.SetActive(false);
        ForgeCraftUi.SetActive(false);
        TimberCraftUi.SetActive(false);
        MineCraftUi.SetActive(false);
        if (ArmoryCraftUi != null) ArmoryCraftUi.SetActive(false);
    }

    private void SpawnCraftOutput(CardId id, Vector3? preferredPosition = null)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (spawner == null) return;

        var go = spawner.Spawn((int)id);
        if (go == null) return;

        Vector3 position = preferredPosition ?? GetRandomSpawnPos();
        position.z = 0f;
        go.transform.SetPositionAndRotation(position, Quaternion.identity);
        go.name = id.ToString();
        gd.Card.Add(go);
        RecalcSafe();
    }

    private void SpawnCraftOutput(int prefabIndex)
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if (prefabIndex < 0 || prefabIndex >= CraftCardSet.Length) return;
        if (spawner == null) return;

        Vector3 pos = GetRandomSpawnPos();

        //이름을 id로 변환해서 spawn
        //TODO 이름기반을 id기반으로 전부 변경하기
        var prefabName = CraftCardSet[prefabIndex] != null ? CraftCardSet[prefabIndex].name : null;
        if (string.IsNullOrEmpty(prefabName)) return;

        CardId id;
        switch (prefabName)
        {
            case "House": id = CardId.House; break;
            case "Forge": id = CardId.Forge; break;
            case "Timber": id = CardId.Timber; break;
            case "Mine": id = CardId.Mine; break;
            default:
                // 여기 걸리면 CraftCardSet 프리팹 이름이 enum이랑 다르다는 뜻
                Debug.LogError($"[CraftManager] Unknown craft output prefab name: {prefabName}");
                return;
        }

        var go = spawner.Spawn((int)id);
        if (go == null) return;

        go.transform.SetPositionAndRotation(pos, Quaternion.identity);
        go.name = prefabName;

        gd.Card.Add(go);
        RecalcSafe();
    }

    private Vector3 GetRandomSpawnPos()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();
        return CardSpawnPositionFinder.FindAvailablePosition(gd.Card);
    }

    private void TryCraftKitchenInstant()
    {
        var gd = DataController.instance.gameData;

        if(gd.WoodCard >= 2 && gd.StoneCard >= 2 && gd.IronIngotCard >= 2)
        {
            for (int i = 0; i < 2; i++)
            {
                _cardManager.removeCard(0);
                _cardManager.removeCard(1);
                _cardManager.removeCard(11);
            }

            gd.Add(GameData.CardType.Kitchen, 1);
            RecalcSafe();
        }
        else
        {
            ErrorUi.SetActive(true);
        }
    }

    private void RecalcSafe()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();
    }

    private bool TryTakeRecipeMaterialsFromStack(
        IList<GameObject> stackCards,
        CraftRecipe recipe,
        out List<ReservedMaterial> reserved,
        out List<GameObject> workCards)
    {
        reserved = new List<ReservedMaterial>();
        workCards = new List<GameObject>();

        for (int i = 0; i < recipe.Requirements.Count; i++)
        {
            Req requirement = recipe.Requirements[i];
            CardId id = IndexToCardId(requirement.RemoveIndex);
            int taken = 0;

            for (int u = 0; u < stackCards.Count && taken < requirement.Need; u++)
            {
                GameObject card = stackCards[u];
                if (card == null ||
                    reserved.Exists(material => material.Card == card) ||
                    !DoesCardMatch(card, id))
                {
                    continue;
                }

                reserved.Add(new ReservedMaterial(card, requirement.RemoveIndex));
                workCards.Add(card);
                taken++;
            }

            if (taken < requirement.Need)
                return false;
        }

        return true;
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

    private static bool DoesStackExactlyMatchRecipe(
        Dictionary<CardId, int> actual,
        CraftRecipe recipe)
    {
        var expected = new Dictionary<CardId, int>
        {
            { CardId.Player, recipe.WorkerCost }
        };

        for (int i = 0; i < recipe.Requirements.Count; i++)
        {
            Req requirement = recipe.Requirements[i];
            CardId id = IndexToCardId(requirement.RemoveIndex);
            if (!expected.ContainsKey(id))
                expected[id] = 0;

            expected[id] += requirement.Need;
        }

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

    private static bool DoesCardMatch(GameObject card, CardId id)
    {
        return card != null &&
            card.TryGetComponent(out CardIdentity identity) &&
            identity.cardId == id;
    }

    private static CardId IndexToCardId(int index)
    {
        switch (index)
        {
            case 0: return CardId.Wood;
            case 1: return CardId.Stone;
            case 2: return CardId.Tree;
            case 3: return CardId.Rock;
            case 4: return CardId.BananaTree;
            case 5: return CardId.Banana;
            case 6: return CardId.StrawBerry;
            case 7: return CardId.StrawBerryTree;
            case 8: return CardId.Iron;
            case 9: return CardId.Gold;
            case 10: return CardId.Branch;
            case 11: return CardId.IronIngot;
            case 12: return CardId.GoldIngot;
            case 13: return CardId.Brick;
            case 14: return CardId.Panel;
            case 15: return CardId.House;
            case 16: return CardId.Forge;
            case 17: return CardId.Timber;
            case 18: return CardId.Mine;
            case 19: return CardId.Kitchen;
            case 20: return CardId.Player;
            case 21: return CardId.Armory;
            default: return CardId.Wood;
        }
    }
}
