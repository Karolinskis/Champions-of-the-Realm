namespace ChampionsOfTheRealm;

public partial class LoadingScreen : Control
{
    private Control control;    // control node
    private ProgressBar loadingBar; // loading bar node
    private AnimationPlayer animationPlayer;    // animation player node
    private string nextScene = ""; // stores next scene path

    public override void _Ready()
    {
        control = GetNode<Control>("CanvasLayer/Control/");
        loadingBar = GetNode<ProgressBar>("CanvasLayer/Control/TextureRect/CenterContainer/PanelContainer/VBoxContainer/ProgressBar");
        animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/LoadingAnimation");
        animationPlayer.Play("TransIn");
    }

    /// <summary>
    /// Animates the loading bar.
    /// </summary>
    /// <param name="progress">max progress</param>
    /// <param name="duration">duration of the tween.</param>
    private void AnimateLoadingBar(float progress, double duration)
    {
        Tween loadingTween = CreateTween();
        loadingTween.SetTrans(Tween.TransitionType.Expo);
        loadingTween.SetEase(Tween.EaseType.Out);
        loadingTween.TweenMethod(new Callable(this, "ChangeLoadingBarValue"), loadingBar.Value, progress, duration);
    }

    /// <summary>
    /// Changes value of the loading bar.
    /// </summary>
    /// <param name="value">new loading bar value.</param>
    private void ChangeLoadingBarValue(int value) => loadingBar.Value = value;

    /// <summary>
    /// Loads a new scene.
    /// </summary>
    /// <param name="scenePath">New scene path</param>
    public void LoadNewScene(string scenePath)
    {
        nextScene = scenePath;
        AnimateLoadingBar(95, 1.5);
    }

    /// <summary>
    /// Loads new scene bit by bit, allowing the use of a loading bar to show progress.
    /// </summary>
    private void InitializeResourceLoader()
    {
        ResourceLoader.LoadThreadedRequest(nextScene);  // Begin loading
        ResourceLoader.ThreadLoadStatus sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);

        // Checking if scene was cached to avoid loading it again.
        if (ResourceLoader.HasCached(nextScene) || sceneLoadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            AddScene();
            return;
        }
        while (sceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);
            // Loads the new scene once everything has been loaded.
            switch (sceneLoadStatus)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                    break;

                case ResourceLoader.ThreadLoadStatus.Loaded:
                    AddScene();
                    break;

                case ResourceLoader.ThreadLoadStatus.Failed:
                    GD.PrintErr("Resource loading failed.");
                    return;

                case ResourceLoader.ThreadLoadStatus.InvalidResource:
                    GD.PrintErr("Cannot load, invalid resource.");
                    return;

                default:
                    GD.PrintErr("State doesn't exist.");
                    return;
            }
        }
    }

    private void AddScene()
    {
        PackedScene loadedScene = ResourceLoader.LoadThreadedGet(nextScene) as PackedScene;
        Node newRootNode = loadedScene.Instantiate<Node>();
        GetNode("/root").AddChild(newRootNode);
        AnimateLoadingBar(100, 1);
        animationPlayer.Play("TransOut");
    }

    /// <summary>
    /// Deletes this scene.
    /// </summary>
    public void DeleteScene()
    {
        QueueFree();
    }
}
