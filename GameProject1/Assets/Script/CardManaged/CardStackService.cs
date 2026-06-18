using System.Collections.Generic;
using UnityEngine;

public static class CardStackService
{
    private const float StackOffsetY = -0.16f;
    private const float StackOffsetZ = -0.01f;
    private const float MergeDistance = 0.75f;

    private sealed class StackGroup
    {
        public readonly List<GameObject> Cards = new List<GameObject>();
    }

    private static readonly Dictionary<GameObject, StackGroup> _groups =
        new Dictionary<GameObject, StackGroup>();

    public static List<GameObject> GetCards(GameObject card)
    {
        Cleanup();

        if (card != null && _groups.TryGetValue(card, out StackGroup group))
            return new List<GameObject>(group.Cards);

        return card == null
            ? new List<GameObject>()
            : new List<GameObject> { card };
    }

    public static bool TryMergeWithMatchingCard(GameObject draggedCard)
    {
        if (draggedCard == null ||
            CardWorkService.IsLocked(draggedCard))
        {
            return false;
        }

        List<GameObject> draggedGroup = GetCards(draggedCard);
        GameObject bestTarget = null;
        float bestDistance = float.MaxValue;
        var gameData = DataController.instance != null
            ? DataController.instance.gameData
            : null;

        if (gameData == null || gameData.Card == null)
            return false;

        for (int i = 0; i < gameData.Card.Count; i++)
        {
            GameObject candidate = gameData.Card[i];
            if (candidate == null ||
                !candidate.activeInHierarchy ||
                draggedGroup.Contains(candidate) ||
                CardWorkService.IsLocked(candidate))
            {
                continue;
            }

            float distance = Vector2.Distance(
                draggedCard.transform.position,
                candidate.transform.position);

            if (distance <= MergeDistance && distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = candidate;
            }
        }

        if (bestTarget == null)
            return false;

        List<GameObject> mergedCards = Merge(draggedCard, bestTarget);
        CraftManager craftManager = Object.FindObjectOfType<CraftManager>();
        if (craftManager != null)
            craftManager.TryStartWorkFromStack(mergedCards);

        return true;
    }

    public static void Detach(GameObject card)
    {
        if (card == null || !_groups.TryGetValue(card, out StackGroup group))
            return;

        group.Cards.Remove(card);
        _groups.Remove(card);

        if (group.Cards.Count <= 1)
        {
            if (group.Cards.Count == 1)
                _groups.Remove(group.Cards[0]);
            return;
        }

        Arrange(group);
    }

    public static List<GameObject> SplitForDrag(GameObject card, int count)
    {
        Cleanup();

        if (card == null)
            return new List<GameObject>();

        if (!_groups.TryGetValue(card, out StackGroup group))
            return new List<GameObject> { card };

        if (CardWorkService.IsAnyLocked(group.Cards))
            return new List<GameObject>();

        int startIndex = group.Cards.IndexOf(card);
        if (startIndex < 0)
            startIndex = 0;

        int availableCount = group.Cards.Count - startIndex;
        int splitCount = Mathf.Clamp(count, 1, availableCount);
        if (splitCount >= group.Cards.Count)
            return new List<GameObject>(group.Cards);

        List<GameObject> splitCards = group.Cards.GetRange(startIndex, splitCount);
        for (int i = 0; i < splitCards.Count; i++)
        {
            GameObject splitCard = splitCards[i];
            group.Cards.Remove(splitCard);
            _groups.Remove(splitCard);
        }

        if (group.Cards.Count <= 1)
        {
            if (group.Cards.Count == 1)
                _groups.Remove(group.Cards[0]);
        }
        else
        {
            for (int i = 0; i < group.Cards.Count; i++)
                _groups[group.Cards[i]] = group;

            Arrange(group);
        }

        if (splitCards.Count > 1)
        {
            var splitGroup = new StackGroup();
            AddUniqueActiveCards(splitGroup.Cards, splitCards);

            for (int i = 0; i < splitGroup.Cards.Count; i++)
                _groups[splitGroup.Cards[i]] = splitGroup;

            Arrange(splitGroup);
            return new List<GameObject>(splitGroup.Cards);
        }

        return splitCards;
    }

    public static void Remove(GameObject card)
    {
        Detach(card);
    }

    public static void CreateWorkStack(
        GameObject anchor,
        IList<GameObject> cards)
    {
        if (anchor == null || cards == null)
            return;

        var group = new StackGroup();
        group.Cards.Add(anchor);

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null || group.Cards.Contains(card))
                continue;

            Detach(card);
            group.Cards.Add(card);
        }

        if (group.Cards.Count < 2)
            return;

        for (int i = 0; i < group.Cards.Count; i++)
            _groups[group.Cards[i]] = group;

        Arrange(group);
    }

    private static List<GameObject> Merge(GameObject sourceCard, GameObject targetCard)
    {
        List<GameObject> sourceCards = GetCards(sourceCard);
        List<GameObject> targetCards = GetCards(targetCard);
        var merged = new StackGroup();

        AddUniqueActiveCards(merged.Cards, targetCards);
        AddUniqueActiveCards(merged.Cards, sourceCards);

        if (merged.Cards.Count < 2)
            return merged.Cards;

        for (int i = 0; i < merged.Cards.Count; i++)
            _groups[merged.Cards[i]] = merged;

        Arrange(merged);
        return new List<GameObject>(merged.Cards);
    }

    private static void Arrange(StackGroup group)
    {
        if (group == null || group.Cards.Count == 0)
            return;

        GameObject anchor = group.Cards[0];
        if (anchor == null)
            return;

        Vector3 anchorPosition = anchor.transform.position;
        for (int i = 0; i < group.Cards.Count; i++)
        {
            GameObject card = group.Cards[i];
            if (card == null) continue;

            card.transform.position = anchorPosition +
                new Vector3(0f, StackOffsetY * i, StackOffsetZ * i);
        }
    }

    private static void AddUniqueActiveCards(
        List<GameObject> destination,
        List<GameObject> source)
    {
        for (int i = 0; i < source.Count; i++)
        {
            GameObject card = source[i];
            if (card != null && card.activeInHierarchy && !destination.Contains(card))
                destination.Add(card);
        }
    }

    private static void Cleanup()
    {
        var invalidCards = new List<GameObject>();
        foreach (KeyValuePair<GameObject, StackGroup> pair in _groups)
        {
            if (pair.Key == null || !pair.Key.activeInHierarchy)
                invalidCards.Add(pair.Key);
        }

        for (int i = 0; i < invalidCards.Count; i++)
            _groups.Remove(invalidCards[i]);
    }
}
