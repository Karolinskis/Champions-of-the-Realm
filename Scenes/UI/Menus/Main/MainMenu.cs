using Godot;
using System;

public partial class MainMenu : Control
{
    private PackedScene settingsScene;
    private Globals globals; // global variables and functionality

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
        settingsScene = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Menus/Settings/Settings.tscn");
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
        Settings settingsScreen = settingsScene.Instantiate() as Settings;
        AddChild(settingsScreen);
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