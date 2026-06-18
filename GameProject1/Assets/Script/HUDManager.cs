using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HUDManager : MonoBehaviour
{
    private const string CookieRunFontResourcesPath = "Fonts/Regular SDF";
    private const string CookieRunFontAssetPath = "Assets/CookieRunFont_TTF/Regular SDF.asset";
    private const float HudLayoutScale = 1f;

    [Header("Timer")]
    [SerializeField] private Slider slTimer;
    [SerializeField] private DayNightManager dayNight;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TMP_FontAsset cookieRunFont;

    [Header("Resource Texts")]
    [SerializeField] private TextMeshProUGUI WoodCountText;
    [SerializeField] private TextMeshProUGUI StoneCountText;
    [SerializeField] private TextMeshProUGUI IronCountText;
    [SerializeField] private TextMeshProUGUI GoldCountText;
    [SerializeField] private TextMeshProUGUI PanelCountText;
    [SerializeField] private TextMeshProUGUI BrickCountText;
    [SerializeField] private TextMeshProUGUI IronIngotCountText;
    [SerializeField] private TextMeshProUGUI GoldIngotCountText;
    [SerializeField] private TextMeshProUGUI BranchCountText;

    [Header("Top HUD Texts")]
    [SerializeField] private TextMeshProUGUI GoldText;
    [SerializeField] private TextMeshProUGUI FoodCountText;
    [SerializeField] private TextMeshProUGUI WokerCountText;
    [SerializeField] private TextMeshProUGUI CardCountText;
    [SerializeField] private TextMeshProUGUI DayText;
    [SerializeField] private TextMeshProUGUI GoalText;

    [Header("Quest Texts")]
    [SerializeField] private TextMeshProUGUI QuestText;
    [SerializeField] private TextMeshProUGUI QuestNumText;

    [Header("Tutorial Texts")]
    [SerializeField] private TextMeshProUGUI tutoBuyText;
    [SerializeField] private TextMeshProUGUI tutoSellText;
    [SerializeField] private TextMeshProUGUI tutoCraftText;
    [SerializeField] private TextMeshProUGUI tutoDayText;
    [SerializeField] private TextMeshProUGUI tutoStoreUpText;

    [Header("Other Static Texts")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI startText;

    private bool _timerLayoutAdjusted;
    private int _cachedPlayer;
    private int _cachedFood;

    private void Start()
    {
        if (dayNight == null)
            dayNight = DayNightManager.Instance;

        if (dayNight != null && slTimer != null)
        {
            slTimer.maxValue = dayNight.DayDuration;
            slTimer.value = dayNight.TimeLeft;
        }

        ApplyCookieRunFontToTexts();
        ConfigureTimerText();
        SetStaticTexts();

        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;
        gd?.RaiseAllEvents();
    }

    private void Update()
    {
        if (dayNight != null && slTimer != null)
        {
            slTimer.value = dayNight.TimeLeft;
            UpdateTimerText();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGoldChanged += UpdateGold;
        GameEvents.OnFoodChanged += UpdateFood;
        GameEvents.OnPlayerChanged += UpdatePlayer;
        GameEvents.OnCardCountChanged += UpdateCardCount;
        GameEvents.OnDayChanged += UpdateDay;
        GameEvents.OnWorkerChanged += UpdateWorker;
        GameEvents.OnInventoryChanged += UpdateInventory;
        GameEvents.OnQuestChanged += UpdateQuest;
    }

    private void OnDisable()
    {
        GameEvents.OnGoldChanged -= UpdateGold;
        GameEvents.OnFoodChanged -= UpdateFood;
        GameEvents.OnPlayerChanged -= UpdatePlayer;
        GameEvents.OnCardCountChanged -= UpdateCardCount;
        GameEvents.OnDayChanged -= UpdateDay;
        GameEvents.OnWorkerChanged -= UpdateWorker;
        GameEvents.OnInventoryChanged -= UpdateInventory;
        GameEvents.OnQuestChanged -= UpdateQuest;
    }

    private void UpdateGold(int value)
    {
        if (GoldText) GoldText.text = "골드 : " + value;
    }

    private void UpdateFood(int value)
    {
        _cachedFood = value;
        if (FoodCountText) FoodCountText.text = $"음식 : {_cachedFood}/{_cachedPlayer}";
    }

    private void UpdatePlayer(int value)
    {
        _cachedPlayer = value;
        if (FoodCountText) FoodCountText.text = $"음식 : {_cachedFood}/{_cachedPlayer}";

        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;
        if (gd != null && WokerCountText) WokerCountText.text = $"일꾼 : {gd.Woker} / {gd.PlayerCount}";
    }

    private void UpdateWorker(int value)
    {
        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;
        if (gd != null && WokerCountText) WokerCountText.text = $"일꾼 : {gd.Woker} / {gd.PlayerCount}";
    }

    private void UpdateCardCount(int limit, int count)
    {
        if (CardCountText) CardCountText.text = $"카드 : {count}/{limit}";
    }

    private void UpdateDay(int value)
    {
        if (DayText) DayText.text = "생존일 : " + value;
    }

    private void UpdateInventory()
    {
        GameData gd = DataController.instance != null
            ? DataController.instance.gameData
            : null;
        if (gd == null) return;

        if (WoodCountText) WoodCountText.text = "목재 : " + gd.WoodCard;
        if (StoneCountText) StoneCountText.text = "석재 : " + gd.StoneCard;
        if (IronCountText) IronCountText.text = "철광석 : " + gd.IronCard;
        if (GoldCountText) GoldCountText.text = "금광석 : " + gd.GoldCard;

        if (GoldIngotCountText) GoldIngotCountText.text = "금괴 : " + gd.GoldIngotCard;
        if (IronIngotCountText) IronIngotCountText.text = "철괴 : " + gd.IronIngotCard;
        if (BrickCountText) BrickCountText.text = "벽돌 : " + gd.BrickCard;
        if (PanelCountText) PanelCountText.text = "판자 : " + gd.PanelCard;
        if (BranchCountText) BranchCountText.text = "나뭇가지 : " + gd.BranchCard;

        if (GoalText) GoalText.text = "목표 : 금괴 10 / " + gd.GoldIngotCard;
    }

    private void UpdateQuest(int questNum)
    {
        if (QuestText) QuestText.text = GetQuestText(questNum);
        if (QuestNumText) QuestNumText.text = questNum.ToString();
    }

    private void SetStaticTexts()
    {
        if (tutoBuyText) tutoBuyText.text = "3골드로 카드를 구매합니다.";
        if (tutoCraftText) tutoCraftText.text = "재료를 모아 제작을 시작합니다.";
        if (tutoDayText)
        {
            tutoDayText.text =
                "밤이 되면 제한된 카드 수에 맞춰 카드 판매가 필요합니다.\n" +
                "밤마다 주민에게 음식이 필요하며\n" +
                "음식이 부족한 만큼 주민이 죽습니다.";
        }
        if (tutoSellText)
        {
            tutoSellText.text =
                "카드 판매가 활성화되었을 때\n" +
                "카드를 클릭해\n" +
                "팔 수 있습니다.";
        }
        if (tutoStoreUpText) tutoStoreUpText.text = "100골드로 상점을 업그레이드할 수 있습니다. 새로운 재료가 나옵니다.";

        if (gameOverText) gameOverText.text = "게임오버!\n모든 주민이\n음식 부족으로 죽었습니다.";
        if (startText) startText.text = "목표를 달성했습니다.\n게임 클리어!";
    }

    private static string GetQuestText(int questNum)
    {
        return questNum switch
        {
            0 => "구매 버튼을 눌러 카드를 구매하세요.",
            1 => "나무 카드 위에 주민을 올려 목재를 얻으세요.",
            2 => "목재를 판매하고 바나나를 채집하세요.",
            3 => "제작으로 광산 또는 제재소를 만드세요.",
            4 => "광산 또는 제재소를 만드세요.",
            5 => "100골드를 모아 상점을 업그레이드하세요.",
            6 => "화로를 만들어 금괴를 만드세요.",
            7 => "퀘스트 완료! 목표를 달성하세요.",
            _ => ""
        };
    }

    private void ConfigureTimerText()
    {
        if (slTimer == null)
            return;

        RectTransform sliderRect = slTimer.GetComponent<RectTransform>();
        if (sliderRect != null && !_timerLayoutAdjusted)
        {
            sliderRect.localScale = Vector3.one;
            sliderRect.sizeDelta = Scaled(new Vector2(440f, 42f));
            _timerLayoutAdjusted = true;
        }

        if (timerText == null)
        {
            Transform old = slTimer.transform.Find("RuntimeTimerText");
            if (old != null)
                timerText = old.GetComponent<TextMeshProUGUI>();
        }

        if (timerText == null)
        {
            GameObject textObject = new GameObject("RuntimeTimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(slTimer.transform, false);
            timerText = textObject.GetComponent<TextMeshProUGUI>();
        }

        RectTransform textRect = timerText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        timerText.alignment = TextAlignmentOptions.Center;
        timerText.fontSize = Scaled(18f);
        timerText.fontStyle = FontStyles.Bold;
        timerText.color = Color.white;
        timerText.raycastTarget = false;
        ApplyCookieRunFont(timerText);
        timerText.transform.SetAsLastSibling();
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null || dayNight == null)
            return;

        int seconds = Mathf.CeilToInt(dayNight.TimeLeft);
        timerText.text = $"남은 시간 {seconds}s";
    }

    private void ApplyCookieRunFontToTexts()
    {
        TextMeshProUGUI[] texts =
        {
            WoodCountText, StoneCountText, IronCountText, GoldCountText,
            PanelCountText, BrickCountText, IronIngotCountText, GoldIngotCountText,
            BranchCountText, GoldText, FoodCountText, WokerCountText, CardCountText,
            DayText, GoalText, QuestText, QuestNumText, tutoBuyText, tutoSellText,
            tutoCraftText, tutoDayText, tutoStoreUpText, gameOverText, startText,
            timerText
        };

        for (int i = 0; i < texts.Length; i++)
            ApplyCookieRunFont(texts[i]);
    }

    private void ApplyCookieRunFont(TextMeshProUGUI text)
    {
        if (text == null)
            return;

        TMP_FontAsset fontAsset = LoadCookieRunFont();
        if (fontAsset != null)
            text.font = fontAsset;
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

    private static Vector2 Scaled(Vector2 value)
    {
        return value * HudLayoutScale;
    }

    private static float Scaled(float value)
    {
        return value * HudLayoutScale;
    }
}
