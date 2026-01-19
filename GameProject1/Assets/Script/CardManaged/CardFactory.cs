using System;
using System.Collections.Generic;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [Serializable]
    public struct CardPrefabEntry
    {
        public int id;
        public GameObject prefab;
    }

    [SerializeField] private List<CardPrefabEntry> entries = new();
    private Dictionary<int, GameObject> _map;

    private void Awake()
    {
        _map = new Dictionary<int, GameObject>();
        foreach (var e in entries)
        {
            if (e.prefab == null) continue;
            _map[e.id] = e.prefab;
        }
    }

    public GameObject GetPrefab(int id)
    {
        if (_map.TryGetValue(id, out var prefab))
            return prefab;

        Debug.LogError($"[CardFactory] No prefab mapped for id={id}");
        return null;
    }
}
