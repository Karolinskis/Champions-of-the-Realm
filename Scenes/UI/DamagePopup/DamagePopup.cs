using Godot;
using System;

public partial class DamagePopup : Marker2D
{
    // Called when the node enters the scene tree for the first time.

    private Label label;
    private Tween tween;
    public int Amount { get; set; } = 0;
    public string Type { get; set; } = "";
    private Vector2 velocity = Vector2.Zero;
    private float parentRotation;
    private Color healColor = new Color("2eff27");
    private Color damageColor = new Color("c11f1f");
    Vector2 maxSize = new Vector2(1, 1);
    private CharacterBody2D parent;
    public override void _Ready()
    {
        label = GetNode<Label>("FloatingText");
        label.Text = Amount.ToString();
        parent = GetParent() as CharacterBody2D;
        switch (Type)
        {
            case "Heal":
                label.Set("theme_override_colors/font_color", healColor);
                break;
            case "Damage":
                label.Set("theme_override_colors/font_color", damageColor);
                break;
            case "Critical":
                label.Set("theme_override_colors/font_color", damageColor);
                break;
        }
        Random rand = new Random();
        float sideMovementx = (rand.Next() % 81) - 61;
        float sideMovementy = (rand.Next() % 81) - 61;
        velocity = new Vector2(10, 0);
        tween = CreateTween();
        tween.TweenProperty(this, "scale", maxSize, 0.7f);
        tween.Chain().TweenProperty(this, "scale", new Vector2(0.1f, 0.1f), 0.7f).SetDelay(0.3f);
        tween.SetTrans(Tween.TransitionType.Linear);
        tween.SetEase(Tween.EaseType.Out);
        tween.TweenCallback(new Callable(this, "TweenAllCompleted"));
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        parentRotation = parent.Rotation;
        Rotation = -parentRotation;
        Position -= velocity * Convert.ToSingle(delta);
    }
    private void TweenAllCompleted()
    {
        QueueFree();
    }
}