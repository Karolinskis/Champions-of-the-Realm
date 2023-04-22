using Godot;
using Godot.Collections;
using System;

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

    public Dictionary<string, Variant> Player { get; set; } // For transfering player between scenes

    PackedScene loadingScreenScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Loading/LoadingScreen.tscn"); // Loading scene resource

    /// <summary>
    /// Method which saves all nodes in Persist group by parsing dictionary to JSON file
    /// </summary>
    /// <returns>Returns true, if saved successfully, otherwise false</returns>
    public bool SaveGame()
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
        Array<Node> saveNodes = GetTree().GetNodesInGroup("Persist");
        foreach (Node saveNode in saveNodes)
        {
            // Checking if can be saved
            if (!saveNode.HasMethod("Save") || saveNode.Name.IsEmpty)
            {
                continue;
            }
            // Getting a dictionary of data about every node by calling their save method
            Variant data = saveNode.Call("Save");
            GD.Print(data); // for debugging purposes
            saveFile.StoreLine(Json.Stringify(data)); // parsing dictionary to JSON file
        }
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
        // getting all unwanted save nodes in current scene
        Array<Node> saveNodes = GetTree().GetNodesInGroup("Presist");
        foreach (Node saveNode in saveNodes)
        {
            saveNode.QueueFree(); // removing all unwanted save nodes
        }
        GetTree().CurrentScene.QueueFree(); // Big bug
        if (saveNodes.Count > 0)
        {
            // Giving small amount of time to fully remove all unwanted nodes (It takes time to free from memory)
            await ToSignal(GetTree().CreateTimer(0.00001f), "timeout"); // TODO: implement better solution, without async
        }
        FileAccess saveFile = FileAccess.OpenEncryptedWithPass(savePath, FileAccess.ModeFlags.Read, "nekompiliuoja");
        if (saveFile.GetError() != Error.Ok) // Checking if saveFile was loaded without any errors
        {
            GD.PushError("Failed to load data!"); // for debuging purposes
        }
        while (saveFile.GetPosition() < saveFile.GetLength()) // reading every JSON line
        {
            // Loading data about saved nodes and parsing it to dictionaries
            Dictionary<string, Variant> data = (Dictionary<string, Variant>)Json.ParseString(saveFile.GetLine());

            // Creating Persist node, adding to scene and loading data
            PackedScene newObjectScene = ResourceLoader.Load<PackedScene>(data["Filename"].ToString());
            Node newObject = newObjectScene.Instantiate();
            newObject.Set("position", new Vector2((float)data["PosX"], (float)data["PosY"]));
            GetNode(data["Parent"].ToString()).AddChild(newObject);
            newObject.Call("Load", data);
        }
        saveFile.Close();
        // Calling method to load player inside scene
        GetTree().CurrentScene.Call("LoadSavedPlayer");
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
        {
            return null;
            //GD.PushError("Settings save file doesn't exist!");
        }
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
    /// Loads loading screen scene which is used to load a new scene and deletes itself after it's done.
    /// </summary>
    /// <param name="scenePath">Path to the next scene</param>
    public void ChangeScene(string scenePath)
    {
        // Loading screen is loaded.
        LoadingScreen loadingScreen = loadingScreenScene.Instantiate<LoadingScreen>();
        AddChild(loadingScreen);
        loadingScreen.LoadNewScene(scenePath);
    }
}