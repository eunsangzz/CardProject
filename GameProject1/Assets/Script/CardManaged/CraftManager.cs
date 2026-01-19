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

    private class CraftRecipe
    {
        public int WorkerCost { get; }
        public float Duration { get; }
        public int OutputSpawnIndex { get; }
        public System.Action OnSuccess { get; }
        public System.Func<bool> QuestAdvanceIf { get; }
        private readonly List<Req> _reqs;

        public CraftRecipe(int workerCost, float duration, int outputSpawnIndex,
            System.Action onSuccess,
            List<Req> requirements,
            System.Func<bool> questAdvanceIf = null)
        {
            WorkerCost = workerCost;
            Duration = duration;
            OutputSpawnIndex = outputSpawnIndex;
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

        public void ConsumeMaterials(CardManager cardManager)
        {
            for (int i = 0; i < _reqs.Count; i++)
            {
                for (int u = 0; u < _reqs[i].Need; u++)
                    cardManager.removeCard(_reqs[i].RemoveIndex);
            }

        }
    }


    [Header("Prefabs")]
    [SerializeField] private GameObject[] CraftCardSet = new GameObject[4];

    [Header("UI")]
    [SerializeField] private GameObject ErrorUi;
    [SerializeField] private GameObject CraftUI;

    [SerializeField] private GameObject CraftList;
    [SerializeField] private GameObject HouseCraftUI;
    [SerializeField] private GameObject ForgeCraftUi;
    [SerializeField] private GameObject TimberCraftUi;
    [SerializeField] private GameObject MineCraftUi;

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
        };

        _recipes = new Dictionary<string, CraftRecipe> // 제작 카드 레시피
        {
            {"HouseCraft", new CraftRecipe(
                workerCost: 1,
                duration: 60f,
                outputSpawnIndex: 0,
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
                  outputSpawnIndex: 3,
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
              outputSpawnIndex: 1,
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
              outputSpawnIndex: 2,
              onSuccess: () => gd.Add(GameData.CardType.Timber, 1),
              requirements: new List<Req>
              {
                    new Req(() => DataController.instance.gameData.WoodCard, 3, removeIndex: 0),
                    new Req(() => DataController.instance.gameData.StoneCard, 1, removeIndex: 1),
              },
              questAdvanceIf: () => DataController.instance.gameData.QusetNum == 3)
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

        recipe.ConsumeMaterials(_cardManager);

        if (recipe.QuestAdvanceIf != null && recipe.QuestAdvanceIf())
            gd.AddQuest(1);

        StartCoroutine(CraftRoutine(recipe));
    }

    private IEnumerator CraftRoutine(CraftRecipe recipe)
    {
        var gd = DataController.instance.gameData;

        gd.AddWorker(-recipe.WorkerCost);

        CraftUI.SetActive(false);
        HideAllCategoryPanels();

        yield return new WaitForSeconds(recipe.Duration);

        SpawnCraftOutput(recipe.OutputSpawnIndex);

        recipe.OnSuccess?.Invoke();

        gd.AddWorker(recipe.WorkerCost);

        RecalcSafe();
        yield break;
    }

    public void CraftUi()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        CraftList.SetActive(false);
        HideAllCategoryPanels();

        if (_categoryPanels.TryGetValue(clickObject.name, out var panel))
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
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-4f, 2f);
        return new Vector3(x, y, 0f);
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
}
