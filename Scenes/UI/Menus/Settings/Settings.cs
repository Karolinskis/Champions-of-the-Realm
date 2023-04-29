namespace ChampionsOfTheRealm;

using System.Collections.Generic;

public partial class Settings : Control
{
    // globals
    private Globals globals;

    // audio variables
    private int musicBus;
    private int sfxBus;
    private Slider musicSlider;
    private Slider sfxSlider;

    // resolution variables
    private OptionButton resolutionMenu;
    int resolutionIndex = 3;

    // Resolution values for drop down menu.
    private Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>
    {
        { "1920x1080", new Vector2I(1920, 1080) },
        { "1600x900", new Vector2I(1600, 900) },
        { "1366x768", new Vector2I(1366, 768) },
        { "1280x720", new Vector2I(1280, 720) },
        { "1024x768", new Vector2I(1024, 768) },
        { "800x600", new Vector2I(800, 600) }
    };

    // Called when the node enters the scene tree for the first time.
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
    /// Changes musicSlider and sfxSlider to correctly represent musicBus and sfxBus volume values.
    /// </summary>
    /// <param name="musicBusValue">musicBus volume value</param>
    /// <param name="sfxBusValue">sfxBus volume value</param>
    public void SetSliderValues(int musicBusValue, int sfxBusValue)
    {
        musicSlider.Value = musicBusValue;
        sfxSlider.Value = sfxBusValue;
    }

    /// <summary>
    /// Method for enabling or disabling music.
    /// </summary>
    /// <param name="button_pressed">Music toggle</param>
    private void MusicToggled(bool buttonPressed)
    {
        AudioServer.SetBusMute(musicBus, !AudioServer.IsBusMute(musicBus));
    }

    /// <summary>
    /// Method for enabling or disabling sfx.
    /// </summary>
    /// <param name="button_pressed">Sfx toggle</param>
    private void SfxToggled(bool buttonPressed)
    {
        AudioServer.SetBusMute(sfxBus, !AudioServer.IsBusMute(sfxBus));
    }

    /// <summary>
    /// Music volume slider
    /// </summary>
    private void MusicSliderVolumeChanged(double value)
    {
        AudioServer.SetBusVolumeDb(musicBus, (float)value);
        Save();
    }

    /// <summary>
    /// SFX volume slider
    /// </summary>
    private void SfxSliderVolumeChanged(double value)
    {
        AudioServer.SetBusVolumeDb(sfxBus, (float)value);
        Save();
    }

    /// <summary>
    /// Adds dictionary items to the resolution drop down menu
    /// </summary>
    private void AddItems()
    {
        foreach (var resolution in Resolutions)
        {
            resolutionMenu.AddItem(resolution.Key);
        }
    }

    /// <summary>
    /// Resolution drop down menu
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
    /// Goes back to the previous scene.
    /// </summary>
    private void ButtonBackPressed()
    {
        Node parent = GetParent();
        Control control = parent.GetNode<Control>("CanvasLayer/Control");
        control.Show();
        QueueFree();
    }

    /// <summary>
    /// Method for saving Resolution and volume sliders.
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