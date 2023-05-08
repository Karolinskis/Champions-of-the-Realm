global using Godot;
global using System;
global using System.Linq;

using Godot.Collections;

namespace ChampionsOfTheRealm;

/// <summary>
/// Class for handling global actions (Scene switching, save/load functionality)
/// </summary>
public partial class Globals : Node
{
    private const string SaveDir = "user://saves/"; // save files directory path
    private string savePath = SaveDir + "save.dat"; // save file path
    private string settingsPath = SaveDir + "settings.dat"; // settings file path
    [Signal] public delegate void CoinsDropedEventHandler(int coins, Marker2D position);

    /// <summary>
    /// Loading forms which indicate how map scenes should be loaded
    /// (Loading new game, loading from save file or just certain scenes,
    /// for example when map level is switched)
    /// </summary>
    public enum LoadingForms
    {
        New,
        Save,
        Load
    }
    public LoadingForms LoadingForm { get; set; } = LoadingForms.New;

    /// <summary>
    /// Dictionary for storing player data
    /// Used for saving, loading and passing player data between scenes
    /// </summary>
    public Dictionary<string, Variant> PlayerSave { get; set; }

    PackedScene loadingScreenScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Loading/LoadingScreen.tscn"); // Loading scene resource

    /// <summary>
    /// Method which saves all nodes in Persist group by parsing dictionary to JSON file
    /// </summary>
    /// <returns>Returns true, if saved successfully, otherwise false</returns>
    public bool SaveGame(Dictionary<string, Variant> playerData)
    {
        if (!DirAccess.DirExistsAbsolute(SaveDir)) // Checks if directory exists
        {
            DirAccess.MakeDirRecursiveAbsolute(SaveDir); // if not create one
        }

        // opening file and encrpypting file with password
        FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Write, "nekompiliuoja");
        if (saveFile.GetError() != Error.Ok) // checking if opened JSON file without errors
        {
            GD.PushError("Failed to save data!"); // for debuging purposes
            return false;
        }
        // Saving player data to JSON file
        PlayerSave = playerData;
        saveFile.StoreLine(Json.Stringify(PlayerSave));
        // GD.Print(PlayerSave); // For debuging purposes
        saveFile.Close();
        return true;
    }

    /// <summary>
    /// Method for loading saved game and its state from JSON file
    /// </summary>
    public async void LoadGame()
    {
        // Checking if save path exists
        if (!FileAccess.FileExists(savePath))
        {
            // TODO: in future scene shouldn't be changed
            GetTree().ChangeSceneToFile("res://Scenes/UI/Menus/Main/MainMenu.tscn");
            return;
        }
        FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, "nekompiliuoja");
        if (saveFile.GetError() != Error.Ok)
        {
            GD.PushError("Failed to load data!"); // for debuging purposes
        }
        PlayerSave = (Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());
        saveFile.Close();

        // Loading main scene
        LoadingForm = LoadingForms.Load;
        ChangeScene("res://Scenes/Maps/Main/Main.tscn");
    }

    /// <summary>
    /// Method for saving settings values such as resolution and volume.
    /// </summary>
    /// <param name="data">Dictionary for storing Resolution and volume bus values</param>
    /// <returns>False if failed to save else true</returns>
    public bool SaveSettings(Godot.Collections.Dictionary<string, Variant> data)
    {
        FileAccess saveFile = FileAccess.Open(settingsPath, FileAccess.ModeFlags.Write);

        if (saveFile.GetError() != Error.Ok)
        {
            GD.PushError("Failed to save settings!");
            return false;
        }

        saveFile.StoreLine(Json.Stringify(data));
        saveFile.Close();
        return true;
    }

    /// <summary>
    /// Method for loading resolution and settings values.
    /// </summary>
    public Dictionary<string, Variant> LoadSettings()
    {
        if (!FileAccess.FileExists(settingsPath))
            return null;

        FileAccess saveFile = FileAccess.Open(settingsPath, FileAccess.ModeFlags.Read);
        Dictionary<string, Variant> data = (Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());
        Vector2I size = new Vector2I((int)data["ResolutionX"], (int)data["ResolutionY"]);
        DisplayServer.WindowSetSize(size);
        AudioServer.SetBusVolumeDb(1, (int)data["MusicBusValue"]);
        AudioServer.SetBusVolumeDb(2, (int)data["SfxBusValue"]);
        saveFile.Close();
        return data;
    }

    /// <summary>
    /// Method to generate a random floating point between two numbers
    /// </summary>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <returns>floating point in between given values</returns>
    public static float GetRandomFloat(float min, float max)
    {
        Random rand = new Random();
        float range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }

    /// <summary>
    /// Method to get a random position within a set radius
    /// </summary>
    /// <param name="globalPosition">Player coordinates</param>
    /// <param name="limit">Radius limit</param>
    /// <returns>Vector2 object with position coordinates</returns>
    public static Vector2 GetRandomPositionWithinRadius(Vector2 globalPosition, int limit)
    {
        float x = GetRandomFloat(globalPosition.X - limit, globalPosition.X + limit);
        float y = GetRandomFloat(globalPosition.Y - limit, globalPosition.Y + limit);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Loads loading screen scene which is used to load a new scene and deletes itself after it's done.
    /// </summary>
    /// <param name="scenePath">Path to the next scene</param>
    public void ChangeScene(string scenePath)
    {
        // Deleting all nodes except for the globals
        Array<Node> nodes = GetNode<Node>("/root").GetChildren();
        foreach (Node node in nodes)
        {
            if (node != this)
            {
                node.QueueFree();
            }
        }
        // Loading screen is loaded.
        LoadingScreen loadingScreen = loadingScreenScene.Instantiate<LoadingScreen>();
        AddChild(loadingScreen);
        loadingScreen.LoadNewScene(scenePath);
    }
}