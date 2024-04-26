using DeepGodot.ReinforcementLearning;
using Godot;
using System;
using TorchSharp;

namespace ReinforcementLearning
{
    public partial class Agent : Node
    {
        [Export] public Behaviour Model { get; set; }
        [Export] public BehaviourType Behaviour { get; set; } = BehaviourType.Learning;
        [Export] public OnEpisodeEndType OnEpisodeEnd { get; set; } = OnEpisodeEndType.ResetAgent;
        // [Export] public UseSensorsType UseSensors { get; set; } = UseSensorsType.On;
        [Export] public DeviceType Device { get; set; } = DeviceType.CPU;
        [Export] public int MaxStep { get; set; } = 1000;


        StateSequenceBuffer stateBuffer;
        TimestepTuple timestep;
        MemoryBuffer memory;
        PoseReseter poseReseter;
        ISensor[] sensors;
        int episodeTimesteps = 0;
        float episodeCumulatedReward = 0f;
        
        public override void _Ready()
        {
            if (Model == null) return;

            Setup();

            Model.Initialize();

            stateBuffer = new StateSequenceBuffer(Model.ObservationsShape, Model.StateStack);
            memory = new MemoryBuffer();

            if (OnEpisodeEnd == OnEpisodeEndType.ResetAgent)
                poseReseter = new PoseReseter(this);
            else if (OnEpisodeEnd == OnEpisodeEndType.ResetEnvironment)
                poseReseter = new PoseReseter(this.GetParent());

            OnEpisodeBegin();
            
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _PhysicsProcess(double delta)
        {
            if (Model == null) return;

            CollectObservations(timestep.state);
            stateBuffer.Add(timestep.state);
            Model.Requires_Grad = false;
            Model.Forward(stateBuffer.GetStateSequence(), out timestep.action_continuous_unsquashed, out timestep.prob_continuous, out timestep.action_discrete_onehot, out timestep.prob_discrete); 
            ActionVector actionVec = new ActionVector(timestep.action_continuous_unsquashed, timestep.action_discrete_onehot);
            OnActionReceived(actionVec);

            // Memorize
            if (episodeTimesteps == MaxStep && MaxStep > 0)
                EndEpisode();

            if(Behaviour == BehaviourType.Learning)
                CollectObservations(timestep.nextState);

            episodeCumulatedReward += timestep.reward;

            if(timestep.done == 1)
            {
                stateBuffer.Reset();
                episodeTimesteps = 0;
                episodeCumulatedReward = 0f;
                poseReseter?.Reset();
                OnEpisodeBegin();
            }

        }


        public virtual void Setup()
        {
           
        }
        public virtual void OnEpisodeBegin() { }
        public virtual void CollectObservations(torch.Tensor state) { }
        public virtual void OnActionReceived(ActionVector action) { }
        public void EndEpisode()
        {
            timestep.done = 1;
        }
        public void AddReward(float reward)
        {
            timestep.reward += reward;
        }
    }





    public enum BehaviourType
    {
        Off,
        Inference,
        Learning,
    }
    public enum OnEpisodeEndType
    {
        Nothing,
        ResetAgent,
        ResetEnvironment
    }

    public enum UseSensorsType
    {
        Off,
        On
    }

    public enum DeviceType
    {
        CPU,
        GPU
    }
}
