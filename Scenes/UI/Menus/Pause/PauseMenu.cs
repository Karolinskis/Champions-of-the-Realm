namespace ChampionsOfTheRealm;

public partial class PauseMenu : Control
{
    private PackedScene settingsScene;	// Settings resources.
    private Control control;
    private Globals globals;

    private Godot.Collections.Dictionary<string, Variant> data; // Dictionary containing settings data

    // Called when the node enters the scene tree for the first time.
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
    /// Method for handeling Input
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
    /// Button for resuming the game.
    /// </summary>
    private void ButtonResumePressed()
    {
        GetTree().Paused = false;
        QueueFree();
    }

    /// <summary>
    /// Button for saving current game session.
    /// </summary>
    private void ButtonSavePressed()
    {
        globals.SaveGame();
    }

    /// <summary>
    /// Button for loading existing save file.
    /// </summary>
    private void ButtonLoadPressed()
    {
        globals.LoadingForm = Globals.LoadingForms.Save;
        globals.LoadGame(); // Changing scene through globals
        GetTree().Paused = false; // resuming process
        QueueFree(); // removing pauseMenu
    }

    /// <summary>
    /// Button for accessing the settings menu.
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
    /// Button for quitting the game.
    /// </summary>
    private void ButtonQuitMainMenuPressed()
    {
        GetTree().Paused = false;
        globals.ChangeScene("res://Scenes/UI/Menus/Main/MainMenu.tscn");
        GetParent().QueueFree(); // Using GetParent() since pause menu is a child of Map.tscn
    }

    /// <summary>
    /// Button for quitting the game.
    /// </summary>
    private void ButtonQuitPressed()
    {
        GetTree().Quit();
    }
}
