using Godot;
using System.Collections.Generic;
using System.Linq;
using TorchSharp;
using static TorchSharp.torch;

namespace ReinforcementLearning
{
    public partial class StateSequenceBuffer
    {
        private LinkedList<Tensor> states;
        private long[] shape;
        private int stackSize;

        public StateSequenceBuffer(long[] input_shape, int sequence_size = 1)
        {
            states = new LinkedList<Tensor>();
            this.shape = input_shape.ToArray();
            this.stackSize = sequence_size;
            Reset();
        }

        public void Add(Tensor state)
        {
            if (!state.shape.SequenceEqual(this.shape))
                throw new System.ArgumentException("Incorrect input shape");

            states.RemoveFirst();

            if (stackSize == 1)
                states.AddLast(state);
            else
                states.AddLast(state.unsqueeze(0));
        }
        
        public Tensor GetStateSequence()
        {
            if (stackSize == 1)
                return states.First();
            else
                return torch.cat(states.ToArray());
        }

        public void Reset()
        {
            states.Clear();
            for (int i = 0; i < stackSize; i++)
            {
                states.AddLast(zeros(shape));
            }
        }

    }

}