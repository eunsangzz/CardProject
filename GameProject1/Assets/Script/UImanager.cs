using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
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

    // [CHANGED] CardInfo/Skill 딕셔너리
    private readonly Dictionary<CardId, (string displayName, string desc)> _cardInfoMap
        = new Dictionary<CardId, (string, string)>();

    private readonly Dictionary<CardId, GameObject> _skillButtonMap
        = new Dictionary<CardId, GameObject>();

    private GameObject[] _allSkillButtons;

    private void Start()
    {
        cardInfoUi.SetActive(false);

        tutoday = false;
        tutocraft = false;

        // 시작 튜토: 일시정지
        Time.timeScale = 0f;
        craftsibling = craftUiBtn.transform.GetSiblingIndex();

        tutoInfoUi.SetActive(true);
        tutoStart.SetActive(true);

        // [CHANGED] 딕셔너리 초기화
        InitCardInfoMap();
        InitSkillButtonMap();

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
            tutoEndUI.SetActive(true);

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
        SellUi.SetActive(gd.Sell);

        // 낮: 클릭 UI 가능 / 밤: 기본은 막음(판매는 CardManager가 처리)
        if (!gd.endDay)
        {
            if (craftUi.activeSelf)
            {
                SellBtn.SetActive(false);
                buyBtn.SetActive(false);
                craftUiBtn.SetActive(false);
            }
            else
            {
                SellBtn.SetActive(true);
                buyBtn.SetActive(true);
                craftUiBtn.SetActive(true);
            }

            if (!gd.Skill)
                cardSkillUi.SetActive(false);

            CardInfo();
            CardSkillUI();
        }
        else
        {
            // 밤에는 구매/제작 비활성(선택)
            buyBtn.SetActive(false);
            craftUiBtn.SetActive(false);
        }

        TutoInfoOff();
    }

    // [CHANGED] 밤 이벤트(시간 시스템은 DayNightManager가 담당, UI만 반응)
    private void HandleNightStarted()
    {
        var gd = DataController.instance.gameData;

        craftUi.SetActive(false);
        cardInfoUi.SetActive(false);
        cardSkillUi.SetActive(false);

        if (gd.tuto && !tutoday)
        {
            tutoday = true;
            Time.timeScale = 0f;
            tutoInfoUi.SetActive(true);
            tutoDayUi.SetActive(true);
            tutoDay.SetActive(true);
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
            tutoInfoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoCraft.SetActive(true);
            craftUiBtn.transform.SetAsLastSibling();
            Time.timeScale = 0f;
            tutocraft = true;
        }

        craftUi.SetActive(true);
        craftListUi.SetActive(true);
        buyBtn.SetActive(false);
        SellBtn.SetActive(false);
        cardInfoUi.SetActive(false);
    }

    public void CraftUiCloseBtn()
    {
        craftUi.SetActive(false);
        craftUiBtn.SetActive(true);
        buyBtn.SetActive(true);
    }

    public void CardSkillCloseBtn()
    {
        cardSkillUi.SetActive(false);
    }

    public void MenuUiBtn()
    {
        menuUi.SetActive(true);
        menuUiBtn.SetActive(false);
    }

    public void MenuUiCloseBtn()
    {
        menuUi.SetActive(false);
        menuUiBtn.SetActive(true);
    }

    public void ErrorMessageClose()
    {
        ErrorMessage.SetActive(false);
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
            _allSkillButtons[i].SetActive(false);
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

        _cardInfoMap[CardId.Player] = ("주민", "주민이 없으면 게임은 끝나버린다. 배가 고프지");
    }

    private void CardInfo()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (DataController.instance.gameData.Sell) return;

        if (!TryRaycastCardUnderMouse(out GameObject touch))
        {
            cardInfoUi.SetActive(false);
            return;
        }

        if (!touch.TryGetComponent<CardIdentity>(out var ident)) 
        {
            cardInfoUi.SetActive(false);
            return;
        }

        if (_cardInfoMap.TryGetValue(ident.cardId, out var info))
        {
            cardInfoUi.SetActive(true);
            CardNameText.text = info.displayName;
            CardInfoText.text = info.desc;
        }
        else
        {
            cardInfoUi.SetActive(false);
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
        cardSkillUi.SetActive(true);

        HideAllSkillButtons();
        buttonToShow.SetActive(true);
    }

    // ===== 튜토 닫기 =====
    private void TutoInfoOff()
    {
        if (!tutoInfoUi.activeSelf) return;

        if (Input.GetMouseButton(0))
        {
            tutoInfoUi.SetActive(false);
            tutoBuy.SetActive(false);
            tutoCraft.SetActive(false);
            tutoSell.SetActive(false);
            tutoDay.SetActive(false);
            tutoDayUi.SetActive(false);
            tutoBtnUi.SetActive(false);
            tutoStart.SetActive(false);
            tutoStoreUp.SetActive(false);
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
            tutoInfoUi.SetActive(true);
            tutoBtnUi.SetActive(true);
            tutoStoreUp.SetActive(true);
            Time.timeScale = 0f;
            StoreUp = true;
        }

        if (gd.gold >= 100 && gd.storeUpgrade == 0 && Time.timeScale != 0f)
        {
            if (gd.QusetNum == 5) gd.AddQuest(1);

            gd.storeUpgrade += 1;
            gd.AddGold(-100);
            StoreUpBtn.SetActive(false);
        }
        else if (gd.gold < 100 && Time.timeScale != 0f)
        {
            ErrorMessage.SetActive(true);
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
        tutoInfoUi.SetActive(false);
        tutoBuy.SetActive(false);
        tutoCraft.SetActive(false);
        tutoSell.SetActive(false);
        tutoDay.SetActive(false);
        tutoDayUi.SetActive(false);
        tutoBtnUi.SetActive(false);
        tutoStart.SetActive(false);
        tutoStoreUp.SetActive(false);

        GameOverMessage.SetActive(true);
        Over = true;
    }

    public void tutoEndBtn()
    {
        tutoEndUI.SetActive(false);
    }
}