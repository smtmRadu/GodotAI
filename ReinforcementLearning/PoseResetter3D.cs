using Godot;
using System.Collections.Generic;
using System.Linq;

namespace DeepGodot.ReinforcementLearning
{
    public class PoseReseter3D
    {
        private Node3D parent;
        private List<Transform3D> transformes;
        private List<RigidBody3D> rigidBodies;
        
        public PoseReseter3D(Node3D parent)
        {
            this.parent = parent;
            transformes = new List<Transform3D>();
            rigidBodies = new List<RigidBody3D>();
            GetAllTransforms(parent);
            GetAllRigidBodies(parent);
        }

        public void Reset()
        {
            int transformsStart = 0;
            ResetAllTransforms(parent, ref transformsStart);
            ResetAllRigidBodies();
        }

        private void GetAllTransforms(Node3D parent)
        {
             transformes.Add(parent.Transform);

             foreach (Node3D child in parent.GetChildren().OfType<Node3D>())
             {
                 GetAllTransforms(child);
             }     
        }

        private void GetAllRigidBodies(Node3D parent)
        {

            RigidBody3D rbs = parent.GetNodeOrNull<RigidBody3D>("RigidBody3D");

            if (rbs == null)
                return;

            rigidBodies.Add(rbs);
            foreach (Node3D child in parent.GetChildren().OfType<Node3D>())
            {
                GetAllRigidBodies(child);
            }
        }
        private void ResetAllTransforms(Node3D parent, ref int index)
        {
            if(parent is Node3D parent3D)
            {
                Transform3D initialTransform = transformes[index++];

                parent3D.Transform = initialTransform;

                foreach (Node3D child in parent.GetChildren())
                {
                    ResetAllTransforms(child, ref index);
                }
            }
            
        }

        private void ResetAllRigidBodies()
        {
            foreach (var rb in rigidBodies)
            {
                if (rb.Freeze)
                    continue;

                rb.LinearVelocity = Vector3.Zero;
                rb.AngularVelocity = Vector3.Zero;
            }
        }
    }
}
