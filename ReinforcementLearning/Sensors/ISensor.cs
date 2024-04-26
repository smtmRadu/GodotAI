using Godot;
using System;
using TorchSharp;

namespace ReinforcementLearning
{
    public interface  ISensor 
    {
        public torch.Tensor GetObservations();
    }
}

