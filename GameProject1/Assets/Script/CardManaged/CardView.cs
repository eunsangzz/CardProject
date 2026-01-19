using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour, IPoolable
{
    public int CardId { get; private set; }
    
    [SerializeField] private GameObject shadow;

    public void Init(int id)
    {
        CardId = id;
    }

    public void OnSpawned()
    {
        if (shadow) shadow.SetActive(false);
    }

    public void OnDespawned()
    {
        if (shadow) shadow.SetActive(false);
    }
}
