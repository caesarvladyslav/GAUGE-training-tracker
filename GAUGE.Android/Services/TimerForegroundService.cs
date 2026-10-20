using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System;
using GAUGE.Services;

namespace GAUGE.Android.Services;

[Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeSpecialUse)]
public class TimerForegroundService : Service
{
    public const string CHANNEL_ID = "gauge_timer_channel";
    public const int NOTIFICATION_ID = 1001;

    public const string ACTION_START = "ACTION_START";
    public const string ACTION_PAUSE = "ACTION_PAUSE";
    public const string ACTION_RESET = "ACTION_RESET";
    public const string ACTION_SAVE_REPS = "ACTION_SAVE_REPS";
    public const string ACTION_REST = "ACTION_REST";

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        string action = intent?.Action ?? string.Empty;
        string timeText = intent?.GetStringExtra("TIME_TEXT") ?? "00:00";
        bool isRunning = intent?.GetBooleanExtra("IS_RUNNING", false) ?? false;

        if (action == ACTION_PAUSE || action == ACTION_START)
        {
            TimerServiceBridge.TriggerToggle();
        }
        else if (action == ACTION_SAVE_REPS)
        {
            TimerServiceBridge.TriggerSaveReps();
        }
        else if (action == ACTION_RESET)
        {
            TimerServiceBridge.TriggerReset();
        }
        else if (action == ACTION_REST)
        {
            TimerServiceBridge.TriggerRest(); // <-- Прокидаємо у ViewModel
        }

        var notification = BuildNotification(timeText, isRunning);
        StartForeground(NOTIFICATION_ID, notification);

        return StartCommandResult.Sticky;
    }

    private Notification BuildNotification(string timeText, bool isRunning)
    {
        var pendingFlags = Build.VERSION.SdkInt >= BuildVersionCodes.M
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        // Інтент для кнопки Старт / Пауза
        var toggleIntent = new Intent(this, typeof(TimerForegroundService));
        toggleIntent.SetAction(isRunning ? ACTION_PAUSE : ACTION_START);
        var togglePendingIntent = PendingIntent.GetService(this, 0, toggleIntent, pendingFlags);

      // Інтент для кнопки "Відпочинок"
        var restIntent = new Intent(this, typeof(TimerForegroundService));
        restIntent.SetAction(ACTION_REST);
        var restPendingIntent = PendingIntent.GetService(this, 1, restIntent, pendingFlags);

        // Інтент для кнопки "Скидання"
        var resetIntent = new Intent(this, typeof(TimerForegroundService));
        resetIntent.SetAction(ACTION_RESET);
        var resetPendingIntent = PendingIntent.GetService(this, 2, resetIntent, pendingFlags);

        var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
            .SetContentTitle("GAUGE — Таймер")
            .SetContentText($"Час: {timeText}")
            .SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
            .SetOngoing(true)
            .SetOnlyAlertOnce(true)
            .AddAction(0, isRunning ? " || Пауза" : " ▶ Старт", togglePendingIntent)
            .AddAction(0, " В відпочинок", restPendingIntent)
            .AddAction(0, " ⛌ Скинути", resetPendingIntent);

        return builder.Build();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                CHANNEL_ID,
                "GAUGE Секундомір",
                NotificationImportance.Low)
            {
                Description = "Канал для роботи таймера тренування у шторці"
            };

            var manager = (NotificationManager?)GetSystemService(NotificationService);
            manager?.CreateNotificationChannel(channel);
        }
    }

    public override void OnTaskRemoved(Intent? rootIntent)
{
    base.OnTaskRemoved(rootIntent);
    
    // Зупиняємо таймер та прибираємо нотифікацію
    StopForeground(StopForegroundFlags.Remove);
    StopSelf();
}
}