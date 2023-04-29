namespace ChampionsOfTheRealm;

/// <summary>
/// Class for handeling blood particles
/// </summary>
public partial class Blood : GpuParticles2D
{
    private Tween tween; // to interpolate opacity

    public override void _Ready()
    {
        Emitting = true;
        tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, 2.5f); // interpolation
        tween.TweenCallback(new Callable(this, "TweenAllCompleted"));
    }

    /// <summary>
    /// Method for removing blood from scene when opacity reaches 0
    /// </summary>
    private void TweenAllCompleted()
    {
        QueueFree();
    }
}