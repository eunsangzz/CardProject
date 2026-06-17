using System.Collections.Generic;
using UnityEngine;

public static class CardSpawnPositionFinder
{
    private const float MinX = -5f;
    private const float MaxX = 5f;
    private const float MinY = -4f;
    private const float MaxY = 2f;
    private const float CardSpacing = 1.15f;
    private const int RandomAttempts = 80;

    public static Vector3 FindAvailablePosition(IReadOnlyList<GameObject> cards)
    {
        for (int i = 0; i < RandomAttempts; i++)
        {
            Vector3 candidate = CreateRandomPosition();
            if (IsAvailable(candidate, cards))
                return candidate;
        }

        return FindBestGridPosition(cards);
    }

    private static Vector3 CreateRandomPosition()
    {
        return new Vector3(
            Random.Range(MinX, MaxX),
            Random.Range(MinY, MaxY),
            0f);
    }

    private static bool IsAvailable(Vector3 candidate, IReadOnlyList<GameObject> cards)
    {
        if (cards == null) return true;

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null || !card.activeInHierarchy) continue;

            Vector3 position = card.transform.position;
            if (Mathf.Abs(candidate.x - position.x) < CardSpacing &&
                Mathf.Abs(candidate.y - position.y) < CardSpacing)
            {
                return false;
            }
        }

        return true;
    }

    private static Vector3 FindBestGridPosition(IReadOnlyList<GameObject> cards)
    {
        Vector3 bestPosition = CreateRandomPosition();
        float bestDistance = -1f;
        float offsetX = Random.Range(0f, CardSpacing);
        float offsetY = Random.Range(0f, CardSpacing);

        for (float y = MinY + offsetY; y <= MaxY; y += CardSpacing)
        {
            for (float x = MinX + offsetX; x <= MaxX; x += CardSpacing)
            {
                Vector3 candidate = new Vector3(x, y, 0f);
                float distance = GetNearestCardDistance(candidate, cards);

                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestPosition = candidate;
                }
            }
        }

        return bestPosition;
    }

    private static float GetNearestCardDistance(
        Vector3 candidate,
        IReadOnlyList<GameObject> cards)
    {
        if (cards == null || cards.Count == 0)
            return float.MaxValue;

        float nearestDistance = float.MaxValue;

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null || !card.activeInHierarchy) continue;

            float distance = Vector2.SqrMagnitude(
                (Vector2)(candidate - card.transform.position));
            nearestDistance = Mathf.Min(nearestDistance, distance);
        }

        return nearestDistance;
    }
}
