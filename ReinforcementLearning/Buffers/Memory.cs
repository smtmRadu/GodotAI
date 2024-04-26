using Godot;
using System;
using System.Collections.Generic;

namespace ReinforcementLearning
{
    public partial class MemoryBuffer
    {
        public int Count { get => frames.Count; }
        public List<TimestepTuple> frames { get; private set; } = new();

        public void Add(TimestepTuple timestep)
        {
            frames.Add(timestep);
        }
        public void Clear()
        {
            frames.Clear();
        }
    }

}
