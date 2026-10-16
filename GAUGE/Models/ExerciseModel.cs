using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GAUGE.Models;

public partial class ExerciseModel : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _reps;
    [ObservableProperty] private int _restSeconds;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private string _category = "Верх";
    [ObservableProperty] private DateTime _lastUsedAt = DateTime.MinValue;
    
    [ObservableProperty] private bool _isTimeBased;

    public string FormattedValue
    {
        get
        {
            if (IsTimeBased)
            {
                TimeSpan ts = TimeSpan.FromSeconds(Reps);
                int totalHours = (int)ts.TotalHours;
                int mins = ts.Minutes;
                int secs = ts.Seconds;

                if (totalHours >= 1)
                {
                    return string.Format("{0}:{1:D2}:{2:D2}", totalHours, mins, secs);
                }
                return string.Format("{0:D2}:{1:D2}", mins, secs);
            }
            return string.Format("{0} повт.", Reps);
        }
    }
}