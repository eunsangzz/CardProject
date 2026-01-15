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
        if (_nightRoutineRunning) return;

        var gd = DataController.instance.gameData;
        if (gd == null) return;

        if (gd.endDay) return;

        TimeLeft -= Time.deltaTime;
        if(TimeLeft <= 0f)
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
        gd.Day += 1;

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

        gd.Woker = Mathf.Min(gd.Woker, gd.PlayerCount);

        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();

    }

    private void FinishNight()
    {
        _nightRoutineRunning = false;
        OnNightFinished?.Invoke();
    }

}
