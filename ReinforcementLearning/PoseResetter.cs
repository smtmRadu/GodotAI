using Godot;
using System.Collections.Generic;

namespace DeepGodot.ReinforcementLearning
{
    public class PoseReseter
    {
        private Node parent;
        private List<Transform3D> transformes;
        private List<Transform2D> transformes2D;
        private List<RigidBody3D> rigidBodies;
        private List<RigidBody2D> rigidbodies2D;
        
        // private BodyController bodyController;

        public PoseReseter(Node parent)
        {
            this.parent = parent;
            transformes = new List<Transform3D>();
            rigidBodies = new List<RigidBody3D>();
            // bodyController = parent.GetNode<BodyController>("BodyController");
            GetAllTransforms(parent);
            GetAllRigidBodies(parent);
        }

        public void Reset()
        {
            int transformsStart = 0;
            ResetAllTransforms(parent, ref transformsStart);
            ResetAllRigidBodies();
            // bodyController?.bodyPartsList.ForEach(part =>
            // {
            //     part.CurrentStrength = 0f;
            //     part.CurrentNormalizedStrength = 0f;
            //     part.CurrentEulerRotation = new Vector3();
            //     part.CurrentNormalizedEulerRotation = new Vector3();
            // });
        }

        private void GetAllTransforms(Node parent)
        {
            if(parent is Node3D parent3D)
            {
                transformes.Add(parent3D.Transform);

                foreach (Node child in parent.GetChildren())
                {
                    GetAllTransforms(child);
                }

            }
            else if(parent is Node2D parent2D)
            {
                // to be implemented;
            }
        

            
        }

        private void GetAllRigidBodies(Node parent)
        {
            if(parent is Node3D parent3d)
            {
                RigidBody3D rbs = parent3d.GetNode<RigidBody3D>("RigidBody3D");
                rigidBodies.Add(rbs);
                foreach (Node child in parent.GetChildren())
                {
                    GetAllRigidBodies(child);
                }
            }
            else
            {
                throw new System.NotImplementedException();
            }
          
        }
        private void ResetAllTransforms(Node parent, ref int index)
        {
            if(parent is Node3D parent3D)
            {
                Transform3D initialTransform = transformes[index++];

                parent3D.Transform = initialTransform;

                foreach (Node child in parent.GetChildren())
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

                rb.LinearVelocity = new Vector3();
                rb.AngularVelocity = new Vector3();
            }
            foreach (var rb in rigidbodies2D)
            {
                if (rb.Freeze)
                    continue;

                rb.LinearVelocity = new Vector2();
                rb.AngularVelocity = 0f;
            }
        }
    }
}
