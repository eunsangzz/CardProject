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
        var prefab = factory.GetPrefab(cardId);
        if (prefab == null) return null;

        var cardObj = pool.Get(prefab, cardParent);

        if (cardObj.TryGetComponent<CardView>(out var view))
            view.Init(cardId);

        return cardObj;
    }

    public void Despawn(GameObject cardObj)
    {
        pool.Release(cardObj);
    }
}
