using Godot;
using ReinforcementLearning;
using TorchSharp;

/// <summary>
/// This script is only to see how it should be used.
/// </summary>
public partial class Walker : Agent3D
{
    [Export] BodyController3D bodyController;


    public override void _Setup()
    {
        ObservationShape = new long[] { 10 };
        ContinuousActions = 10;
        TrainerConfig = new PPO(3e-4f, 0.2f);
    }
    public override void OnEpisodeBegin()
    {
        // bodyController.Reset();
    }
    public override void CollectObservations(torch.Tensor state)
    {
        state = torch.randn(10);
    }

    public override void OnActionReceived(ActionVector action)
    {
        // var x = action.ContinuousActions[0];
    }
}
