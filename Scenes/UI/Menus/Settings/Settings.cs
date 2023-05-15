namespace ChampionsOfTheRealm;

using System.Collections.Generic;

public partial class Settings : Control
{
    private Globals globals;    // Global variables and functionality

    private int musicBus;   // Music audio bus index
    private int sfxBus;    // SFX audio bus index
    private Slider musicSlider; // Music volume slider
    private Slider sfxSlider;   // SFX volume slider

    private OptionButton resolutionMenu;   // Resolution drop down menu
    int resolutionIndex = 3;   // Index of the current resolution

    // Resolution values for drop-down menu
    private Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>
    {
        { "1920x1080", new Vector2I(1920, 1080) },
        { "1600x900", new Vector2I(1600, 900) },
        { "1366x768", new Vector2I(1366, 768) },
        { "1280x720", new Vector2I(1280, 720) },
        { "1024x768", new Vector2I(1024, 768) },
        { "800x600", new Vector2I(800, 600) }
    };

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

        // audio
        musicBus = AudioServer.GetBusIndex("New Bus");      // audio buses should be named Music and SFX, but I crash while renaming them so it will stay like this for now.
        sfxBus = AudioServer.GetBusIndex("New Bus 2");

        //sliders
        musicSlider = GetNode<Slider>("CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/AudioContainer/MusicContainer/MusicSlider");
        sfxSlider = GetNode<Slider>("CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/AudioContainer/SFXContainer/SFXSlider");

        // resolution
        resolutionMenu = GetNode<OptionButton>("CanvasLayer/Control/CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ResolutionContainer/ResolutionDropDown");
        AddItems();
    }

    /// <summary>
    /// Sets the values of musicSlider and sfxSlider to correctly represent the musicBus and sfxBus volume values
    /// </summary>
    /// <param name="musicBusValue">Music bus volume value</param>
    /// <param name="sfxBusValue">SFX bus volume value</param>
    public void SetSliderValues(int musicBusValue, int sfxBusValue)
    {
        musicSlider.Value = musicBusValue;
        sfxSlider.Value = sfxBusValue;
    }

    /// <summary>
    /// Toggles the music on/off
    /// </summary>
    /// <param name="button_pressed">Music toggle</param>
    private void MusicToggled(bool buttonPressed)
    {
        AudioServer.SetBusMute(musicBus, !AudioServer.IsBusMute(musicBus));
    }

    /// <summary>
    /// Toggles the SFX on/off
    /// </summary>
    /// <param name="button_pressed">Sfx toggle</param>
    private void SfxToggled(bool buttonPressed)
    {
        AudioServer.SetBusMute(sfxBus, !AudioServer.IsBusMute(sfxBus));
    }

    /// <summary>
    /// Event handler for music volume slider
    /// </summary>
    private void MusicSliderVolumeChanged(double value)
    {
        AudioServer.SetBusVolumeDb(musicBus, (float)value);
        Save();
    }

    /// <summary>
    /// Event handler for SFX volume slider
    /// </summary>
    private void SfxSliderVolumeChanged(double value)
    {
        AudioServer.SetBusVolumeDb(sfxBus, (float)value);
        Save();
    }

    /// <summary>
    /// Adds dictionary items to the resolution drop-down menu
    /// </summary>
    private void AddItems()
    {
        foreach (var resolution in Resolutions)
        {
            resolutionMenu.AddItem(resolution.Key);
        }
    }

    /// <summary>
    /// Event handler for the resolution drop-down menu
    /// </summary>
    /// <param name="index">The index of selected entry</param>
    public void ResolutionDropDownItemSelected(int index)
    {
        Vector2I size = Resolutions[resolutionMenu.GetItemText(index)];
        resolutionIndex = Resolutions.Values.ToList().IndexOf(size);
        resolutionMenu.Selected = index;
        DisplayServer.WindowSetSize(size);
        Save();
    }

    /// <summary>
    /// Returns to the previous scene
    /// </summary>
    private void ButtonBackPressed()
    {
        Node parent = GetParent();
        Control control = parent.GetNode<Control>("CanvasLayer/Control");
        control.Show();
        QueueFree();
    }

    /// <summary>
    /// Saves the resolution and volume slider settings
    /// </summary>
    public void Save()
    {
        Vector2I resolution = DisplayServer.WindowGetSize();
        
        Godot.Collections.Dictionary<string, Variant> data = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "ResolutionIndex", resolutionIndex },
            { "ResolutionX", resolution.X},
            { "ResolutionY", resolution.Y},
            { "MusicBusValue", AudioServer.GetBusVolumeDb(musicBus)},
            { "SfxBusValue", AudioServer.GetBusVolumeDb(sfxBus)}
        };

        globals.SaveSettings(data);
    }
}