using System;

public static class GameEvents
{
    public static Action<int> OnGoldChanged;
    public static Action<int> OnFoodChanged;
    public static Action<int> OnPlayerChanged;
    public static Action<int, int> OnCardCountChanged;
    public static Action<int> OnDayChanged;
    public static Action<int> OnWorkerChanged;

    public static Action OnInventoryChanged;

    public static Action<int> OnQuestChanged;
}
