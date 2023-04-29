namespace ChampionsOfTheRealm;

public partial class MainMenu : Control
{
    private PackedScene settingsScene;
    private Globals globals; // global variables and functionality
    protected bool LoadNew;
    private Godot.Collections.Dictionary<string, Variant> data; // Dictionary containing settings data

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
        settingsScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Settings/Settings.tscn");
        data = globals.LoadSettings();
    }

    /// <summary>
    /// Button for starting a new game.
    /// </summary>
    private void ButtonStartNewPressed()
    {
        globals.LoadingForm = Globals.LoadingForms.New;
        globals.ChangeScene("res://Scenes/Maps/Main/Main.tscn");
        QueueFree();
    }

    /// <summary>
    /// Button for loading an existing save file.
    /// </summary>
    private void ButtonLoadPressed()
    {
        globals.LoadingForm = Globals.LoadingForms.Save;
        globals.LoadGame(); // Changing scene through globals
        //QueueFree(); // Removing main menu
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
    /// Button for quiting the game.
    /// </summary>
    private void ButtonQuitPressed()
    {
        GetTree().Quit();
    }
}