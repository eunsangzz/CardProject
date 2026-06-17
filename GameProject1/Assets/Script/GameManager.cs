using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Dev")]
    [SerializeField] private bool resetOnStart = true; //세이브 적용시 fasle

    private GameData _gd;

    private void Awake()
    {
        if (Instance != null && Instance != this) //싱글톤
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _gd = DataController.instance.gameData;
        _gd.EnsureRuntimeDefaults();

        if (resetOnStart)
            resetForNewGame();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RecalculateTotals();
    }

    private void LoadMainSecne()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void resetForNewGame()
    {
        _gd.storeUpgrade = 0;

        _gd.WoodCard = 0;
        _gd.StoneCard = 0;
        _gd.TreeCard = 0;
        _gd.RockCard = 0;

        _gd.BananaTreeCard = 0;
        _gd.BananaCard = 0;

        _gd.StrawBerryTreeCard = 0;
        _gd.StrawBerryCard = 0;

        _gd.BranchCard = 0;
        _gd.BrickCard = 0;
        _gd.PanelCard = 0;

        _gd.IronCard = 0;
        _gd.IronIngotCard = 0;

        _gd.GoldCard = 0;
        _gd.GoldIngotCard = 0;

        _gd.ForgeCard = 0;
        _gd.TimberCard = 0;
        _gd.MineCard = 0;
        _gd.KitchenCard = 0;
        _gd.ArmoryCard = 0;

        _gd.HouseCard = 0;

        // 게임 진행/경제
        _gd.SetGold(10);
        _gd.SetCardLimit(15);
        _gd.SetCardCount(0);

        _gd.SetPlayer(1);
        _gd.EnemyCount = 0;
        _gd.SetDay(0);
        _gd.Stage = 1;

        _gd.BossStage = false;
        _gd.Boss1Hp = 45;
        _gd.Boss2Hp = 70;

        _gd.SetWorker(1);
        _gd.SetQuest(0);

        // 상태 플래그
        _gd.Sell = false;
        _gd.Skill = false;
        _gd.endDay = false;
        _gd.Fight = false;
        _gd.FirstNightEnemySpawned = false;

        // 런타임 카드 리스트는 “비우기”만 하고 새 리스트로 덮어쓰지 않음
        _gd.EnsureRuntimeDefaults();
        _gd.Card.Clear();

        RecalculateTotals();
    }

    private static int ComputeFoodCount(GameData gd)
    {
        return gd.BananaCard + gd.StrawBerryCard;
    }

    public void RecalculateTotals()
    {
        _gd.SetFood(_gd.BananaCard + _gd.StrawBerryCard);

        int total =
            _gd.WoodCard +
            _gd.StoneCard +
            _gd.TreeCard +
            _gd.RockCard +
            _gd.BananaTreeCard +
            _gd.BananaCard +
            _gd.StrawBerryTreeCard +
            _gd.StrawBerryCard +
            _gd.IronCard +
            _gd.GoldCard +
            _gd.BranchCard +
            _gd.IronIngotCard +
            _gd.GoldIngotCard +
            _gd.BrickCard +
            _gd.PanelCard +
            _gd.HouseCard +
            _gd.ForgeCard +
            _gd.TimberCard +
            _gd.MineCard +
            _gd.KitchenCard +
            _gd.ArmoryCard;

        _gd.SetCardCount(total);
    }
}
