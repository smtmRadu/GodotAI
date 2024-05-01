using Godot;

public partial class Main : Node
{
	[Export] private Generic6DofJoint3D joint;
	[Export] private Label fpsLabel;

	[Export] private Generic6DofJoint3D headJoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		fpsLabel.Text = $"FPS: {1/delta}";
	}
}
