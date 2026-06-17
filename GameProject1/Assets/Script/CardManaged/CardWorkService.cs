using System.Collections.Generic;
using UnityEngine;

public static class CardWorkService
{
    public sealed class WorkHandle
    {
        internal readonly List<GameObject> Residents = new List<GameObject>();
        internal readonly List<GameObject> Participants = new List<GameObject>();
        internal readonly Dictionary<GameObject, Vector3> OriginalPositions =
            new Dictionary<GameObject, Vector3>();
        internal CardWorkProgress Progress;
        internal bool CanCancel;
        internal bool IsCanceled;
        internal System.Action OnCancel;

        public Vector3 AnchorPosition { get; internal set; }
    }

    private static readonly HashSet<GameObject> _lockedCards =
        new HashSet<GameObject>();
    private static readonly Dictionary<GameObject, WorkHandle> _workByCard =
        new Dictionary<GameObject, WorkHandle>();

    public static bool IsLocked(GameObject card)
    {
        return card != null && _lockedCards.Contains(card);
    }

    public static bool IsAnyLocked(IList<GameObject> cards)
    {
        if (cards == null) return false;

        for (int i = 0; i < cards.Count; i++)
        {
            if (IsLocked(cards[i]))
                return true;
        }

        return false;
    }

    public static void Lock(GameObject card)
    {
        if (card != null)
            _lockedCards.Add(card);
    }

    public static void Unlock(GameObject card)
    {
        if (card != null)
            _lockedCards.Remove(card);
    }

    public static bool TryBeginResidentWork(
        GameObject target,
        int residentCount,
        out WorkHandle handle)
    {
        var participants = new List<GameObject>();
        if (target != null)
            participants.Add(target);

        return TryBeginWork(
            participants,
            residentCount,
            0f,
            out handle);
    }

    public static bool TryBeginWork(
        IList<GameObject> workCards,
        int residentCount,
        float duration,
        out WorkHandle handle)
    {
        handle = new WorkHandle();
        if (residentCount <= 0)
            return false;

        if (workCards != null)
        {
            for (int i = 0; i < workCards.Count; i++)
            {
                if (IsLocked(workCards[i]))
                    return false;
            }
        }

        var gameData = DataController.instance != null
            ? DataController.instance.gameData
            : null;

        if (gameData == null || gameData.Card == null)
            return false;

        AddMissingResidentsToRuntimeList(gameData);

        for (int i = 0; i < gameData.Card.Count; i++)
        {
            GameObject card = gameData.Card[i];
            if (card == null ||
                !card.activeInHierarchy ||
                IsLocked(card) ||
                !card.TryGetComponent(out CardIdentity identity) ||
                identity.cardId != CardId.Player)
            {
                continue;
            }

            CardStackService.Detach(card);
            handle.Residents.Add(card);
            handle.Participants.Add(card);
            handle.OriginalPositions[card] = card.transform.position;

            Lock(card);
            SetDragEnabled(card, false);

            if (handle.Residents.Count >= residentCount)
                break;
        }

        if (handle.Residents.Count < residentCount)
        {
            EndResidentWork(handle);
            handle = null;
            return false;
        }

        GameObject anchor = handle.Residents[0];
        handle.AnchorPosition = anchor.transform.position;

        if (workCards != null)
        {
            for (int i = 0; i < workCards.Count; i++)
            {
                GameObject card = workCards[i];
                if (card == null || handle.Participants.Contains(card))
                    continue;

                CardStackService.Detach(card);
                handle.Participants.Add(card);
                handle.OriginalPositions[card] = card.transform.position;
                Lock(card);
                SetDragEnabled(card, false);
            }
        }

        CardStackService.CreateWorkStack(anchor, handle.Participants);

        handle.Progress = anchor.GetComponent<CardWorkProgress>();
        if (handle.Progress == null)
            handle.Progress = anchor.AddComponent<CardWorkProgress>();

        if (duration > 0f)
            handle.Progress.Begin(duration);

        return true;
    }

