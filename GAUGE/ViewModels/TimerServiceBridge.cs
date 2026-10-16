using System;

namespace GAUGE.Services;

public static class TimerServiceBridge
{
    // З Android у ViewModel (кнопки у шторці)
    public static event Action? OnToggleRequested;
    public static event Action? OnSaveRepsRequested;
    public static event Action? OnResetRequested;
    public static event Action? OnRestRequested;

    // З ViewModel у Android (оновлення шторки)
    public static event Action<string, bool>? OnNotificationUpdateRequested;

    public static void TriggerToggle() => OnToggleRequested?.Invoke();
    public static void TriggerSaveReps() => OnSaveRepsRequested?.Invoke();
    public static void TriggerReset() => OnResetRequested?.Invoke();
    public static void TriggerRest() => OnRestRequested?.Invoke();

    // Викличемо цей метод із ViewModel!
    public static void RequestNotificationUpdate(string timeText, bool isRunning) 
        => OnNotificationUpdateRequested?.Invoke(timeText, isRunning);
}