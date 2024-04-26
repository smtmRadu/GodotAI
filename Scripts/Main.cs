using Godot;
using System.Linq;
using TorchSharp;


public partial class Main : Node
{
	[Export] private Label fpsLabel;

	[Export] private Generic6DofJoint3D headJoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{


		var model = torch.nn.Sequential(
			torch.nn.Linear(42, 100),
			torch.nn.SELU(),
			torch.nn.Conv1d(1, 3, 3),
			torch.nn.BatchNorm2d(3),
			torch.nn.ReLU(),
			torch.nn.Linear(100, 10),
			torch.nn.Softmax(-1));

		

		// model.save("C:\\Users\\radup\\OneDrive\\Desktop\\Weights\\model_weights.pt");

		var paramn = model.parameters().Sum(x => x.numel());
		GD.Print(paramn);
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
