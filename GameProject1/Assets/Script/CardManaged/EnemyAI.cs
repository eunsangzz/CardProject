using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.7f;
    [SerializeField] private float battleStartDistance = 0.65f;

    private EnemyCombatStats _stats;
    private bool _inBattle;

    private void Awake()
    {
        _stats = GetComponent<EnemyCombatStats>();
        if (_stats == null)
            _stats = gameObject.AddComponent<EnemyCombatStats>();
    }

    private void Update()
    {
        if (_inBattle ||
            _stats == null ||
            _stats.IsDead ||
            TurnBattleManager.IsBattleRunning)
        {
            return;
        }

        ResidentCombatStats target = FindNearestResident();
        if (target == null)
            return;

        Vector3 delta = target.transform.position - transform.position;
        delta.z = 0f;

        float distance = delta.magnitude;
        if (distance <= battleStartDistance)
        {
            _inBattle = true;
            TurnBattleManager.Instance.StartBattle();
            return;
        }

        transform.position += delta.normalized * moveSpeed * Time.deltaTime;
    }

    public void LeaveBattle()
    {
        _inBattle = false;
    }

    private ResidentCombatStats FindNearestResident()
    {
        ResidentCombatStats[] residents = Object.FindObjectsOfType<ResidentCombatStats>();
        ResidentCombatStats nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < residents.Length; i++)
        {
            ResidentCombatStats resident = residents[i];
            if (resident == null ||
                resident.IsDead ||
                !resident.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector2.SqrMagnitude(
                resident.transform.position - transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = resident;
            }
        }

        return nearest;
    }
}
