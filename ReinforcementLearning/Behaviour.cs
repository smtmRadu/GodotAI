using Godot;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Linq;
using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch.nn;

namespace ReinforcementLearning
{
    public partial class Behaviour : Resource
    {
        [Export] public long[] ObservationsShape { get; set; }
        [Export] public int StateStack { get; set; }
        [Export] public int ContinuousActions { get; set; }
        [Export] public int DiscreteActions { get; set; }
        [Export] public bool Normalize { get; set; }

        [Export] public string VNetworkPath;
        [Export] public string MuNetworkPath;
        [Export] public string SigmaNetworkPath;

        public Normalizer normalizer;

        private Sequential vnet;
        private Sequential munet;
        private Sequential sigmanet;


        private bool _computes_grad;
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
        public void Initialize()
        {
            const int hiddenSize = 128;

            vnet = Sequential(
                ("lin1", Linear(ObservationsShape.Last(), hiddenSize)),
                ("act1", Tanh()),
                ("lin2", Linear(hiddenSize, hiddenSize)),
                ("act2", Tanh()),
                ("lin3", Linear(hiddenSize, 1))
                );

            vnet.load(VNetworkPath);

            munet = Sequential(
               ("lin1", Linear(ObservationsShape.Last(), hiddenSize)),
               ("act1", Tanh()),
               ("lin2", Linear(hiddenSize, hiddenSize)),
               ("act2", Tanh()),
               ("lin3", Linear(hiddenSize, ContinuousActions))
               );

            munet.load(MuNetworkPath);


            sigmanet = Sequential(
             ("lin1", Linear(ObservationsShape.Last(), hiddenSize)),
             ("act1", Tanh()),
             ("lin2", Linear(hiddenSize, hiddenSize)),
             ("act2", Tanh()),
             ("lin3", Linear(hiddenSize, ContinuousActions)),
             ("act3", Softplus())
             );
            

            sigmanet.load(SigmaNetworkPath);
        }
        
        public void Forward(torch.Tensor state, out torch.Tensor continuous_unnormalized, out torch.Tensor probs_continuous, out torch.Tensor discrete, out torch.Tensor probs_discrete)
        {
            if(ContinuousActions > 0)
            {
                var means = munet.forward(state);
                var stdvs = munet.forward(state);

                continuous_unnormalized = torch.normal(means, stdvs);
                probs_continuous = 1 / (stdvs * MathF.Sqrt(2f * MathF.PI)) * torch.exp(-(continuous_unnormalized - means).square() / (2 * stdvs.square()));
            }
            else
            {
                continuous_unnormalized = null;
                probs_continuous = null;
            }
            if(DiscreteActions > 0)
            {
               
            }
            discrete = null;
            probs_discrete = null;
        }
    }

    public class Normalizer
    {

    }
   
}

