namespace ChampionsOfTheRealm;

public partial class PauseMenu : Control
{
    private PackedScene settingsScene;	// Settings resources.
    private Control control;
    private Globals globals;

    private Godot.Collections.Dictionary<string, Variant> data; // Dictionary containing settings data

    public override void _Ready()
    {
        // Pausing the game
        GetTree().Paused = true;

        // To be used for showing/hiding pause menu.
        control = GetNodeOrNull<Control>("CanvasLayer/Control");
        globals = GetNode<Globals>("/root/Globals");

        // Loading settings scene
        settingsScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Settings/Settings.tscn");
        data = globals.LoadSettings();
    }

    /// <summary>
    /// Handles input events.
    /// </summary>
    /// <param name="event">Input event</param>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKeyboardKey)
        {
            if (eventKeyboardKey.Keycode == Key.Escape && eventKeyboardKey.IsPressed())
            {
                GetTree().Paused = false;
                QueueFree();
            }
        }
    }

    /// <summary>
    /// Resumes the game when the "Resume" button is pressed.
    /// </summary>
    private void ButtonResumePressed()
    {
        GetTree().Paused = false;
        QueueFree();
    }

    /// <summary>
    /// Loads an existing save file when the "Load" button is pressed.
    /// </summary>
    private void ButtonLoadPressed()
    {
        GetTree().Paused = false;
        globals.LoadingForm = Globals.LoadingForms.Save;
        globals.LoadGame(); // Changing scene through globals
        QueueFree(); // removing pauseMenu
    }

    /// <summary>
    /// Accesses the settings menu when the "Settings" button is pressed.
    /// </summary>
    private void ButtonSettingsPressed()
    {
        data = globals.LoadSettings();
        Settings settingsScreen = settingsScene.Instantiate<Settings>();
        AddChild(settingsScreen);

        if (data != null)
        {
            // Setting resolution drop down and volume sliders
            int resolution = (int)data["ResolutionIndex"];
            int musicBusValue = (int)data["MusicBusValue"];
            int sfxBusValue = (int)data["SfxBusValue"];
            settingsScreen.ResolutionDropDownItemSelected(resolution);
            settingsScreen.SetSliderValues(musicBusValue, sfxBusValue);
        }

        Control control = GetNode<Control>("CanvasLayer/Control");
        control.Hide();
    }

    /// <summary>
    /// Quits to the main menu when the "Quit to main menu" button is pressed.
    /// </summary>
    private void ButtonQuitMainMenuPressed()
    {
        GetTree().Paused = false;
        globals.ChangeScene("res://Scenes/UI/Menus/Main/MainMenu.tscn");
        GetParent().QueueFree(); // Using GetParent() since pause menu is a child of Map.tscn
    }

    /// <summary>
    /// Quits the game when the "Quit" button is pressed.
    /// </summary>
    private void ButtonQuitPressed()
    {
        GetTree().Quit();
    }
}