    public static bool TryBeginStackWork(
        IList<GameObject> residents,
        IList<GameObject> workCards,
        float duration,
        bool canCancel,
        System.Action onCancel,
        out WorkHandle handle)
    {
        handle = new WorkHandle
        {
            CanCancel = canCancel,
            OnCancel = onCancel
        };

        if (residents == null || residents.Count == 0)
            return false;

        for (int i = 0; i < residents.Count; i++)
        {
            GameObject resident = residents[i];
            if (resident == null ||
                IsLocked(resident) ||
                !resident.TryGetComponent(out CardIdentity identity) ||
                identity.cardId != CardId.Player)
            {
                return false;
            }
        }

        if (workCards != null)
        {
            for (int i = 0; i < workCards.Count; i++)
            {
                if (IsLocked(workCards[i]))
                    return false;
            }
        }

        GameObject anchor = residents[0];
        handle.AnchorPosition = anchor.transform.position;

        for (int i = 0; i < residents.Count; i++)
            AddParticipant(handle, residents[i], isResident: true);

        if (workCards != null)
        {
            for (int i = 0; i < workCards.Count; i++)
                AddParticipant(handle, workCards[i], isResident: false);
        }

        CardStackService.CreateWorkStack(anchor, handle.Participants);

        handle.Progress = anchor.GetComponent<CardWorkProgress>();
        if (handle.Progress == null)
            handle.Progress = anchor.AddComponent<CardWorkProgress>();

        if (duration > 0f)
            handle.Progress.Begin(duration);

        return true;
    }

    public static bool TryCancelWork(GameObject card)
    {
        if (card == null ||
            !_workByCard.TryGetValue(card, out WorkHandle handle) ||
            handle == null ||
            !handle.CanCancel ||
            handle.IsCanceled)
        {
            return false;
        }

        handle.IsCanceled = true;
        handle.OnCancel?.Invoke();
        EndResidentWork(handle);
        return true;
    }

    private static void AddMissingResidentsToRuntimeList(GameData gameData)
    {
        CardIdentity[] identities = Object.FindObjectsOfType<CardIdentity>();
        for (int i = 0; i < identities.Length; i++)
        {
            CardIdentity identity = identities[i];
            if (identity == null ||
                !identity.gameObject.activeInHierarchy ||
                identity.cardId != CardId.Player ||
                gameData.Card.Contains(identity.gameObject))
            {
                continue;
            }

            gameData.Card.Add(identity.gameObject);
        }
    }

    public static void EndResidentWork(WorkHandle handle)
    {
        if (handle == null)
            return;

        if (handle.Progress != null)
            handle.Progress.Finish();

        for (int i = 0; i < handle.Participants.Count; i++)
        {
            GameObject card = handle.Participants[i];
            if (card == null) continue;

            CardStackService.Detach(card);
            _workByCard.Remove(card);

            if (handle.OriginalPositions.TryGetValue(card, out Vector3 position))
                card.transform.position = position;

            SetDragEnabled(card, true);
            Unlock(card);
        }

        for (int i = 0; i < handle.Residents.Count; i++)
        {
            GameObject resident = handle.Residents[i];
            if (resident != null)
                CardStackService.TryMergeWithMatchingCard(resident);
        }
    }

    private static void SetDragEnabled(GameObject card, bool enabled)
    {
        if (card != null && card.TryGetComponent(out MouseDrag drag))
            drag.enabled = enabled;
    }

    private static void AddParticipant(
        WorkHandle handle,
        GameObject card,
        bool isResident)
    {
        if (handle == null ||
            card == null ||
            handle.Participants.Contains(card))
        {
            return;
        }

        CardStackService.Detach(card);
        handle.Participants.Add(card);
        if (isResident)
            handle.Residents.Add(card);

        handle.OriginalPositions[card] = card.transform.position;
        _workByCard[card] = handle;
        Lock(card);
        SetDragEnabled(card, false);

        if (handle.CanCancel && card.GetComponent<CardWorkCancelClick>() == null)
            card.AddComponent<CardWorkCancelClick>();
    }
}
