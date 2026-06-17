using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class RuntimeStackUiLayout : MonoBehaviour
{
    private static RuntimeStackUiLayout _instance;

    private Canvas _canvas;
    private RectTransform _root;
    private TextMeshProUGUI _cardText;
    private TextMeshProUGUI _foodText;
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _timeText;
    private GameObject _recipeOverlay;
    private DayNightManager _dayNight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (_instance != null)
            return;

        var host = new GameObject("RuntimeStackUiLayout");
        DontDestroyOnLoad(host);
        _instance = host.AddComponent<RuntimeStackUiLayout>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshSceneUi();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Update()
    {
        if (_canvas == null)
            RefreshSceneUi();

        HideCardSkillUi();
        EnsureRecipeOverlayState();
        UpdateTopHud();
    }

    private void LateUpdate()
    {
        HideCardSkillUi();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshSceneUi();
    }

    private void RefreshSceneUi()
    {
        _canvas = FindObjectOfType<Canvas>();
        _dayNight = DayNightManager.Instance != null
            ? DayNightManager.Instance
            : FindObjectOfType<DayNightManager>();

        if (_canvas == null)
            return;

        BuildTopHud();
        RenameCraftTab();
        HideLegacyTopHud();
        HideCardSkillUi();
        EnsureRecipeOverlayState();
    }

    private void BuildTopHud()
    {
        Transform oldRoot = _canvas.transform.Find("RuntimeStackTopHud");
        if (oldRoot != null)
            Destroy(oldRoot.gameObject);

        GameObject rootObject = new GameObject("RuntimeStackTopHud", typeof(RectTransform));
        rootObject.transform.SetParent(_canvas.transform, false);
        rootObject.transform.SetAsLastSibling();

        _root = rootObject.GetComponent<RectTransform>();
        _root.anchorMin = Vector2.zero;
        _root.anchorMax = Vector2.one;
        _root.offsetMin = Vector2.zero;
        _root.offsetMax = Vector2.zero;

        GameObject stats = CreatePanel(
            "CardFoodGold",
            _root,
            new Vector2(520f, 46f),
            new Vector2(-214f, -12f));

        _cardText = CreateText("CardText", stats.transform, TextAlignmentOptions.Left);
        _foodText = CreateText("FoodText", stats.transform, TextAlignmentOptions.Center);
        _goldText = CreateText("GoldText", stats.transform, TextAlignmentOptions.Right);

        ArrangeTextColumn(_cardText.rectTransform, 0f, 0.34f);
        ArrangeTextColumn(_foodText.rectTransform, 0.33f, 0.67f);
        ArrangeTextColumn(_goldText.rectTransform, 0.66f, 1f);

        GameObject time = CreatePanel(
            "Time",
            _root,
            new Vector2(190f, 46f),
            new Vector2(-12f, -12f));

        _timeText = CreateText("TimeText", time.transform, TextAlignmentOptions.Center);
        Stretch(_timeText.rectTransform, 8f, 4f);

        UpdateTopHud();
    }

    private static GameObject CreatePanel(
        string name,
        Transform parent,
        Vector2 size,
        Vector2 anchoredPosition)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        Image image = panel.GetComponent<Image>();
        image.color = new Color(0.96f, 0.92f, 0.78f, 0.96f);

        return panel;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 24f;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.black;
        text.alignment = alignment;
        text.enableWordWrapping = false;

        return text;
    }

    private static void ArrangeTextColumn(RectTransform rect, float minX, float maxX)
    {
        rect.anchorMin = new Vector2(minX, 0f);
        rect.anchorMax = new Vector2(maxX, 1f);
        rect.offsetMin = new Vector2(10f, 4f);
        rect.offsetMax = new Vector2(-10f, -4f);
    }

    private static void Stretch(RectTransform rect, float horizontalPadding, float verticalPadding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
        rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
    }

    private void UpdateTopHud()
    {
        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;

        if (gd == null)
            return;

        if (_cardText != null)
            _cardText.text = $"카드 {gd.CardCount}/{gd.CardLimit}";

        if (_foodText != null)
            _foodText.text = $"음식 {gd.FoodCount}/{gd.PlayerCount}";

        if (_goldText != null)
            _goldText.text = $"골드 {gd.gold}";

        if (_timeText != null)
        {
            int seconds = _dayNight != null
                ? Mathf.CeilToInt(_dayNight.TimeLeft)
                : 0;

            _timeText.text = $"{gd.Day}일차  {seconds}s";
        }
    }

    private void RenameCraftTab()
    {
        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform == null || transform.name != "Craftbtn")
                continue;

            SetChildText(transform.gameObject, "레시피");
        }
    }

    private void HideLegacyTopHud()
    {
        string[] names =
        {
            "Gold", "FoodCount", "CardCount", "Day", "DayCount"
        };

        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform == null ||
                IsInsideRuntimeUi(transform) ||
                transform.GetComponentInParent<Canvas>() == null ||
                transform.GetComponentInChildren<TextMeshProUGUI>(true) == null)
            {
                continue;
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (transform.name == names[i])
                    transform.gameObject.SetActive(false);
            }
        }
    }

    private void HideCardSkillUi()
    {
        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;

        if (gd != null)
            gd.Skill = false;

        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform != null && transform.name == "CardSkillUi")
                transform.gameObject.SetActive(false);
        }
    }

    private void EnsureRecipeOverlayState()
    {
        Transform craftUi = FindSceneTransform("CraftUi");
        if (craftUi == null)
            return;

        if (_recipeOverlay == null || _recipeOverlay.transform.parent != craftUi)
            _recipeOverlay = BuildRecipeOverlay(craftUi);

        _recipeOverlay.SetActive(craftUi.gameObject.activeInHierarchy);
    }

    private GameObject BuildRecipeOverlay(Transform craftUi)
    {
        Transform old = craftUi.Find("RuntimeRecipeList");
        if (old != null)
            Destroy(old.gameObject);

        GameObject overlay = new GameObject("RuntimeRecipeList", typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(craftUi, false);
        overlay.transform.SetAsLastSibling();

        RectTransform rect = overlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(16f, 16f);
        rect.offsetMax = new Vector2(-16f, -16f);

        Image image = overlay.GetComponent<Image>();
        image.color = new Color(0.96f, 0.92f, 0.78f, 1f);

        GameObject textObject = new GameObject("RecipeText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(overlay.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(22f, 18f);
        textRect.offsetMax = new Vector2(-22f, -18f);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 18f;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.enableWordWrapping = true;
        text.lineSpacing = 3f;
        text.paragraphSpacing = 5f;
        text.text = RecipeText;

        return overlay;
    }

    private static void SetChildText(GameObject root, string value)
    {
        TextMeshProUGUI[] texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
            texts[i].text = value;

        Text[] legacyTexts = root.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < legacyTexts.Length; i++)
            legacyTexts[i].text = value;
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in GetSceneTransforms())
        {
            if (transform != null && transform.name == name)
                return transform;
        }

        return null;
    }

    private static Transform[] GetSceneTransforms()
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        var sceneTransforms = new List<Transform>();

        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform transform = allTransforms[i];
            if (transform != null &&
                transform.gameObject.scene.IsValid() &&
                transform.gameObject.scene.isLoaded)
            {
                sceneTransforms.Add(transform);
            }
        }

        return sceneTransforms.ToArray();
    }

    private bool IsInsideRuntimeUi(Transform transform)
    {
        return _root != null && transform.IsChildOf(_root);
    }

    private const string RecipeText =
        "레시피\n\n" +
        "스택 작업 규칙\n" +
        "필요한 카드만 한 스택에 올려야 작업이 시작됩니다.\n" +
        "다른 카드가 섞이거나 주민 수가 다르면 시작되지 않습니다.\n" +
        "우클릭 중단 가능 작업은 진행 중인 스택을 우클릭해 취소할 수 있습니다.\n\n" +
        "기본 채집 - 중단 불가\n" +
        "주민 + 나무 = 목재 2개 / 4초\n" +
        "주민 + 바위 = 석재 2개 / 4초\n" +
        "주민 + 목재 = 나뭇가지 3개 / 6초\n" +
        "주민 + 바나나나무 = 바나나 3개 / 6초\n" +
        "주민 + 딸기나무 = 딸기 3개 / 6초\n\n" +
        "건물 제작 - 우클릭 중단 가능\n" +
        "주민 + 목재 1 + 석재 3 = 광산 / 15초\n" +
        "주민 + 목재 3 + 석재 1 = 제재소 / 15초\n" +
        "주민 + 나뭇가지 1 + 벽돌 2 = 화로 / 30초\n" +
        "주민 + 판자 3 + 벽돌 3 = 집 / 60초\n" +
        "주민 + 벽돌 2 + 판자 2 + 철괴 1 = 무기고 / 45초\n\n" +
        "시설 작업 - 우클릭 중단 가능\n" +
        "주민 + 제재소 + 목재 2 + 나뭇가지 1 = 판자 / 5초 / 골드 1 이상 필요\n" +
        "주민 + 광산 + 석재 2 = 벽돌 / 5초 / 골드 1 이상 필요\n" +
        "주민 + 화로 + 목재 2 + 나뭇가지 2 + 철광석 1 = 철괴 / 5초\n" +
        "주민 + 화로 + 목재 2 + 나뭇가지 1 + 금광석 1 = 금괴 / 5초\n" +
        "주민 2 + 집 = 주민 1명 / 60초 / 골드 16 이상 필요, 15 소모";
}
