using UnityEngine;

public class CardIdentity : MonoBehaviour
{
    [SerializeField] private CardInstanceData data;

    public CardId cardId
    {
        get => Data.cardId;
        set => Data.cardId = value;
    }

    public string UniqueId => Data.uniqueId;
    public CardInstanceData Data => data ?? (data = CardInstanceData.CreateNew(CardId.Wood));

    public void InitializeNew(CardId id)
    {
        data = CardInstanceData.CreateNew(id);
    }

    public void InitializeFromData(CardInstanceData savedData)
    {
        data = savedData != null
            ? savedData.Clone()
            : CardInstanceData.CreateNew(CardId.Wood);

        if (string.IsNullOrEmpty(data.uniqueId))
            data.uniqueId = System.Guid.NewGuid().ToString("N");

        transform.position = data.GetPosition();
    }

    public CardInstanceData CaptureData()
    {
        Data.CapturePosition(transform.position);
        return Data.Clone();
    }
}
