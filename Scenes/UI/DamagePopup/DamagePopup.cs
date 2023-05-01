namespace ChampionsOfTheRealm;

public partial class DamagePopup : Marker2D
{
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
        float sideMovementx = (rand.Next() % 81) - 61; //side movement variable
        float sideMovementy = (rand.Next() % 81) - 61; //side movement variable
        velocity = new Vector2(10, 0); //velocity vector
        tween = CreateTween();
        tween.TweenProperty(this, "scale", maxSize, 0.7f); //tween to scale the popup to 70%
        tween.Chain().TweenProperty(this, "scale", new Vector2(0.1f, 0.1f), 0.7f).SetDelay(0.3f); //tween to scale the vector with velocity
        tween.SetTrans(Tween.TransitionType.Linear); //trainsition type linear
        tween.SetEase(Tween.EaseType.Out); //ease type out
        tween.TweenCallback(new Callable(this, "TweenAllCompleted")); //callint method to remove the popup
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        parentRotation = parent.Rotation;
        Rotation = -parentRotation;
        Position -= velocity * Convert.ToSingle(delta);
    }

    /// <summary>
    /// Method for removing damage popup from screen when the animation is finished
    /// </summary>
    private void TweenAllCompleted() => QueueFree();
}