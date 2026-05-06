// Author: mcf
// Static event bus for cross-module notifications.

using System;

public static class EventManager
{
    /// <summary>
    /// Triggered when the player confirms sleep.
    /// </summary>
    public static event Action OnPlayerSleep;

    public static void TriggerPlayerSleep()
    {
        OnPlayerSleep?.Invoke();
    }

    /// <summary>
    /// Triggered after shipping income is settled.
    /// </summary>
    public static event Action<int> OnShippingSettled;

    public static void TriggerShippingSettled(int totalGold)
    {
        OnShippingSettled?.Invoke(totalGold);
    }
}
