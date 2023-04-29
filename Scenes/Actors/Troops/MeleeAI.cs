using Godot;

public partial class MeleeAI : Node2D
{
    public enum State
    {
        Idle = 0,   // Waits for next event
        Engage = 1, // Engages the enemy
        Attack = 2  // Attacks the enemy
    }

    protected Area2D detectionZone;
    protected Area2D attackZone;

    private Actor target;
    private Infantry parent;
    private State currentState = State.Idle;
    private NavigationAgent2D navAgent;
    private TileMap map;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        parent = GetParent<Infantry>();
        attackZone = GetNode<Area2D>("AttackArea");
        target = GetNode<Actor>("/root/Main/Player");
        navAgent = parent.GetNode<NavigationAgent2D>("NavigationAgent2D");
        map = GetNode<TileMap>("/root/Main/TileMap");
        navAgent.SetNavigationMap(map.GetNavigationMap(0));
        navAgent.AvoidanceEnabled = true;
        if (target is not null)
        {
            currentState = State.Engage;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        navAgent.TargetPosition = target.GlobalPosition;

        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Engage:
                if (target is null || !IsInstanceValid(target)) // Check to see if we actualy have a target
                {
                    break;
                }
                Rotation = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();
                parent.Direction = parent.GlobalPosition.DirectionTo(navAgent.GetNextPathPosition());
                break;

            case State.Attack:
                Rotation = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();
                parent.Direction = Vector2.Zero;
                parent.Attack();
                break;
        }
    }

    /// <summary>
    /// Changes the state of the troop
    /// </summary>
    /// <param name="newState">New state to give the troop</param>
    private void ChangeState(State newState)
    {
        // Ignore if the state is already the same
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        if (currentState == State.Idle)
        {
            parent.Velocity = Vector2.Zero;
            CallDeferred("RefreshDetectionZone");
            return;
        }

        if (currentState == State.Attack)
        {
            parent.Velocity = Vector2.Zero;
            return;
        }
    }

    /// <summary>
    /// Check if the entered actor is not in the same team as the character
    /// and the character is not in the Attack state, then attack the actor
    /// </summary>
    /// <param name="body">Actor to check</param>
    private void AttackAreaBodyEntered(Node body)
    {
        // If the entered body is an actor and is not in the same team, attack
        if (body is Actor actor && parent.GetTeam() != actor.GetTeam() && currentState != State.Attack)
        {
            target = actor;
            ChangeState(State.Attack);
        }
    }

    /// <summary>
    /// Check if the exited actor is the target, then idle
    /// </summary>
    /// <param name="actor">Actor to check</param>
    private void AttackAreaBodyExited(Node body)
    {
        if (body is Actor actor && actor == target &&
            target != null && !IsQueuedForDeletion())
        {
            ChangeState(State.Engage);
        }
    }
}
