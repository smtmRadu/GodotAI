using Godot;
using System;
using System.Linq;
using TorchSharp;

namespace ReinforcementLearning
{
    public partial class ActionVector
    {
        /// <summary>
        /// The index of the discrete action of value in range [0, <em>Discrete Actions - 1]</em>. If no Discrete Actions are used, this will be equal to -1.
        /// </summary>
        public int DiscreteAction { get; set; }
        /// <summary>
        /// A vector of Length <em>Continuous Actions</em> containing values in range [-1, 1]. If no Continuous Actions are used, this array is null.
        /// </summary>
        public float[] ContinuousActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="continuous">unnormalized continuous values</param>
        /// <param name="discrete">The onehotvec</param>
        public ActionVector(torch.Tensor continuous, torch.Tensor discrete)
        {
            DiscreteAction = discrete.argmax(dim: -1).item<int>();
            ContinuousActions = continuous.tanh().data<float>().ToArray();
        }

        public void Clear()
        {
            DiscreteAction = -1;
            ContinuousActions = ContinuousActions?.Select(x => 0f).ToArray();
        }
        public override string ToString()
        {
            return $"[Continuous Actions [{ContinuousActions?.Join()}] | Discrete Action [{DiscreteAction}]]";
        }
    }
}

