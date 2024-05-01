using System;

namespace ReinforcementLearning
{
    public interface ITrainerConfig { }

    [Serializable]
    public class PPO : ITrainerConfig
    {
        public float learningRate = 3e-4f;
        public float epsilon = 0.2f;

        public PPO(float lr = 3e-4f, float epsilon = 0.2f)
        {
            this.learningRate = lr;
            this.epsilon = epsilon;
        }


    }
    [Serializable]
    public class SAC : ITrainerConfig
    {
        public float learningRate;
    }

}

