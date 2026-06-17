using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("Day Setting")]
    [SerializeField] private float dayDuration = 120f;

    [Header("Dependencies")]
    [SerializeField] private CardManager cardManager;
    [SerializeField] private GameObject sellUi;

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyParent;

    public float TimeLeft { get; private set; }
    public float DayDuration => dayDuration;
    public bool IsNightRoutineRunning => _nightRoutineRunning;

    public event Action OnNightStarted;
    public event Action OnNightFinished;
    public event Action OnNeedSellToLimit;
    public event Action OnAfterFeed;

    private bool _nightRoutineRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (cardManager == null)
            cardManager = FindObjectOfType<CardManager>();
    }

    private void Start()
    {
        RestDayTimer();
    }

    private void Update()
    {
        if (_nightRoutineRunning) return;

        var gd = DataController.instance.gameData;
        if (gd == null) return;

        if (gd.endDay) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            StartNight();
        }
    }

    public void RestDayTimer()
    {
        TimeLeft = dayDuration;
    }



    public void StartNight()
    {
        if (_nightRoutineRunning) return;

        var gd = DataController.instance.gameData;
        if (gd == null) return;

        gd.endDay = true;
        _nightRoutineRunning = true;

        OnNightStarted?.Invoke();
        StartCoroutine(NightRoutine());
    }

    private IEnumerator NightRoutine()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        if(gd.PlayerCount <= 0)
        {
            FinishNight();
            yield break;
        }

        if (gd.CardCount > gd.CardLimit)
        {
            gd.Sell = true;
            if (sellUi != null) sellUi.SetActive(true);

            OnNeedSellToLimit?.Invoke();

            while (gd.CardCount > gd.CardLimit)
                yield return null;

            gd.Sell = false;
            if (sellUi != null) sellUi.SetActive(false);
        }

        yield return StartCoroutine(FeedPlayerRoutine());

        OnAfterFeed?.Invoke();

        if (gd.PlayerCount <= 0) 
        {
            FinishNight();
            yield break;
        }

        gd.endDay = false;
        gd.NextDay();

        SpawnFirstNightEnemyIfNeeded(gd);

        RestDayTimer();

        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();

        FinishNight();
    }

    private IEnumerator FeedPlayerRoutine()
    {
        var gd = DataController.instance.gameData;
        if (cardManager == null)
            cardManager = FindObjectOfType<CardManager>();

        // 안전장치
        if (cardManager == null)
            yield break;

        int need = gd.PlayerCount;
        int have = gd.FoodCount;

        int willFeed = Mathf.Min(need, have);
        int willDie = need - willFeed;

        for (int i = 0; i < willFeed; i++)
        {
            if (gd.BananaCard > 0) cardManager.removeCard(5);
            else if (gd.StrawBerryCard > 0) cardManager.removeCard(6);
            else break;

            yield return null;
        }

        for (int i = 0; i < willDie; i++)
        {
            if (gd.PlayerCount <= 0) break;
            cardManager.removeCard(20);
            yield return null;
        }

        gd.SetWorker(Mathf.Min(gd.Woker, gd.PlayerCount));

        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();

    }

    private void FinishNight()
    {
        _nightRoutineRunning = false;
        OnNightFinished?.Invoke();
    }
    private void SpawnFirstNightEnemyIfNeeded(GameData gd)
    {
        if (gd == null || gd.FirstNightEnemySpawned || gd.Day < 1)
            return;

        SpawnEnemy();
        gd.FirstNightEnemySpawned = true;
        gd.EnemyCount += 1;
    }

    private void SpawnEnemy()
    {
        var gd = DataController.instance.gameData;
        gd.EnsureRuntimeDefaults();

        Vector3 spawnPosition =
            CardSpawnPositionFinder.FindAvailablePosition(gd.Card) +
            new Vector3(0f, 1.25f, 0f);

        GameObject enemy = enemyPrefab != null
            ? Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyParent)
            : CreateDefaultEnemy(spawnPosition);

        enemy.name = "Enemy";

        if (enemy.GetComponent<EnemyCombatStats>() == null)
            enemy.AddComponent<EnemyCombatStats>();

        if (enemy.GetComponent<EnemyAI>() == null)
            enemy.AddComponent<EnemyAI>();
    }

    private GameObject CreateDefaultEnemy(Vector3 position)
    {
        var enemy = new GameObject("Enemy");
        enemy.transform.SetParent(enemyParent, false);
        enemy.transform.position = position;

        var renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSolidSprite(new Color(0.75f, 0.12f, 0.12f, 1f));
        renderer.sortingOrder = 50;

        enemy.AddComponent<BoxCollider>();
        enemy.transform.localScale = new Vector3(0.9f, 1.2f, 1f);

        var labelObject = new GameObject("EnemyLabel");
        labelObject.transform.SetParent(enemy.transform, false);
        labelObject.transform.localPosition = new Vector3(0f, -0.75f, -0.05f);

        var label = labelObject.AddComponent<TMPro.TextMeshPro>();
        label.text = "Enemy";
        label.fontSize = 2.6f;
        label.alignment = TMPro.TextAlignmentOptions.Center;
        label.color = Color.white;
        label.rectTransform.sizeDelta = new Vector2(3f, 0.7f);
        label.GetComponent<Renderer>().sortingOrder = 60;

        return enemy;
    }

    private static Sprite CreateSolidSprite(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f);
    }

}
