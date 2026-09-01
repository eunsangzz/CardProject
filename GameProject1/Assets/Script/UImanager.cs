using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UImanager : MonoBehaviour
{
    private const string CookieRunFontResourcesPath = "Fonts/Regular SDF";
    private const string CookieRunFontAssetPath = "Assets/CookieRunFont_TTF/Regular SDF.asset";

    public GameObject Function;

    public GameObject craftUi;
    public GameObject craftListUi;
    public GameObject craftUiBtn;

    public GameObject menuUi;
    public GameObject menuUiBtn;

    public GameObject buyBtn;
    public GameObject SellUi;
    public GameObject SellBtn;

    public GameObject cardInfoUi;
    public GameObject cardSkillUi;

    // 스킬 버튼들(카드별로 1개만 켜기)
    public GameObject treeSkillBtn;
    public GameObject rockSkillBtn;
    public GameObject TimberSkillBtn;
    public GameObject ForgeSkillBtn;
    public GameObject MineSkillBtn;
    public GameObject WoodSkillBtn;
    public GameObject HouseSkillBtn;
    public GameObject bananaTreeBtn;
    public GameObject StrawBerryTreeBtn;

    public GameObject StoreUpBtn;

    // 튜토 UI 패널(텍스트는 HUDManager가 갱신)
    public GameObject tutoInfoUi;
    public GameObject tutoDayUi;
    public GameObject tutoBtnUi;
    public GameObject tutoBuy;
    public GameObject tutoSell;
    public GameObject tutoCraft;
    public GameObject tutoDay;
    public GameObject tutoStoreUp;
    public GameObject tutoStart;

    private bool tutoday;
    private bool tutocraft;
    private bool StoreUp;
    private GameObject runtimeRecipeUi;
    private TMP_FontAsset cookieRunFont;

    // 카드 정보 UI 텍스트(이건 HUD가 아니라 카드 클릭 정보라 UImanager가 유지)
    public TextMeshProUGUI CardInfoText;
    public TextMeshProUGUI CardNameText;

    public GameObject ErrorMessage;
    public TextMeshProUGUI ErrorMessageText;

    public GameObject GameOverMessage;
    public GameObject tutoEndUI;

    private float delayQuest;
    private bool Over = false;
    private int craftsibling;

    // [CHANGED] DayNightManager 참조(밤 시작 이벤트만 UI에서 사용)
    [SerializeField] private DayNightManager dayNight;
    [SerializeField] private CardManager cardManager;

    // [CHANGED] CardInfo/Skill 딕셔너리
    private readonly Dictionary<CardId, (string displayName, string desc)> _cardInfoMap
        = new Dictionary<CardId, (string, string)>();

    private readonly Dictionary<CardId, GameObject> _skillButtonMap
        = new Dictionary<CardId, GameObject>();

    private GameObject[] _allSkillButtons;

    private void Start()
    {
        SetActiveIfExists(cardInfoUi, false);

        tutoday = false;
        tutocraft = false;

        // 시작 튜토: 일시정지
        Time.timeScale = 0f;
        craftsibling = craftUiBtn != null ? craftUiBtn.transform.GetSiblingIndex() : 0;

        SetActiveIfExists(tutoInfoUi, true);
        SetActiveIfExists(tutoStart, true);

        // [CHANGED] 딕셔너리 초기화
        InitCardInfoMap();
        InitSkillButtonMap();

        if (cardManager == null)
            cardManager = FindObjectOfType<CardManager>();

        // [CHANGED] DayNight 이벤트 구독
        if (dayNight == null)
            dayNight = DayNightManager.Instance;

        if (dayNight != null)
        {
            dayNight.OnNightStarted += HandleNightStarted;
            dayNight.OnNightFinished += HandleNightFinished;
        }
    }

    private void OnDestroy()
    {
        if (dayNight != null)
        {
            dayNight.OnNightStarted -= HandleNightStarted;
            dayNight.OnNightFinished -= HandleNightFinished;
        }
    }

    private void Update()
    {
        var gd = DataController.instance.gameData;

        // 목표(금괴 10)
        if (gd.GoldIngotCard == 10)
            SetActiveIfExists(tutoEndUI, true);

        // 씬 이름으로 튜토 판정
        Scene scene = SceneManager.GetActiveScene();
        gd.tuto = (scene.name == "Tuto");

        // 퀘스트 1: 음식 3개 2초 유지
        if (gd.QusetNum == 1)
        {
            if (gd.FoodCount >= 3)
            {
                delayQuest += Time.deltaTime;
                if (delayQuest >= 2f)
                    gd.AddQuest(1);
            }
        }

        // 주민 0 → 실패
        if (gd.PlayerCount <= 0)
        {
            Fail();
            return;
        }

        if (Over) return;

        // Sell UI는 상태값 표시만
        SetActiveIfExists(SellUi, gd.Sell);

        // 낮: 클릭 UI 가능 / 밤: 기본은 막음(판매는 CardManager가 처리)
        if (!gd.endDay)
        {
            SetActiveIfExists(SellBtn, true);
            SetActiveIfExists(buyBtn, true);
            SetActiveIfExists(craftUiBtn, true);

            if (!gd.Skill)
                SetActiveIfExists(cardSkillUi, false);

            CardInfo();
            CardSkillUI();
        }
        else
        {
            // 밤에는 구매/제작 비활성(선택)
            SetActiveIfExists(buyBtn, false);
            SetActiveIfExists(craftUiBtn, false);
        }

        TutoInfoOff();
    }

    // [CHANGED] 밤 이벤트(시간 시스템은 DayNightManager가 담당, UI만 반응)
    private void HandleNightStarted()
    {
        var gd = DataController.instance.gameData;

        SetActiveIfExists(craftUi, false);
        SetActiveIfExists(runtimeRecipeUi, false);
        SetActiveIfExists(cardInfoUi, false);
        SetActiveIfExists(cardSkillUi, false);

        if (gd.tuto && !tutoday)
        {
            tutoday = true;
            Time.timeScale = 0f;
            SetActiveIfExists(tutoInfoUi, true);
            SetActiveIfExists(tutoDayUi, true);
            SetActiveIfExists(tutoDay, true);
        }
    }

    private void HandleNightFinished()
    {
        // 낮으로 돌아오면 Update에서 버튼 자동 복구
    }

    // ===== UI 버튼들 =====
    public void CraftUiBtn()
    {
        var gd = DataController.instance.gameData;

        if (gd.tuto && !tutocraft)
        {
            SetActiveIfExists(tutoInfoUi, true);
            SetActiveIfExists(tutoBtnUi, true);
            SetActiveIfExists(tutoCraft, true);
            if (craftUiBtn != null)
                craftUiBtn.transform.SetAsLastSibling();
            Time.timeScale = 0f;
            tutocraft = true;
        }

        SetActiveIfExists(craftUi, false);
        SetActiveIfExists(craftListUi, false);

        if (IsActive(runtimeRecipeUi))
        {
            SetActiveIfExists(runtimeRecipeUi, false);
            return;
        }

        ShowRuntimeRecipeUi();
        SetActiveIfExists(cardInfoUi, false);
    }

    public void CraftUiCloseBtn()
    {
        SetActiveIfExists(craftUi, false);
        SetActiveIfExists(runtimeRecipeUi, false);
        SetActiveIfExists(craftUiBtn, true);
        SetActiveIfExists(buyBtn, true);
    }

    public void CardSkillCloseBtn()
    {
        SetActiveIfExists(cardSkillUi, false);
    }

    public void MenuUiBtn()
    {
        SetActiveIfExists(menuUi, true);
        SetActiveIfExists(menuUiBtn, false);
    }

    public void MenuUiCloseBtn()
    {
        SetActiveIfExists(menuUi, false);
        SetActiveIfExists(menuUiBtn, true);
    }

    public void ErrorMessageClose()
    {
        SetActiveIfExists(ErrorMessage, false);
    }

    // ===== 공통 유틸 ==

    private bool TryRaycastCardUnderMouse(out GameObject hitObj)
    {
        hitObj = null;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return false;

        hitObj = hit.transform.gameObject;
        return hitObj != null;
    }

    private void HideAllSkillButtons()
    {
        if (_allSkillButtons == null) return;
        for (int i = 0; i < _allSkillButtons.Length; i++)
            SetActiveIfExists(_allSkillButtons[i], false);
    }

    // ===== CardInfo 데이터/표시 =====
    private void InitCardInfoMap()
    {
        _cardInfoMap.Clear();

        _cardInfoMap[CardId.Wood] = ("목재", "가장 기본재료 나무를 벌목해 얻는다. 여러가지 제작품에 재료로 사용가능하다.");
        _cardInfoMap[CardId.Stone] = ("석재", "가장 기본재료 암석을 채광해 얻는다. 여러가지 제작품에 재료로 사용가능하다.");
        _cardInfoMap[CardId.Tree] = ("나무", "벌목하면 목재를 얻을 수 있다. 지금은 아무것도 아니지");
        _cardInfoMap[CardId.Rock] = ("암석", "채광하면 석재를 얻을 수 있다. 지금은 너무 무겁지");
        _cardInfoMap[CardId.House] = ("집", "카드의 한도를 늘려준다. 플레이어의 수를 늘릴수있다.");

        _cardInfoMap[CardId.BananaTree] = ("바나나나무", "채집을 하면 바나나를 얻을 수 있다.");
        _cardInfoMap[CardId.Banana] = ("바나나", "기본음식 그냥도 먹을 수 있지만 요리해서 먹으면 더욱 배부르다.");

        _cardInfoMap[CardId.StrawBerryTree] = ("딸기나무", "채집을 하면 딸기를 얻을 수 있다.");
        _cardInfoMap[CardId.StrawBerry] = ("딸기", "기본음식 그냥도 먹을 수 있지만 요리해서 먹으면 더욱 배부르다.");

        _cardInfoMap[CardId.Brick] = ("벽돌", "돌을 가공해 만든 벽돌 튼튼하다.");
        _cardInfoMap[CardId.Panel] = ("판자", "목재를 가공해 만드는 판때기 집만들때 사용한다");
        _cardInfoMap[CardId.Branch] = ("나뭇가지", "목재를 손질해 얻은 나뭇가지\n화로의 연료로 사용한다.");

        _cardInfoMap[CardId.Forge] = ("용광로", "철과 금을 제련할 수 있다.");
        _cardInfoMap[CardId.Mine] = ("벽돌공장", "돌을 가공해 벽돌을 만드는 공장");
        _cardInfoMap[CardId.Timber] = ("제재소", "목재를 가공해 판자를 만드는 공장");

        _cardInfoMap[CardId.Gold] = ("금광석", "제련을 통해 빛나는 금괴로 만들 수 있다.");
        _cardInfoMap[CardId.GoldIngot] = ("금괴", "비싸게 팔리는 금괴 다른 역할은?");

        _cardInfoMap[CardId.Iron] = ("철광석", "제련을 통해 단단한 철괴를 만들 수 있다.");
        _cardInfoMap[CardId.IronIngot] = ("철괴", "많은 것을 만들 수 있는 기본이면서 최강의 제료");

        _cardInfoMap[CardId.Armory] = ("무기고", "무기와 전투 장비를 제작하고 보관하는 건물이다.");
        _cardInfoMap[CardId.Player] = ("주민", "주민이 없으면 게임은 끝나버린다. 배가 고프지");
        _cardInfoMap[CardId.WoodShield] = ("나무 방패", "나무로 만든 기본 방패다. 주민의 방어력을 올리는 장비로 사용한다.");
        _cardInfoMap[CardId.IronShield] = ("철 방패", "철로 만든 튼튼한 방패다. 더 높은 방어력을 제공하는 장비다.");
        _cardInfoMap[CardId.WoodSword] = ("목검", "나무로 만든 기본 무기다. 주민의 공격력을 올리는 장비로 사용한다.");
        _cardInfoMap[CardId.IronSword] = ("철검", "철로 만든 무기다. 목검보다 강한 공격력을 제공한다.");
    }

    private void CardInfo()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (DataController.instance.gameData.Sell) return;

        if (!TryRaycastCardUnderMouse(out GameObject touch))
        {
            SetActiveIfExists(cardInfoUi, false);
            return;
        }

        if (!touch.TryGetComponent<CardIdentity>(out var ident)) 
        {
            SetActiveIfExists(cardInfoUi, false);
            return;
        }

        if (_cardInfoMap.TryGetValue(ident.cardId, out var info))
        {
            SetActiveIfExists(cardInfoUi, true);
            if (CardNameText != null) CardNameText.text = info.displayName;
            if (CardInfoText != null) CardInfoText.text = info.desc;
        }
        else
        {
            SetActiveIfExists(cardInfoUi, false);
        }
    }

    // ===== Skill UI =====
    private void InitSkillButtonMap()
    {
        _skillButtonMap.Clear();

        _skillButtonMap[CardId.Tree] = treeSkillBtn;
        _skillButtonMap[CardId.Rock] = rockSkillBtn;
        _skillButtonMap[CardId.BananaTree] = bananaTreeBtn;
        _skillButtonMap[CardId.StrawBerryTree] = StrawBerryTreeBtn;
        _skillButtonMap[CardId.Wood] = WoodSkillBtn;

        _skillButtonMap[CardId.Timber] = TimberSkillBtn;
        _skillButtonMap[CardId.Mine] = MineSkillBtn;
        _skillButtonMap[CardId.House] = HouseSkillBtn;
        _skillButtonMap[CardId.Forge] = ForgeSkillBtn;

        _allSkillButtons = new[]
        {
            treeSkillBtn, rockSkillBtn, bananaTreeBtn, StrawBerryTreeBtn,
            WoodSkillBtn, TimberSkillBtn, MineSkillBtn, HouseSkillBtn, ForgeSkillBtn
        };

        HideAllSkillButtons();
    }

    private void CardSkillUI()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (DataController.instance.gameData.Sell) return;

        if (!TryRaycastCardUnderMouse(out GameObject touch)) return;

        if (!touch.TryGetComponent<CardIdentity>(out var ident)) return;

        if (!_skillButtonMap.TryGetValue(ident.cardId, out GameObject buttonToShow))
            return;

        DataController.instance.gameData.Skill = true;
        SetActiveIfExists(cardSkillUi, true);
        cardManager?.SetSelectedCard(touch);

        HideAllSkillButtons();
        SetActiveIfExists(buttonToShow, true);
    }

    // ===== 튜토 닫기 =====
    private void TutoInfoOff()
    {
        if (!IsActive(tutoInfoUi)) return;

        if (Input.GetMouseButton(0))
        {
            SetActiveIfExists(tutoInfoUi, false);
            SetActiveIfExists(tutoBuy, false);
            SetActiveIfExists(tutoCraft, false);
            SetActiveIfExists(tutoSell, false);
            SetActiveIfExists(tutoDay, false);
            SetActiveIfExists(tutoDayUi, false);
            SetActiveIfExists(tutoBtnUi, false);
            SetActiveIfExists(tutoStart, false);
            SetActiveIfExists(tutoStoreUp, false);
            if (craftUiBtn != null)
                craftUiBtn.transform.SetSiblingIndex(craftsibling);
            Time.timeScale = 1f;
        }
    }

    // ===== 상점 업그레이드 =====
    public void StoreUpgrade()
    {
        var gd = DataController.instance.gameData;

        if (gd.tuto && !StoreUp)
        {
            SetActiveIfExists(tutoInfoUi, true);
            SetActiveIfExists(tutoBtnUi, true);
            SetActiveIfExists(tutoStoreUp, true);
            Time.timeScale = 0f;
            StoreUp = true;
        }

        if (gd.gold >= 100 && gd.storeUpgrade == 0 && Time.timeScale != 0f)
        {
            if (gd.QusetNum == 5) gd.AddQuest(1);

            gd.storeUpgrade += 1;
            gd.AddGold(-100);
            SetActiveIfExists(StoreUpBtn, false);
        }
        else if (gd.gold < 100 && Time.timeScale != 0f)
        {
            SetActiveIfExists(ErrorMessage, true);
            if (ErrorMessageText != null)
                ErrorMessageText.text = "상점을 업그레이드\n하려면 100골드가 필요합니다!";
        }
    }

    // ===== 씬/게임오버 =====
    public void GameOver()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MainSecne()
    {
        SceneManager.LoadScene("Tuto");
    }

    public void StartBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void TutoBtn()
    {
        SceneManager.LoadScene("Tuto");
    }

    public void Fail()
    {
        SetActiveIfExists(tutoInfoUi, false);
        SetActiveIfExists(tutoBuy, false);
        SetActiveIfExists(tutoCraft, false);
        SetActiveIfExists(tutoSell, false);
        SetActiveIfExists(tutoDay, false);
        SetActiveIfExists(tutoDayUi, false);
        SetActiveIfExists(tutoBtnUi, false);
        SetActiveIfExists(tutoStart, false);
        SetActiveIfExists(tutoStoreUp, false);

        SetActiveIfExists(GameOverMessage, true);
        Over = true;
    }

    public void tutoEndBtn()
    {
        SetActiveIfExists(tutoEndUI, false);
    }

    private void ShowRuntimeRecipeUi()
    {
        if (runtimeRecipeUi == null)
            runtimeRecipeUi = BuildRuntimeRecipeUi();

        EnsureRuntimeRecipeUiCanvasParent();
        SetActiveIfExists(runtimeRecipeUi, true);
        if (runtimeRecipeUi != null)
            runtimeRecipeUi.transform.SetAsLastSibling();
    }

    private GameObject BuildRuntimeRecipeUi()
    {
        Canvas canvas = ResolveScreenUiCanvas();
        if (canvas == null)
            return null;

        GameObject root = CreateRuntimePanel(
            "RuntimeRecipeList",
            canvas.transform,
            new Vector2(360f, 1000f),
            new Vector2(-770f, -10f),
            new Color(0f, 0f, 0f, 0f));
        root.transform.SetAsLastSibling();

        GameObject listPanel = CreateRuntimePanel(
            "RecipeCraftList",
            root.transform,
            new Vector2(340f, 700f),
            new Vector2(0f, 125f),
            new Color(0.96f, 0.92f, 0.78f, 0.98f));

        GameObject detailPanel = CreateRuntimePanel(
            "RecipeDetail",
            root.transform,
            new Vector2(340f, 250f),
            new Vector2(0f, -370f),
            new Color(0.96f, 0.92f, 0.78f, 0.98f));

        TextMeshProUGUI titleText = CreateRuntimeText(
            "RecipeTitle",
            listPanel.transform,
            "레시피",
            30f,
            TextAlignmentOptions.Center,
            Color.black);
        SetStretch(titleText.rectTransform, 16f, 178f, 14f, 632f);

        TextMeshProUGUI tabText = CreateRuntimeText(
            "RecipeTabText",
            listPanel.transform,
            "제작물",
            30f,
            TextAlignmentOptions.Center,
            Color.black);
        SetStretch(tabText.rectTransform, 166f, 16f, 14f, 632f);

        CreateRecipeSectionHeader(listPanel.transform, "기본", 218f);
        CreateRecipeSectionHeader(listPanel.transform, "중요", 22f);

        TextMeshProUGUI detailText = CreateRuntimeText(
            "RecipeDetailText",
            detailPanel.transform,
            "",
            23f,
            TextAlignmentOptions.TopLeft,
            Color.black);
        detailText.enableWordWrapping = true;
        SetStretch(detailText.rectTransform, 22f, 22f, 18f, 18f);

        Button closeButton = CreateSmallCloseButton(root.transform);
        closeButton.onClick.AddListener(() => SetActiveIfExists(runtimeRecipeUi, false));

        for (int i = 0; i < RecipeEntries.Length; i++)
            CreateRecipeButton(listPanel.transform, RecipeEntries[i], i, detailText);

        ShowRecipeDetail(detailText, RecipeEntries[0]);

        return root;
    }

    private void EnsureRuntimeRecipeUiCanvasParent()
    {
        if (runtimeRecipeUi == null)
            return;

        Canvas canvas = ResolveScreenUiCanvas();
        if (canvas == null)
            return;

        if (runtimeRecipeUi.transform.parent != canvas.transform)
            runtimeRecipeUi.transform.SetParent(canvas.transform, false);
    }

    private static Canvas ResolveScreenUiCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        Canvas fallback = null;

        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || IsCardAttachedCanvas(canvas))
                continue;

            if (fallback == null)
                fallback = canvas;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                return canvas;
            }
        }

        return fallback;
    }

    private static bool IsCardAttachedCanvas(Canvas canvas)
    {
        Transform current = canvas.transform;
        while (current != null)
        {
            if (current.name == "WorkProgressCanvas" ||
                current.GetComponent<CardIdentity>() != null)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private GameObject CreateRuntimePanel(
        string name,
        Transform parent,
        Vector2 size,
        Vector2 anchoredPosition,
        Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        Image image = panel.GetComponent<Image>();
        image.color = color;

        if (color.a > 0f)
        {
            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(3f, -3f);
        }

        return panel;
    }

    private TextMeshProUGUI CreateRuntimeText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;

        TMP_FontAsset font = LoadCookieRunFont();
        if (font != null)
            text.font = font;

        return text;
    }

    private void CreateRecipeSectionHeader(Transform parent, string title, float y)
    {
        GameObject header = CreateRuntimePanel(
            title + "Header",
            parent,
            new Vector2(300f, 34f),
            new Vector2(0f, y),
            new Color(1f, 0.95f, 0.72f, 0.72f));

        TextMeshProUGUI label = CreateRuntimeText(
            "Label",
            header.transform,
            title + "                      -",
            22f,
            TextAlignmentOptions.MidlineLeft,
            Color.black);
        SetStretch(label.rectTransform, 8f, 8f, 4f, 4f);
    }

    private Button CreateSmallCloseButton(Transform parent)
    {
        GameObject buttonObject = new GameObject(
            "RecipeCloseButton",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(34f, 34f);
        rect.anchoredPosition = new Vector2(-8f, -8f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 0.98f, 0.9f, 1f);

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(3f, -3f);

        TextMeshProUGUI label = CreateRuntimeText(
            "Label",
            buttonObject.transform,
            "<",
            26f,
            TextAlignmentOptions.Center,
            Color.black);
        SetStretch(label.rectTransform, 0f, 0f, 0f, 0f);

        return buttonObject.GetComponent<Button>();
    }

    private void CreateRecipeButton(
        Transform parent,
        RecipeEntry recipe,
        int index,
        TextMeshProUGUI detailText)
    {
        GameObject buttonObject = new GameObject(
            recipe.Title + "Button",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(EventTrigger));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(300f, 28f);
        rect.anchoredPosition = GetRecipeButtonPosition(index);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 0.98f, 0.88f, 0.08f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => ShowRecipeDetail(detailText, recipe));

        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, () => ShowRecipeDetail(detailText, recipe));

        TextMeshProUGUI label = CreateRuntimeText(
            "Label",
            buttonObject.transform,
            "• " + recipe.Title,
            21f,
            TextAlignmentOptions.MidlineLeft,
            Color.black);
        SetStretch(label.rectTransform, 8f, 8f, 0f, 0f);
    }

    private static Vector2 GetRecipeButtonPosition(int index)
    {
        if (index < 5)
            return new Vector2(0f, -152f - (index * 32f));

        return new Vector2(0f, -360f - ((index - 5) * 32f));
    }

    private static void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    private static void ShowRecipeDetail(TextMeshProUGUI detailText, RecipeEntry recipe)
    {
        detailText.text =
            recipe.Title + "\n" +
            "----------------\n" +
            recipe.Requirements + "\n\n" +
            "결과: " + recipe.Result + "\n" +
            "시간: " + recipe.Duration;
    }

    private static void SetStretch(RectTransform rect, float left, float right, float top, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private TMP_FontAsset LoadCookieRunFont()
    {
        if (cookieRunFont != null)
            return cookieRunFont;

        cookieRunFont = Resources.Load<TMP_FontAsset>(CookieRunFontResourcesPath);

#if UNITY_EDITOR
        if (cookieRunFont == null)
            cookieRunFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(CookieRunFontAssetPath);
#endif

        return cookieRunFont;
    }

    private static void SetActiveIfExists(GameObject target, bool active)
    {
        if (target != null)
            target.SetActive(active);
    }

    private static bool IsActive(GameObject target)
    {
        return target != null && target.activeSelf;
    }

    private struct RecipeEntry
    {
        public string Title { get; }
        public string Requirements { get; }
        public string Result { get; }
        public string Duration { get; }

        public RecipeEntry(string title, string requirements, string result, string duration)
        {
            Title = title;
            Requirements = requirements;
            Result = result;
            Duration = duration;
        }
    }

    private static readonly RecipeEntry[] RecipeEntries =
    {
        new RecipeEntry("목재", "주민 + 나무", "목재 2개", "4초"),
        new RecipeEntry("석재", "주민 + 바위", "석재 2개", "4초"),
        new RecipeEntry("나뭇가지", "주민 + 목재", "나뭇가지 3개", "6초"),
        new RecipeEntry("바나나", "주민 + 바나나나무", "바나나 3개", "6초"),
        new RecipeEntry("딸기", "주민 + 딸기나무", "딸기 3개", "6초"),
        new RecipeEntry("광산", "주민 + 목재 1 + 석재 3", "광산", "15초"),
        new RecipeEntry("제재소", "주민 + 목재 3 + 석재 1", "제재소", "15초"),
        new RecipeEntry("용광로", "주민 + 나뭇가지 1 + 벽돌 2", "용광로", "30초"),
        new RecipeEntry("집", "주민 + 판자 3 + 벽돌 3", "집", "60초"),
        new RecipeEntry("무기고", "주민 + 벽돌 2 + 판자 2 + 철괴 1", "무기고", "45초"),
        new RecipeEntry("판자", "주민 + 제재소 + 목재 2 + 나뭇가지 1\n골드 1 이상 필요", "판자", "5초"),
        new RecipeEntry("벽돌", "주민 + 광산 + 석재 2\n골드 1 이상 필요", "벽돌", "5초"),
        new RecipeEntry("철괴", "주민 + 용광로 + 목재 2 + 나뭇가지 2 + 철광석 1", "철괴", "5초"),
        new RecipeEntry("금괴", "주민 + 용광로 + 목재 2 + 나뭇가지 1 + 금광석 1", "금괴", "5초"),
        new RecipeEntry("주민", "주민 2 + 집\n골드 16 이상 필요, 15 소모", "주민 1명", "60초")
    };
}
