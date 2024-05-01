using System.Collections.Generic;

namespace ReinforcementLearning
{
    public abstract class Trainer
    {
        private static Trainer Instance;
        public List<Agent3D> agents;


        public static void Subscribe(Agent3D agent)
        {
            if (Instance == null)
            {
                if (agent.Model.Trainer.GetType() == typeof(PPO))
                    Instance = new PPOTrainer();
                
                Instance.agents = new();

            }

            Instance.agents.Add(agent);
        }

        public abstract void Train();

    }

}
