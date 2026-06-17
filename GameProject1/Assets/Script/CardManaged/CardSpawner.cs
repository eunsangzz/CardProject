using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private CardFactory factory;
    [SerializeField] private ObjectPool pool;
    [SerializeField] private Transform cardParent;

    public GameObject Spawn(int cardId)
    {
        return SpawnNew((CardId)cardId);
    }

    public GameObject Spawn(CardInstanceData savedData)
    {
        if (savedData == null) return null;
        return SpawnInternal(savedData.cardId, savedData);
    }

    private GameObject SpawnNew(CardId cardId)
    {
        return SpawnInternal(cardId, null);
    }

    private GameObject SpawnInternal(CardId cardId, CardInstanceData savedData)
    {
        var prefab = factory.GetPrefab((int)cardId);
        if (prefab == null) return null;

        var cardObj = pool.Get(prefab, cardParent);
        CardWorkService.Unlock(cardObj);
        CardStackService.Remove(cardObj);

        var identity = cardObj.GetComponent<CardIdentity>();

        if (identity == null) identity = cardObj.AddComponent<CardIdentity>();

        if (savedData == null)
            identity.InitializeNew(cardId);
        else
            identity.InitializeFromData(savedData);

        if (cardObj.TryGetComponent<CardView>(out var view))
            view.Init((int)cardId);

        if (cardId == CardId.Player)
        {
            var combatStats = cardObj.GetComponent<ResidentCombatStats>();
            if (combatStats == null)
                combatStats = cardObj.AddComponent<ResidentCombatStats>();

            if (savedData == null)
                combatStats.Initialize(identity);
            else
                combatStats.Bind(identity);

            var combatView = cardObj.GetComponent<ResidentCombatView>();
            if (combatView == null)
                combatView = cardObj.AddComponent<ResidentCombatView>();

            combatView.Bind(combatStats);
        }

        return cardObj;
    }

    public void Despawn(GameObject cardObj)
    {
        CardWorkService.Unlock(cardObj);
        CardStackService.Remove(cardObj);
        pool.Release(cardObj);
    }
}
