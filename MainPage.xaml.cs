
using System.Diagnostics; // For Debug.WriteLine

#if WINDOWS
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System; 
#endif

namespace KeyEventsDemo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        this.Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
#if WINDOWS
        var platformView = this.Handler?.PlatformView as UIElement;

        if (platformView != null)
        {
            platformView.KeyDown += PlatformView_KeyDown;
            platformView.KeyUp += PlatformView_KeyUp; // Still useful for general key up feedback

            StatusLabel.Text = "Page loaded. Press keys!";
        }
        else
        {
            StatusLabel.Text = "Error: Platform view not found.";
        }
#else
        StatusLabel.Text = "This demo is for Windows only. Press keys anyway!";
#endif
    }

#if WINDOWS
    // Handler for the Windows UIElement's KeyDown event
    private void PlatformView_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        // Debug output to see all events
        Debug.WriteLine($"KeyDown: {e.Key} | OriginalKey: {e.OriginalKey} | DeviceId: {e.DeviceId}");

        // Update the UI with information about the individual key press (optional, for feedback)
        StatusLabel.Text = $"Key Down: {e.Key}";

        // --- Detecting Key Combinations ---

        // 1. Check for Ctrl + C
        // GetKeyStateForCurrentThread returns the state of a virtual key.
        // We check if Control key is currently 'Down'.
        var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
        bool isCtrlPressed = ctrlState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (isCtrlPressed && e.Key == VirtualKey.C)
        {
            StatusLabel.Text = "Ctrl + C Pressed!";
            Debug.WriteLine(">>> Ctrl + C shortcut detected!");
            e.Handled = true; // Consume the event if you've handled the shortcut
            return; // Exit after handling this combination
        }

        // 2. Check for Alt + F
        var altState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu); // VirtualKey.Menu is the Alt key
        bool isAltPressed = altState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (isAltPressed && e.Key == VirtualKey.F)
        {
            StatusLabel.Text = "Alt + F Pressed!";
            Debug.WriteLine(">>> Alt + F shortcut detected!");
            e.Handled = true; // Consume the event
            return; // Exit
        }

        // 3. Example: Shift + Enter
        var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
        bool isShiftPressed = shiftState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (isShiftPressed && e.Key == VirtualKey.Enter)
        {
            StatusLabel.Text = "Shift + Enter Pressed!";
            Debug.WriteLine(">>> Shift + Enter shortcut detected!");
            e.Handled = true; // Consume the event
            return; // Exit
        }

        // You can add more combinations here.
        // If no combination was handled, the StatusLabel will show the last individual key pressed.
    }

    // Handler for the Windows UIElement's KeyUp event
    private void PlatformView_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        Debug.WriteLine($"KeyUp: {e.Key} | OriginalKey: {e.OriginalKey} | DeviceId: {e.DeviceId}");
        // You might want to clear the status label or provide different feedback on key up.
        // StatusLabel.Text = $"Key Up: {e.Key}"; // Re-enable if you want persistent key-up feedback
    }
#endif
    // Clean up event handlers
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
#if WINDOWS
        var platformView = this.Handler?.PlatformView as UIElement;
        if (platformView != null)
        {
            platformView.KeyDown -= PlatformView_KeyDown;
            platformView.KeyUp -= PlatformView_KeyUp;
        }
#endif
    }
}