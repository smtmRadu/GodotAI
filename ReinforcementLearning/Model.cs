using Godot;
using System;
using System.Linq;
using TorchSharp;
using TorchSharp.Modules;

namespace ReinforcementLearning
{
    public partial class Model : Resource
    {
        public ITrainerConfig Trainer;
        [Export] public string Name { get; set; } = "NewModel";
        [Export] public long[] ObservationShape { get; set; }
        [Export] public int StateStack { get; set; } = 1;
        [Export] public int ContinuousActions { get; set; } = 0;
        [Export] public int DiscreteActions { get; set; } = 0;
        [Export] public int HiddenSize { get; set; } = 128;
        [Export] public int LayersNum { get; set; } = 2;
        [Export] public bool NormalizeObservations { get; set; } = true;


        // Normalizer
        [Export] private int Steps { get; set; } = 0;
        private float[] MeanSerialized { get; set; } = null;
        private float[] M2Serialized { get; set; } = null;
        torch.Tensor Mean;
        torch.Tensor M2;


        // Networks
        [Export] private string vNetSerialized { get; set; }
        [Export] private string muNetSerialized {  get; set; }
        [Export] private string sigmaNetSerialized { get; set; }

        private Sequential vnet;
        private Sequential munet;
        private Sequential sigmanet;




        /// <summary>
        /// Caching for grad computation is turned off on inference. Turned on only on policy update
        /// </summary>
        private bool _computes_grad = false;
        public bool Requires_Grad
        {
            private get => _computes_grad;
            set
            {
                if(value == true && _computes_grad == false)
                {
                    foreach (var p in vnet.parameters())
                        p.requires_grad = true;
                    foreach (var p in munet.parameters())
                        p.requires_grad = true;
                    foreach(var p in sigmanet.parameters()) 
                        p.requires_grad = true;
                    _computes_grad = true;
                }
                if(value == false && _computes_grad == true)
                {
                    foreach (var p in vnet.parameters())
                        p.requires_grad = false;
                    foreach (var p in munet.parameters())
                        p.requires_grad = false;
                    foreach (var p in sigmanet.parameters())
                        p.requires_grad = false;
                    _computes_grad = false;
                }
            }
        }

        public Model(ITrainerConfig trainer, string name, long[] observationShape, int stateStack, int continuousActions, int discreteActions, int hiddenSize, int layersNum, bool normalizeObservations)
        {
            Trainer = trainer;
            Name = name;
            ObservationShape = observationShape;
            StateStack = stateStack;
            ContinuousActions = continuousActions;
            DiscreteActions = discreteActions;
            HiddenSize = hiddenSize;
            LayersNum = layersNum;
            NormalizeObservations = normalizeObservations;
        }
       
        
        public void _Setup()
        {
            long lengthy = ObservationShape.Aggregate(1L, (t, n) => t * n);
            MeanSerialized = new float[lengthy];
            M2Serialized = new float[lengthy];
            Mean = torch.tensor(MeanSerialized).view(ObservationShape);
            M2 = torch.tensor(M2Serialized).view(ObservationShape);

            // still Checking if i can serialize the nets as string and that's all.
            // Probably i will create a temporary file to write , copy the contents and then delete it:)


            // vnet = Sequential(
            //     ("lin1", Linear(ObservationShape.Last(), hiddenSize)),
            //     ("act1", Tanh()),
            //     ("lin2", Linear(hiddenSize, hiddenSize)),
            //     ("act2", Tanh()),
            //     ("lin3", Linear(hiddenSize, 1))
            //     );
            // 
            // vstate = vnet.state_dict();
            // //  vnet.load(VNetworkPath);
            // 
            // 
            // munet = Sequential(
            //    ("lin1", Linear(ObservationShape.Last(), hiddenSize)),
            //    ("act1", Tanh()),
            //    ("lin2", Linear(hiddenSize, hiddenSize)),
            //    ("act2", Tanh()),
            //    ("lin3", Linear(hiddenSize, ContinuousActions))
            //    );
            // 
            // //  munet.load(MuNetworkPath);
            // 
            // 
            // sigmanet = Sequential(
            //  ("lin1", Linear(ObservationShape.Last(), hiddenSize)),
            //  ("act1", Tanh()),
            //  ("lin2", Linear(hiddenSize, hiddenSize)),
            //  ("act2", Tanh()),
            //  ("lin3", Linear(hiddenSize, ContinuousActions)),
            //  ("act3", Softplus())
            //  );
            // 
            // 
            // //  sigmanet.load(SigmaNetworkPath);
        }
        public void Forward(torch.Tensor state, out torch.Tensor continuous_unnormalized, out torch.Tensor probs_continuous, out torch.Tensor discrete, out torch.Tensor probs_discrete)
        {
            if(ContinuousActions > 0)
            {
                var means = munet.forward(state);
                var stdvs = sigmanet.forward(state);

                continuous_unnormalized = torch.normal(means, stdvs);
                probs_continuous = 1f / (stdvs * MathF.Sqrt(2f * MathF.PI)) * torch.exp(-(continuous_unnormalized - means).square() / (2 * stdvs.square()));
            }
            else
            {
                continuous_unnormalized = null;
                probs_continuous = null;
            }



            if(DiscreteActions > 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                discrete = null;
                probs_discrete = null;
            }
           
        }
        public torch.Tensor UpdateThenNormalize(torch.Tensor state)
        {
            Steps++;

// one of these is null...

            var delta1 = state - Mean;
            Mean += delta1 / Steps;
            var delta2 = state - Mean;
            M2 += delta1 * delta2;

            return Normalize(state);
        }
        public torch.Tensor Normalize(torch.Tensor state)
        {
            // Update
            if (Steps <= 1)
                return state.clone();

            var variance = M2 / (Steps - 1);
            return (state - Mean) / (variance + 1e-10f);
        }
        public Error _Save()
        {
            MeanSerialized = Mean.data<float>().ToArray();
            M2Serialized = M2.data<float>().ToArray();
            return ResourceSaver.Save(this, $"res://{Name}.tres");
        }
    }   
}

