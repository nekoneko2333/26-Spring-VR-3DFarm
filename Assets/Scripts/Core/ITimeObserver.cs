// Author: mcf
// Implement this interface for objects that react to the shared game clock.

public interface ITimeObserver
{
    /// <summary>
    /// Called after game minutes advance.
    /// </summary>
    /// <param name="totalMinutes">Total game minutes since day-count start.</param>
    void OnMinuteChanged(int totalMinutes);

    /// <summary>
    /// Called when the current game hour changes.
    /// </summary>
    /// <param name="currentHour">Current hour, 0-23.</param>
    void OnHourChanged(int currentHour);

    /// <summary>
    /// Called after the game advances to a new day.
    /// </summary>
    /// <param name="currentDay">Current day number.</param>
    void OnDayPassed(int currentDay);
}
