using Godot;
using System;

public partial class Coin : CharacterBody2D, IPoolable
{
    [Signal] public delegate void CoinRemovedEventHandler(Coin coin); // coin removed handler.

    /// <summary>
    /// Current coin value.
    /// </summary>
    /// <value>Coin value</value>
    [Export] public int Gold { get; set; } = 0; // coin value
    private Vector2 movementDirection = new Vector2(-100, -100); // coin movement direction
    private Area2D takeArea; // coin pickup area
    private Area2D slideArea; // coin slide area
    private Timer timer; // coin removed after certain amount of time.
    private Tween tween; // coin magnet.
    public override void _Ready()
    {
        takeArea = GetNode<Area2D>("Area2DTake");
        slideArea = GetNode<Area2D>("Area2DSlide");
        timer = GetNode<Timer>("Timer");
    }

    /// <summary>
    /// Method for making coin move.
    /// </summary>
    private void Move()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "global_position", movementDirection, 0.2f);
    }

    /// <summary>
    /// Method for removing coin if it wasn't picked up in time.
    /// </summary>
    private void TimerTimeout()
    {
        RemoveFromScene();
    }

    /// <summary>
    /// Method for when coin reaches player.
    /// </summary>
    /// <param name="body">Player body</param>
    private void Area2DBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.GetGold(Gold);
            RemoveFromScene();
        }
    }

    /// <summary>
    /// Method for starting to drag coin to player when in range.
    /// </summary>
    /// <param name="body">Player body</param>
    private void Area2DSlideBodyEntered(Node body)
    {
        if (body is Player player)
        {
            movementDirection = player.GlobalPosition;
            Move();
        }
    }

    /// <summary>
    /// Method for stopping magnet when coin reaches player.
    /// </summary>
    /// <param name="body">Player body</param>
    private void Area2DSlideBodyExited(Node body)
    {
        if (body is Player)
        {
            movementDirection = GlobalPosition;
            tween.Stop();
        }
    }

    /// <summary>
    /// Method for adding coin to scene.
    /// </summary>
    public void AddToScene()
    {
        movementDirection = new Vector2(Globals.GetRandomFloat(-25, 25), Globals.GetRandomFloat(-25, 25));
        timer.Start();
        Move();
        Show();
    }

    /// <summary>
    /// Method for removing coin from scene.
    /// </summary>
    public void RemoveFromScene()
    {
        Hide();
        if (tween != null) // TODO
        {
            tween.Stop();
        }
        timer.Stop();
        GlobalPosition = new Vector2(-100, -100);
        movementDirection = GlobalPosition;
        EmitSignal(nameof(CoinRemoved), this);
    }
}