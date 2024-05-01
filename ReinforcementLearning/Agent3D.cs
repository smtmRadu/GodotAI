using DeepGodot.ReinforcementLearning;
using Godot;
using System.IO;
using TorchSharp;


namespace ReinforcementLearning
{
    public partial class Agent3D : Node3D
    {
        [Export] public Model Model { get; set; }
        [Export] public BehaviourType Behaviour { get; set; } = BehaviourType.Learning;
        [Export] public OnEpisodeEndType OnEpisodeEnd { get; set; } = OnEpisodeEndType.ResetAgent;
        [Export] public DeviceType Device { get; set; } = DeviceType.CPU;
        [Export] public int MaxStep { get; set; } = 1000;  

        StateSequenceBuffer stateBuffer;
        TimestepTuple timestep;
        MemoryBuffer memory;
        PoseReseter3D poseReseter;
        int episodeStepsCount = 0;
        float episodeCumulatedReward = 0f;



        /// SETUP -------------------
        public ITrainerConfig TrainerConfig;
        public string ModelName { get; set; } = "NewModel";
        public long[] ObservationShape { get; set; }
        public int StateStack { get; set; } = 1;
        public int ContinuousActions { get; set; } = 0;
        public int DiscreteActions { get; set; } = 0;
        public int HiddenSize { get; set; } = 128;
        public int LayersNum { get; set; } = 2;
        public bool Normalize { get; set; } = true;
       
        //- ----------------------------
        public override void _Ready()
        {
            _Setup();

            if (Model == null)
            {
                if (File.Exists($"res://{ModelName}.tres"))
                    Model = ResourceLoader.Load($"res://{ModelName}.tres") as Model;         
                else 
                    Model = new Model(TrainerConfig, Name, ObservationShape, StateStack, ContinuousActions, DiscreteActions, HiddenSize, LayersNum, Normalize);
            }


            Model._Setup();
            Model._Save();

           

            stateBuffer = new StateSequenceBuffer(Model.ObservationShape, Model.StateStack);
            memory = new MemoryBuffer();
            timestep = new TimestepTuple(episodeStepsCount);
            if (OnEpisodeEnd == OnEpisodeEndType.ResetAgent)
                poseReseter = new PoseReseter3D(this);
            else if (OnEpisodeEnd == OnEpisodeEndType.ResetEnvironment)
                poseReseter = new PoseReseter3D(this.GetParent() as Node3D);

            OnEpisodeBegin();
            
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _PhysicsProcess(double delta)
        {
            if (Model == null) return;

            // Memorize
            if (episodeStepsCount == MaxStep && MaxStep > 0)
                EndEpisode();

            if(episodeStepsCount > 0)
            {
                if (Behaviour == BehaviourType.Learning)
                {
                    CollectObservations(timestep.nextState);
                    timestep.nextState = Model.Normalize(timestep.nextState);
                }
                episodeCumulatedReward += timestep.reward;

                if (timestep.done == 1)
                {
                    stateBuffer.Reset();
                    episodeStepsCount = 0;
                    episodeCumulatedReward = 0f;
                    poseReseter?.Reset();
                    OnEpisodeBegin();
                }
                
            }         

            // Act
            CollectObservations(timestep.state);
            timestep.state = Model.UpdateThenNormalize(timestep.state);
            stateBuffer.Add(timestep.state);
            Model.Requires_Grad = false;
            Model.Forward(stateBuffer.GetStateSequence(), out timestep.action_continuous_unsquashed, out timestep.prob_continuous, out timestep.action_discrete_onehot, out timestep.prob_discrete); 
            ActionVector actionVec = new ActionVector(timestep.action_continuous_unsquashed, timestep.action_discrete_onehot);
            OnActionReceived(actionVec);

        }

        /// <summary>
        /// To setup:<br></br>
        /// ObservationShape <br></br>
        /// Actions<br></br>
        /// Trainer <br></br>
        /// </summary>
        public virtual void _Setup()
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

    public enum DeviceType
    {
        CPU,
        GPU
    }
}
