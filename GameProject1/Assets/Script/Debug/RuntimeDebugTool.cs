using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RuntimeDebugTool : MonoBehaviour
{
    private const int WindowId = 12061207;

    private Rect _windowRect = new Rect(20f, 20f, 520f, 680f);
    private Vector2 _scroll;
    private bool _visible;
    private CardId _selectedSpawnCard = CardId.Wood;
    private string _lastTestResult = "No test run yet.";
    private string _lastSaveLoadResult = "No save/load test run yet.";

    private CardManager _cardManager;
    private CommandManager _commandManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        if (FindObjectOfType<RuntimeDebugTool>() != null)
            return;

        GameObject go = new GameObject("RuntimeDebugTool");
        DontDestroyOnLoad(go);
        go.AddComponent<RuntimeDebugTool>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            _visible = !_visible;
            RefreshReferences();
        }
    }

    private void OnGUI()
    {
        if (!_visible)
            return;

        _windowRect = GUI.Window(WindowId, _windowRect, DrawWindow, "Debug Tool (F12)");
    }

    private void DrawWindow(int id)
    {
        RefreshReferences();

        _scroll = GUILayout.BeginScrollView(_scroll);
        DrawSpawnSection();
        DrawGameDataSection();
        DrawActionQueueSection();
        DrawSelectedCardSection();
        DrawRuleTestSection();
        DrawSaveLoadSection();
        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
    }

    private void DrawSpawnSection()
    {
        GUILayout.Label("Card Spawn");
        _selectedSpawnCard = (CardId)EnumPopup("Card ID", _selectedSpawnCard);

        GUI.enabled = _cardManager != null;
        if (GUILayout.Button("Force Spawn Card"))
        {
            bool spawned = _cardManager.ForceSpawnCard(_selectedSpawnCard);
            _lastTestResult = spawned
                ? $"Spawned {_selectedSpawnCard}."
                : $"Failed to spawn {_selectedSpawnCard}. Check CardFactory prefab mapping.";
        }
        GUI.enabled = true;

        GUILayout.Space(10f);
    }

    private void DrawGameDataSection()
    {
        GUILayout.Label("Current GameData");
        GameData gd = TryGetGameData();
        GUILayout.TextArea(gd != null ? BuildGameDataDump(gd) : "GameData not found.");
        GUILayout.Space(10f);
    }

    private void DrawActionQueueSection()
    {
        GUILayout.Label("ActionQueue / Command State");
        string commandText = _commandManager != null
            ? _commandManager.DebugStatus
            : "CommandManager not found.";

        GUILayout.TextArea(
            commandText + "\n" +
            $"Locked cards: {CardWorkService.LockedCardCount}\n" +
            $"Active work cards: {CardWorkService.ActiveWorkCardCount}");
        GUILayout.Space(10f);
    }

    private void DrawSelectedCardSection()
    {
        GUILayout.Label("Selected Card");

        GameObject selected = _cardManager != null
            ? _cardManager.GetSelectedCard()
            : null;

        if (selected == null)
        {
            GUILayout.TextArea("Selected card: None");
        }
        else if (selected.TryGetComponent(out CardIdentity identity))
        {
            GUILayout.TextArea(
                $"Object: {selected.name}\n" +
                $"Card ID: {identity.cardId} ({(int)identity.cardId})\n" +
                $"Unique ID: {identity.UniqueId}");
        }
        else
        {
            GUILayout.TextArea($"Object: {selected.name}\nCardIdentity not found.");
        }

        GUILayout.Space(10f);
    }

    private void DrawRuleTestSection()
    {
        GUILayout.Label("Combination Rule Test");
        if (GUILayout.Button("Test Card Action Rules"))
            _lastTestResult = BuildRuleTestResult();

        GUILayout.TextArea(_lastTestResult);
        GUILayout.Space(10f);
    }

    private void DrawSaveLoadSection()
    {
        GUILayout.Label("Save / Load Test");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Test"))
            _lastSaveLoadResult = RunSaveTest();

        if (GUILayout.Button("Load Test"))
            _lastSaveLoadResult = RunLoadTest();

        if (GUILayout.Button("Save + Load Test"))
            _lastSaveLoadResult = RunSaveLoadTest();
        GUILayout.EndHorizontal();

        GUILayout.TextArea(_lastSaveLoadResult);
    }

    private void RefreshReferences()
    {
        if (_cardManager == null)
            _cardManager = FindObjectOfType<CardManager>();

        if (_commandManager == null)
            _commandManager = FindObjectOfType<CommandManager>();
    }

    private static GameData TryGetGameData()
    {
        DataController controller = DataController.instance;
        return controller != null ? controller.gameData : null;
    }

    private static Enum EnumPopup(string label, Enum selected)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(80f));

        Array values = Enum.GetValues(selected.GetType());
        int currentIndex = 0;
        string[] names = new string[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            object value = values.GetValue(i);
            names[i] = value.ToString();
            if (value.Equals(selected))
                currentIndex = i;
        }

        int nextIndex = GUILayout.SelectionGrid(currentIndex, names, 3);
        GUILayout.EndHorizontal();

        return (Enum)values.GetValue(nextIndex);
    }

    private static string BuildGameDataDump(GameData gd)
    {
        if (gd == null)
            return "GameData not found.";

        gd.EnsureRuntimeDefaults();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Gold: {gd.gold}");
        sb.AppendLine($"Food: {gd.FoodCount}");
        sb.AppendLine($"CardCount / Limit: {gd.CardCount} / {gd.CardLimit}");
        sb.AppendLine($"Players / Workers / Enemies: {gd.PlayerCount} / {gd.Woker} / {gd.EnemyCount}");
        sb.AppendLine($"Day / Stage / Boss: {gd.Day} / {gd.Stage} / {gd.BossStage}");
        sb.AppendLine($"Quest: {gd.QusetNum}");
        sb.AppendLine($"Flags: Sell={gd.Sell}, Skill={gd.Skill}, Fight={gd.Fight}, EndDay={gd.endDay}");
        sb.AppendLine($"Runtime Cards: {gd.Card.Count}");
        sb.AppendLine();
        sb.AppendLine("Cards");
        sb.AppendLine($"Wood={gd.WoodCard}, Stone={gd.StoneCard}, Tree={gd.TreeCard}, Rock={gd.RockCard}");
        sb.AppendLine($"BananaTree={gd.BananaTreeCard}, Banana={gd.BananaCard}");
        sb.AppendLine($"StrawBerryTree={gd.StrawBerryTreeCard}, StrawBerry={gd.StrawBerryCard}");
        sb.AppendLine($"Iron={gd.IronCard}, Gold={gd.GoldCard}, IronIngot={gd.IronIngotCard}, GoldIngot={gd.GoldIngotCard}");
        sb.AppendLine($"Branch={gd.BranchCard}, Brick={gd.BrickCard}, Panel={gd.PanelCard}");
        sb.AppendLine($"House={gd.HouseCard}, Forge={gd.ForgeCard}, Timber={gd.TimberCard}, Mine={gd.MineCard}");
        sb.AppendLine($"Kitchen={gd.KitchenCard}, Armory={gd.ArmoryCard}");
        return sb.ToString();
    }

    private static string BuildRuleTestResult()
    {
        GameData gd = TryGetGameData();
        if (gd == null)
            return "GameData not found.";

        Dictionary<string, ICardCommand> commands = CardCommandRegistry.Bulid();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Rules found: {commands.Count}");

        foreach (KeyValuePair<string, ICardCommand> pair in commands)
        {
            ICardCommand command = pair.Value;
            sb.Append(pair.Key);
            sb.Append(": ");
            sb.Append(command.CanExecute(gd) ? "CanExecute" : "Blocked");
            sb.Append($", Worker={command.WorkerCost}, Duration={command.Duration:0.##}, ConsumeTarget={command.ConsumeTarget}");

            if (command.Requirements.Count > 0)
            {
                sb.Append(", Req=");
                for (int i = 0; i < command.Requirements.Count; i++)
                {
                    CardRequirement req = command.Requirements[i];
                    if (i > 0)
                        sb.Append(" + ");

                    sb.Append($"Index{req.CardIndex}x{req.Count}");
                }
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string RunSaveTest()
    {
        try
        {
            DataController.instance.SaveData();
            return $"Save OK\nPath: {Application.persistentDataPath}";
        }
        catch (Exception ex)
        {
            return $"Save failed\n{ex.GetType().Name}: {ex.Message}";
        }
    }

    private static string RunLoadTest()
    {
        try
        {
            DataController.instance.LoadData();
            DataController.instance.gameData.RaiseAllEvents();
            return $"Load OK\nPath: {Application.persistentDataPath}";
        }
        catch (Exception ex)
        {
            return $"Load failed\n{ex.GetType().Name}: {ex.Message}";
        }
    }

    private static string RunSaveLoadTest()
    {
        string saveResult = RunSaveTest();
        string loadResult = RunLoadTest();
        return saveResult + "\n\n" + loadResult;
    }
}
