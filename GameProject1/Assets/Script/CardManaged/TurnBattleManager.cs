using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnBattleManager : MonoBehaviour
{
    private const float ResidentHitChance = 0.8f;
    private const float EnemyHitChance = 0.6f;

    private static TurnBattleManager _instance;

    [SerializeField] private float cardSpacing = 1.65f;
    [SerializeField] private float rowDistance = 1.9f;
    [SerializeField] private float attackMoveDuration = 0.18f;
    [SerializeField] private float knockbackDistance = 0.25f;
    [SerializeField] private float turnDelay = 0.35f;
    [SerializeField] private float damagePopupDuration = 0.75f;
    [SerializeField] private float damagePopupRiseDistance = 0.4f;

    public static bool IsBattleRunning { get; private set; }

    public static TurnBattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TurnBattleManager>();
                if (_instance == null)
                    _instance = new GameObject("TurnBattleManager")
                        .AddComponent<TurnBattleManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void StartBattle()
    {
        if (IsBattleRunning)
            return;

        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        IsBattleRunning = true;

        List<ResidentCombatStats> residents = GetLivingResidents();
        List<EnemyCombatStats> enemies = GetLivingEnemies();

        if (residents.Count == 0 || enemies.Count == 0)
        {
            EndBattle(enemies);
            yield break;
        }

        ArrangeForBattle(residents, enemies);
        ResetResidentArmorForBattle(residents);
        SetResidentDragging(residents, false);

        while (residents.Count > 0 && enemies.Count > 0)
        {
            for (int i = 0; i < residents.Count && enemies.Count > 0; i++)
            {
                ResidentCombatStats resident = residents[i];
                if (resident == null || resident.IsDead) continue;

                EnemyCombatStats target = FindLowestHealthEnemy(enemies);
                if (target == null) break;

                yield return Attack(
                    resident.transform,
                    target.transform,
                    resident.AttackPower,
                    ResidentHitChance,
                    damage => target.TakeDamage(damage));

                RemoveDeadEnemies(enemies);
                yield return new WaitForSeconds(turnDelay);
            }

            for (int i = 0; i < enemies.Count && residents.Count > 0; i++)
            {
                EnemyCombatStats enemy = enemies[i];
                if (enemy == null || enemy.IsDead) continue;

                ResidentCombatStats target = residents[Random.Range(0, residents.Count)];

                yield return Attack(
                    enemy.transform,
                    target.transform,
                    enemy.AttackPower,
                    EnemyHitChance,
                    damage => target.TakeDamage(damage));

                RemoveDeadResidents(residents);
                yield return new WaitForSeconds(turnDelay);
            }
        }

        EndBattle(enemies);
    }

    private void ArrangeForBattle(
        List<ResidentCombatStats> residents,
        List<EnemyCombatStats> enemies)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < residents.Count; i++)
            center += residents[i].transform.position;
        for (int i = 0; i < enemies.Count; i++)
            center += enemies[i].transform.position;
        center /= residents.Count + enemies.Count;

        PositionRow(enemies, center + Vector3.up * (rowDistance * 0.5f));
        PositionRow(residents, center + Vector3.down * (rowDistance * 0.5f));
    }

    private void PositionRow<T>(List<T> stats, Vector3 rowCenter)
        where T : Component
    {
        float startX = rowCenter.x - (stats.Count - 1) * cardSpacing * 0.5f;

        for (int i = 0; i < stats.Count; i++)
        {
            Transform card = stats[i].transform;
            card.position = new Vector3(startX + cardSpacing * i, rowCenter.y, card.position.z);

            Renderer[] renderers = card.GetComponentsInChildren<Renderer>(true);
            for (int r = 0; r < renderers.Length; r++)
                renderers[r].sortingOrder = 30 + i;
        }
    }

    private static void ResetResidentArmorForBattle(List<ResidentCombatStats> residents)
    {
        for (int i = 0; i < residents.Count; i++)
        {
            if (residents[i] != null && !residents[i].IsDead)
                residents[i].ResetArmorForBattle();
        }
    }

    private IEnumerator Attack(
        Transform attacker,
        Transform defender,
        int damage,
        float hitChance,
        System.Action<int> applyDamage)
    {
        Vector3 attackerStart = attacker.position;
        Vector3 defenderStart = defender.position;
        Vector3 direction = (defenderStart - attackerStart).normalized;
        Vector3 attackPoint = defenderStart - direction * 0.45f;

        yield return MoveTransform(attacker, attackerStart, attackPoint, attackMoveDuration);

        if (Random.value <= hitChance)
        {
            applyDamage(damage);
            ShowDamagePopup(defender, damage);
            yield return MoveTransform(
                defender,
                defenderStart,
                defenderStart + direction * knockbackDistance,
                attackMoveDuration * 0.5f);
            yield return MoveTransform(
                defender,
                defender.position,
                defenderStart,
                attackMoveDuration * 0.5f);
        }

        yield return MoveTransform(attacker, attacker.position, attackerStart, attackMoveDuration);
    }

    private void ShowDamagePopup(Transform defender, int damage)
    {
        if (defender == null || damage <= 0)
            return;

        SpriteRenderer cardRenderer = GetCardSpriteRenderer(defender);
        Bounds cardBounds = cardRenderer != null
            ? cardRenderer.bounds
            : new Bounds(defender.position, Vector3.one);

        GameObject popupObject = new GameObject(
            "DamagePopup",
            typeof(RectTransform),
            typeof(TextMeshPro));

        TextMeshPro popupText = popupObject.GetComponent<TextMeshPro>();
        popupText.text = "-" + damage;
        popupText.fontStyle = FontStyles.Bold;
        popupText.alignment = TextAlignmentOptions.Center;
        popupText.color = new Color(0.95f, 0.12f, 0.08f, 1f);
        popupText.outlineColor = new Color32(255, 255, 255, 255);
        popupText.outlineWidth = 0.2f;
        popupText.fontSize = Mathf.Max(0.6f, cardBounds.size.y * 0.38f);
        popupText.enableWordWrapping = false;
        popupText.raycastTarget = false;
        popupText.rectTransform.sizeDelta = new Vector2(
            Mathf.Max(1.2f, cardBounds.size.x),
            Mathf.Max(0.7f, cardBounds.size.y * 0.35f));

        float popupZ = cardRenderer != null
            ? cardRenderer.transform.position.z - 0.35f
            : defender.position.z - 0.35f;
        popupObject.transform.position = new Vector3(
            cardBounds.center.x,
            cardBounds.center.y + cardBounds.size.y * 0.12f,
            popupZ);

        Renderer popupRenderer = popupObject.GetComponent<Renderer>();
        if (popupRenderer != null && cardRenderer != null)
        {
            popupRenderer.sortingLayerID = cardRenderer.sortingLayerID;
            popupRenderer.sortingOrder = cardRenderer.sortingOrder + 500;
        }

        StartCoroutine(AnimateDamagePopup(popupText));
    }

    private IEnumerator AnimateDamagePopup(TextMeshPro popupText)
    {
        if (popupText == null)
            yield break;

        Transform popupTransform = popupText.transform;
        Vector3 start = popupTransform.position;
        Vector3 end = start + Vector3.up * damagePopupRiseDistance;
        float duration = Mathf.Max(0.05f, damagePopupDuration);
        float elapsed = 0f;

        while (elapsed < duration && popupText != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            popupTransform.position = Vector3.Lerp(start, end, t);

            Color color = popupText.color;
            color.a = 1f - t;
            popupText.color = color;
            yield return null;
        }

        if (popupText != null)
            Destroy(popupText.gameObject);
    }

    private static SpriteRenderer GetCardSpriteRenderer(Transform card)
    {
        if (card == null)
            return null;

        SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = renderers[i];
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                continue;

            if (spriteRenderer.gameObject.name == "Shadow")
                continue;

            return spriteRenderer;
        }

        return null;
    }

    private static IEnumerator MoveTransform(
        Transform target,
        Vector3 from,
        Vector3 to,
        float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        target.position = to;
    }

    private static EnemyCombatStats FindLowestHealthEnemy(
        List<EnemyCombatStats> enemies)
    {
        EnemyCombatStats target = null;
        int lowestHealth = int.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyCombatStats enemy = enemies[i];
            if (enemy == null || enemy.IsDead) continue;

            if (enemy.CurrentHealth < lowestHealth)
            {
                lowestHealth = enemy.CurrentHealth;
                target = enemy;
            }
        }

        return target;
    }

    private static List<ResidentCombatStats> GetLivingResidents()
    {
        var result = new List<ResidentCombatStats>();
        ResidentCombatStats[] residents = FindObjectsOfType<ResidentCombatStats>();
        for (int i = 0; i < residents.Length; i++)
        {
            if (residents[i] != null &&
                !residents[i].IsDead &&
                residents[i].gameObject.activeInHierarchy)
            {
                result.Add(residents[i]);
            }
        }

        return result;
    }

    private static List<EnemyCombatStats> GetLivingEnemies()
    {
        var result = new List<EnemyCombatStats>();
        EnemyCombatStats[] enemies = FindObjectsOfType<EnemyCombatStats>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null &&
                !enemies[i].IsDead &&
                enemies[i].gameObject.activeInHierarchy)
            {
                result.Add(enemies[i]);
            }
        }

        return result;
    }

    private void RemoveDeadEnemies(List<EnemyCombatStats> enemies)
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            EnemyCombatStats enemy = enemies[i];
            if (enemy == null || !enemy.IsDead) continue;

            enemies.RemoveAt(i);
            GameData gd = DataController.instance.gameData;
            gd.EnemyCount = Mathf.Max(0, gd.EnemyCount - 1);
            Destroy(enemy.gameObject);
        }
    }

    private void RemoveDeadResidents(List<ResidentCombatStats> residents)
    {
        CardManager cardManager = FindObjectOfType<CardManager>();

        for (int i = residents.Count - 1; i >= 0; i--)
        {
            ResidentCombatStats resident = residents[i];
            if (resident == null || !resident.IsDead) continue;

            residents.RemoveAt(i);
            if (cardManager != null)
                cardManager.RemoveSpecificCard(resident.gameObject, 20);
            else
                resident.gameObject.SetActive(false);
        }
    }

    private void EndBattle(List<EnemyCombatStats> enemies)
    {
        SetResidentDragging(GetLivingResidents(), true);

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyAI ai = enemies[i] != null
                ? enemies[i].GetComponent<EnemyAI>()
                : null;
            if (ai != null)
                ai.LeaveBattle();
        }

        IsBattleRunning = false;

        if (GameManager.Instance != null)
            GameManager.Instance.RecalculateTotals();
    }

    private static void SetResidentDragging(
        List<ResidentCombatStats> residents,
        bool enabled)
    {
        for (int i = 0; i < residents.Count; i++)
        {
            MouseDrag drag = residents[i] != null
                ? residents[i].GetComponent<MouseDrag>()
                : null;
            if (drag != null)
                drag.enabled = enabled;
        }
    }
}
