using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Slider slTimer;
    [SerializeField] private DayNightManager dayNight;

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
    [SerializeField] private TextMeshProUGUI QuestNumText; // 필요하면 사용

    [Header("Tutorial Texts")]
    [SerializeField] private TextMeshProUGUI tutoBuyText;
    [SerializeField] private TextMeshProUGUI tutoSellText;
    [SerializeField] private TextMeshProUGUI tutoCraftText;
    [SerializeField] private TextMeshProUGUI tutoDayText;
    [SerializeField] private TextMeshProUGUI tutoStoreUpText;

    [Header("Other Static Texts")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI startText;

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

        SetStaticTexts();

        var gd = DataController.instance.gameData;
        gd?.RaiseAllEvnets();
    }

    private void Update()
    {
        // 타이머는 이벤트로 매 프레임 쏘기엔 과해서 Update로 유지 (예외 OK)
        if (dayNight != null && slTimer != null)
            slTimer.value = dayNight.TimeLeft;
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

        // 일꾼 표기도 플레이어 수가 필요
        var gd = DataController.instance.gameData;
        if (gd != null && WokerCountText) WokerCountText.text = $"일꾼 : {gd.Woker} / {gd.PlayerCount}";
    }

    private void UpdateWorker(int value)
    {
        var gd = DataController.instance.gameData;
        if (gd != null && WokerCountText) WokerCountText.text = $"일꾼 : {gd.Woker} / {gd.PlayerCount}";
    }

    private void UpdateCardCount(int limit, int count)
    {
        if (CardCountText) CardCountText.text = $"카드제한 : {limit}/{count}";
    }

    private void UpdateDay(int value)
    {
        if (DayText) DayText.text = "생존일 : " + value;
    }

    private void UpdateInventory()
    {
        var gd = DataController.instance.gameData;
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

        // Goal도 인벤토리 변화에 따라 같이 갱신(금괴)
        if (GoalText) GoalText.text = "목표 : 금괴 10 / " + gd.GoldIngotCard;
    }

    private void UpdateQuest(int questNum)
    {
        if (QuestText) QuestText.text = GetQuestText(questNum);
        if (QuestNumText) QuestNumText.text = questNum.ToString();
    }

    private void SetStaticTexts()
    {
        if (tutoBuyText) tutoBuyText.text = "3골드로 카드를 구매 가능합니다.";
        if (tutoCraftText) tutoCraftText.text = "재료를 모아 제작이 가능합니다..";
        if (tutoDayText)
        {
            tutoDayText.text =
                "밤이 되면 제한된 카드에 맞춰 카드 판매가 필요합니다.\n" +
                "또한 주민은 음식이 필요하며\n" +
                "음식이 부족한 만큼 주민이 죽습니다.";
        }
        if (tutoSellText)
        {
            tutoSellText.text =
                "상단 판매가 활성화 되어있으면\n" +
                "카드를 클릭해\n" +
                "팔 수 있습니다";
        }
        if (tutoStoreUpText) tutoStoreUpText.text = "100골드로 상점을 업그레이드 할수있습니다. 새로운 재료가 나옵니다.";

        if (gameOverText) gameOverText.text = "게임오버!\n모든 주민이\n음식이 없어 굶어 죽었습니다.";
        if (startText) startText.text = "목표를 달성했습니다.\n이제 본게임으로";
    }

    private static string GetQuestText(int questNum)
    {
        return questNum switch
        {
            0 => "구매 버튼을 눌러 카드를 구매하세요",
            1 => "목재카드 선택 후 나뭇가지를 만드세요",
            2 => "나뭇가지를 판매하고 바나나를 채집하세요",
            3 => "제작에서 제제소 또는 벽돌공장을 만드세요",
            4 => "벽돌 또는 판자를 만드세요",
            5 => "100골드를 모아 상점을 업그레이드 하세요",
            6 => "화로를 만들어 금괴를 만드세요",
            7 => "퀘스트 완료! 목적을 달성하세요",
            _ => ""
        };
    }
}