using Godot;
using System.Collections.Generic;
using System.Linq;

namespace ReinforcementLearning
{
    public partial class BodyController3D : Node
    {
        [Export] public float velocityScale = 300;
        [Export] public float forceLimit = 300;
        [Export] public Godot.Collections.Array<Node3D> bodyparts;

        private Dictionary<Node3D, BodyPart3D> bpsDict;
        public override void _Ready()
        {
            foreach (var part in bodyparts)
            {
                BodyPart3D bp = new BodyPart3D(part, velocityScale, forceLimit);
                bpsDict.Add(part, bp);
            }
        }

        public void Reset()
        {
            foreach (var item in bpsDict.Values)
            {
                item.SetAngularMotor_ForceLimit(0, 0, 0);
                item.SetAngularMotor_ForceLimit(0, 0, 0);
            }
        }
    }

    public class BodyPart3D
    {
        public Node3D Node { get; set; }
        public CollisionShape3D Collider { get; set; }
        public RigidBody3D Rb { get; private set; }
        public Generic6DofJoint3D Joint { get; private set; }

        private float velocity;
        private float force;
        public BodyPart3D(Node3D part, float velocity, float force)
        {
            Node = part;
            Joint = part.GetChild<Generic6DofJoint3D>(3);
            foreach (var item in part.GetChildren())
            {
                if (item is CollisionShape3D col)
                {
                    Collider = col;
                }
                else if (item is RigidBody3D rb)
                {
                    Rb = rb;
                }
            }

            this.velocity = velocity;
            this.force = force;
        }

     
        private float CurrentTargetVelocityX => Joint.GetParamX(Generic6DofJoint3D.Param.AngularMotorTargetVelocity);
        private float CurrentTargetVelocityY => Joint.GetParamY(Generic6DofJoint3D.Param.AngularMotorTargetVelocity);
        private float CurrentTargetVelocityZ => Joint.GetParamZ(Generic6DofJoint3D.Param.AngularMotorTargetVelocity);
        private float CurrentForceLimitX => Joint.GetParamX(Generic6DofJoint3D.Param.AngularMotorForceLimit);
        private float CurrentForceLimitY => Joint.GetParamY(Generic6DofJoint3D.Param.AngularMotorForceLimit);
        private float CurrentForceLimitZ => Joint.GetParamZ(Generic6DofJoint3D.Param.AngularMotorForceLimit);


        // GETTERS
        public Vector3 TargetVelocity => new Vector3(CurrentTargetVelocityX, CurrentTargetVelocityY, CurrentTargetVelocityZ);
        public Vector3 ForceLimit => new Vector3(CurrentForceLimitX, CurrentForceLimitY, CurrentForceLimitZ);
        public Vector3 Velocity => Rb.LinearVelocity;
        public Vector3 AngularVelocity => Rb.AngularVelocity;
        public Vector3 LocalPosition => Node.Position;

        // SETTERS
        public void SetAngularMotor_TargetVelocity(float x, float y, float z)
        {
            if(Joint == null)
            {
                throw new System.Exception("This bodypart doesn't have a joint to be adjusted");
            }
            Joint.SetParamX(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, x * velocity);
            Joint.SetParamY(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, y * velocity);
            Joint.SetParamZ(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, z * velocity);
        }
        public void SetAngularMotor_ForceLimit(float x, float y, float z)
        {
            if (Joint == null)
            {
                throw new System.Exception("This bodypart doesn't have a joint to be adjusted");
            }

            x = (x + 1f) / 2f;
            y = (y + 1f) / 2f;
            z = (z + 1f) / 2f;

            Joint.SetParamX(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, x * force);
            Joint.SetParamY(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, y * force);
            Joint.SetParamZ(Generic6DofJoint3D.Param.AngularMotorTargetVelocity, z * force);
        }
    }

}
