namespace ChampionsOfTheRealm;

/// <summary>
/// Class for handeling blood particles
/// </summary>
public partial class Blood : GpuParticles2D
{
    private Tween Tween { get; set; } // to interpolate opacity

    public override void _Ready()
    {
        Emitting = true;
        Tween = CreateTween();
        Tween.TweenProperty(this, "modulate:a", 0, 2.5f); // interpolation
        Tween.TweenCallback(new Callable(this, "TweenAllCompleted"));
    }

    /// <summary>
    /// Method for removing blood from scene when opacity reaches 0
    /// </summary>
    private void TweenAllCompleted() => QueueFree();
}