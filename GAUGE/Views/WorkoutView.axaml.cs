using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using GAUGE.ViewModels;

namespace GAUGE.Views;

public partial class WorkoutView : UserControl
{
    private bool _isDraggingRep = false;
    private bool _isDraggingRest = false;
    private DispatcherTimer? _autoCloseTimer;

    public WorkoutView()
    {
        InitializeComponent();
        Loaded += (s, e) => SetupSliders();

        // Таймер-страховка на 1 секунду: якщо змахнув і подія загубилась, він сам закриє оверлей
        _autoCloseTimer = new DispatcherTimer { Interval = System.TimeSpan.FromSeconds(1) };
        _autoCloseTimer.Tick += (s, e) =>
        {
            _autoCloseTimer.Stop();
            ForceCloseOverlays();
        };

        AddHandler(InputElement.PointerReleasedEvent, OnGlobalPointerUp, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(InputElement.PointerCaptureLostEvent, OnGlobalPointerUp, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
    }

    private void SetupSliders()
    {
        var repSlider = this.FindControl<Slider>("RepSlider");
        var restSlider = this.FindControl<Slider>("RestSlider");

        if (repSlider != null)
        {
            repSlider.AddHandler(InputElement.PointerPressedEvent, (s, e) => 
            { 
                _isDraggingRep = true;
                _autoCloseTimer?.Stop();
                _autoCloseTimer?.Start(); // Запускаємо відлік при кожному натисканні
                if (DataContext is MainWindowViewModel vm) vm.IsRepOverlayVisible = true; 
            }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        }

        if (restSlider != null)
        {
            restSlider.AddHandler(InputElement.PointerPressedEvent, (s, e) => 
            { 
                _isDraggingRest = true;
                _autoCloseTimer?.Stop();
                _autoCloseTimer?.Start();
                if (DataContext is MainWindowViewModel vm) vm.IsRestOverlayVisible = true; 
            }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        }
    }

    private void OnGlobalPointerUp(object? sender, PointerEventArgs e)
    {
        _autoCloseTimer?.Stop(); // Якщо палець відпустили нормально — зупиняємо таймер
        ForceCloseOverlays();
    }

    private void ForceCloseOverlays()
    {
        _isDraggingRep = false;
        _isDraggingRest = false;
        if (DataContext is MainWindowViewModel vm)
        {
            vm.IsRepOverlayVisible = false;
            vm.IsRestOverlayVisible = false;
        }
    }
}