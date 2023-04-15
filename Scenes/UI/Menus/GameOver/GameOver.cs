using Godot;
using System;

public partial class GameOver : Control
{
    private Globals globals; // global variables and functionality

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Paused = true;
        globals = GetNode<Globals>("/root/Globals");
    }
	
    /// <summary>
    /// Button for restarting scene.
    /// </summary>
    private void ButtonRestartPressed()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }
	
    /// <summary>
    /// Button for loading an existing save file.
    /// </summary>
    private void ButtonLoadPressed()
    {
        globals.LoadingForm = Globals.LoadingForms.Save;
        globals.LoadGame(); // Changing scene through globals
        GetTree().Paused = false; // resuming process
        QueueFree(); // removing pauseMenu
    }
    
    /// <summary>
    /// Button for quitting to main menu.
    /// </summary>
    private void ButtonQuitMainPressed()
    {
        GetTree().Paused = false;
	globals.ChangeScene("res://Scenes/UI/Menus/Main/MainMenu.tscn");
	GetParent().QueueFree(); // Using GetParent() since gameOver screen is a child of Map.tscn
    }
    
    /// <summary>
    /// Button for quiting to desktop.
    /// </summary>
    private void ButtonQuitPressed()
    {
        GetTree().Quit();
    }
}
