using Godot;
using System;
using static TorchSharp.torch;

namespace ReinforcementLearning
{
    public class TimestepTuple : ICloneable
    {
        public int index;

        /// <summary>
        /// (normalized) state
        /// </summary>
        public Tensor state;
        /// <summary>
        /// (normalized) next state
        /// </summary>
        public Tensor nextState;

        /// <summary>
        /// raw continuous actions, unsquashed by Tanh
        /// </summary>
        public Tensor action_continuous_unsquashed;
        public Tensor prob_continuous;
        /// <summary>
        /// one hot embedded of discrete action
        /// </summary>
        public Tensor action_discrete_onehot;
        public Tensor prob_discrete;

        public float reward;
        public int done;

        public Tensor advantage;
        public Tensor value_target;
        public Tensor q_target;


        public TimestepTuple(int index)
        {
            this.index = index;
            done = 0;
            reward = 0;
        }
        private TimestepTuple() { }
        public object Clone()
        {
            TimestepTuple clone = new TimestepTuple();

            clone.index = index;
            clone.state = state;
            clone.nextState = nextState;
            clone.action_continuous_unsquashed = action_continuous_unsquashed;
            clone.action_discrete_onehot = action_discrete_onehot;
            clone.reward = reward;
            clone.prob_continuous = prob_continuous;
            clone.prob_discrete = prob_discrete;
            clone.advantage = advantage;
            clone.value_target = value_target;
            clone.done = done;

            return clone;
        }
    }
}
