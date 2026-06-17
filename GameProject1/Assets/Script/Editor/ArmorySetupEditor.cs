#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ArmorySetupEditor
{
    private const string SourcePrefabPath = "Assets/Prefab/Prefab/Forge.prefab";
    private const string ArmoryPrefabPath = "Assets/Prefab/Prefab/Armory.prefab";

    private static readonly string[] ScenePaths =
    {
        "Assets/Scenes/Tuto.unity",
        "Assets/Scenes/SampleScene.unity"
    };

    static ArmorySetupEditor()
    {
        EditorApplication.delayCall += InstallOnEditorLoad;
    }

    [MenuItem("Tools/Card Project/Install Armory")]
    public static void InstallArmory()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogWarning("[ArmorySetup] Exit Play Mode before installing.");
            return;
        }

        if (HasDirtyOpenScene())
        {
            Debug.LogWarning("[ArmorySetup] Save open scenes before installing the Armory.");
            return;
        }

        var armoryPrefab = EnsureArmoryPrefab();
        if (armoryPrefab == null) return;

        var previousSetup = EditorSceneManager.GetSceneManagerSetup();

        try
        {
            foreach (string scenePath in ScenePaths)
            {
                if (!File.Exists(scenePath)) continue;
                InstallIntoScene(scenePath, armoryPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ArmorySetup] Armory prefab and crafting UI installation complete.");
        }
        finally
        {
            if (previousSetup != null && previousSetup.Length > 0)
                EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
        }
    }

    private static void InstallOnEditorLoad()
    {
        if (EditorApplication.isCompiling ||
            EditorApplication.isUpdating ||
            EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (IsInstallationComplete()) return;
        InstallArmory();
    }

    private static GameObject EnsureArmoryPrefab()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(ArmoryPrefabPath) == null)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(SourcePrefabPath) == null)
            {
                Debug.LogError($"[ArmorySetup] Source prefab not found: {SourcePrefabPath}");
                return null;
            }

            if (!AssetDatabase.CopyAsset(SourcePrefabPath, ArmoryPrefabPath))
            {
                Debug.LogError("[ArmorySetup] Failed to copy the Forge prefab.");
                return null;
            }
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(ArmoryPrefabPath);
        try
        {
            bool changed = prefabRoot.name != "Armory";
            prefabRoot.name = "Armory";

            var identity = prefabRoot.GetComponent<CardIdentity>();
            if (identity == null)
            {
                identity = prefabRoot.AddComponent<CardIdentity>();
                changed = true;
            }

            if (identity.cardId != CardId.Armory)
            {
                identity.InitializeNew(CardId.Armory);
                changed = true;
            }

            if (changed)
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, ArmoryPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        return AssetDatabase.LoadAssetAtPath<GameObject>(ArmoryPrefabPath);
    }

    private static bool IsInstallationComplete()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(ArmoryPrefabPath) == null)
            return false;

        foreach (string scenePath in ScenePaths)
        {
            if (!File.Exists(scenePath)) continue;

            string sceneText = File.ReadAllText(scenePath);
            if (!sceneText.Contains("CraftList:"))
                continue;

            if (!sceneText.Contains("ArmoryCraftUi: {fileID: ") ||
                sceneText.Contains("ArmoryCraftUi: {fileID: 0}"))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasDirtyOpenScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).isDirty)
                return true;
        }

        return false;
    }

    private static void InstallIntoScene(string scenePath, GameObject armoryPrefab)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        bool changed = false;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (CardFactory factory in root.GetComponentsInChildren<CardFactory>(true))
                changed |= EnsureFactoryEntry(factory, armoryPrefab);

            foreach (CraftManager craftManager in root.GetComponentsInChildren<CraftManager>(true))
                changed |= EnsureCraftingUi(craftManager);
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static bool EnsureFactoryEntry(CardFactory factory, GameObject armoryPrefab)
    {
        var serializedFactory = new SerializedObject(factory);
        SerializedProperty entries = serializedFactory.FindProperty("entries");

        for (int i = 0; i < entries.arraySize; i++)
        {
            SerializedProperty entry = entries.GetArrayElementAtIndex(i);
            if (entry.FindPropertyRelative("id").intValue != (int)CardId.Armory) continue;

            SerializedProperty prefabProperty = entry.FindPropertyRelative("prefab");
            if (prefabProperty.objectReferenceValue == armoryPrefab) return false;

            prefabProperty.objectReferenceValue = armoryPrefab;
            serializedFactory.ApplyModifiedPropertiesWithoutUndo();
            return true;
        }

        int newIndex = entries.arraySize;
        entries.InsertArrayElementAtIndex(newIndex);
        SerializedProperty newEntry = entries.GetArrayElementAtIndex(newIndex);
        newEntry.FindPropertyRelative("id").intValue = (int)CardId.Armory;
        newEntry.FindPropertyRelative("prefab").objectReferenceValue = armoryPrefab;
        serializedFactory.ApplyModifiedPropertiesWithoutUndo();
        return true;
    }

    private static bool EnsureCraftingUi(CraftManager craftManager)
    {
        var serializedManager = new SerializedObject(craftManager);
        SerializedProperty craftListProperty = serializedManager.FindProperty("CraftList");
        SerializedProperty forgePanelProperty = serializedManager.FindProperty("ForgeCraftUi");
        SerializedProperty armoryPanelProperty = serializedManager.FindProperty("ArmoryCraftUi");

        var craftList = craftListProperty.objectReferenceValue as GameObject;
        var forgePanel = forgePanelProperty.objectReferenceValue as GameObject;
        if (craftList == null || forgePanel == null) return false;

        bool changed = false;

        Transform armoryCategory = FindDescendant(craftList.transform, "Armory");
        if (armoryCategory == null)
        {
            Transform sourceCategory = FindDescendant(craftList.transform, "Forge");
            if (sourceCategory == null) return false;

            GameObject clone = Object.Instantiate(sourceCategory.gameObject, sourceCategory.parent);
            clone.name = "Armory";
            clone.transform.SetAsLastSibling();
            SetUiText(clone, "무기고", null);
            armoryCategory = clone.transform;
            changed = true;
        }

        GameObject armoryPanel = armoryPanelProperty.objectReferenceValue as GameObject;
        if (armoryPanel == null)
        {
            Transform existing = FindDescendant(forgePanel.transform.parent, "ArmoryCraft");
            armoryPanel = existing != null
                ? existing.gameObject
                : Object.Instantiate(forgePanel, forgePanel.transform.parent);

            armoryPanel.name = "ArmoryCraft";
            armoryPanel.SetActive(false);

            Button craftButton = FindButton(armoryPanel, "ForgeCraft");
            if (craftButton == null)
                craftButton = armoryPanel.GetComponentInChildren<Button>(true);

            if (craftButton != null)
            {
                craftButton.gameObject.name = "ArmoryCraft";
                EnsureCraftButtonEvent(craftButton, craftManager);
            }

            SetUiText(
                armoryPanel,
                "무기고",
                "무기와 전투 장비를 제작할 수 있다.\n\n필요재료 : 벽돌 2개, 판자 2개, 철괴 1개\n소요시간 : 45초");

            armoryPanelProperty.objectReferenceValue = armoryPanel;
            serializedManager.ApplyModifiedPropertiesWithoutUndo();
            changed = true;
        }

        return changed;
    }

    private static void EnsureCraftButtonEvent(Button button, CraftManager craftManager)
    {
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            if (button.onClick.GetPersistentTarget(i) == craftManager &&
                button.onClick.GetPersistentMethodName(i) == nameof(CraftManager.CardCraft))
            {
                return;
            }
        }

        UnityEventTools.AddPersistentListener(button.onClick, craftManager.CardCraft);
        EditorUtility.SetDirty(button);
    }

    private static void SetUiText(GameObject root, string title, string description)
    {
        Text[] legacyTexts = root.GetComponentsInChildren<Text>(true);
        TextMeshProUGUI[] tmpTexts = root.GetComponentsInChildren<TextMeshProUGUI>(true);

        Text longestLegacy = null;
        TextMeshProUGUI longestTmp = null;

        foreach (Text text in legacyTexts)
        {
            if (longestLegacy == null || text.text.Length > longestLegacy.text.Length)
                longestLegacy = text;

            if (text.text.Contains("용광로") || text.text.Contains("Forge"))
                text.text = title;
        }

        foreach (TextMeshProUGUI text in tmpTexts)
        {
            if (longestTmp == null || text.text.Length > longestTmp.text.Length)
                longestTmp = text;

            if (text.text.Contains("용광로") || text.text.Contains("Forge"))
                text.text = title;
        }

        if (!string.IsNullOrEmpty(description))
        {
            if (longestTmp != null)
                longestTmp.text = description;
            else if (longestLegacy != null)
                longestLegacy.text = description;
        }

        if (legacyTexts.Length == 1 && string.IsNullOrEmpty(description))
            legacyTexts[0].text = title;

        if (tmpTexts.Length == 1 && string.IsNullOrEmpty(description))
            tmpTexts[0].text = title;
    }

    private static Transform FindDirectChild(Transform parent, string childName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName) return child;
        }

        return null;
    }

    private static Transform FindDescendant(Transform parent, string childName)
    {
        if (parent == null) return null;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child != parent && child.name == childName)
                return child;
        }

        return null;
    }

    private static Button FindButton(GameObject root, string objectName)
    {
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
        {
            if (button.gameObject.name == objectName)
                return button;
        }

        return null;
    }
}
#endif
