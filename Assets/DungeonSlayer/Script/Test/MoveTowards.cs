using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DungeonSlayer.Script.Gameplay;
using UnityEngine;

namespace DungeonSlayer.Script.Test
{
    public class MoveTowards : Action
    {
        public SharedTransform target;

        public override TaskStatus OnUpdate()
        {
            var origin = transform.position;
            var lookat = (target.GetValue() as Transform).position;
            lookat.y = origin.y;
            
            if ((lookat - origin).magnitude <= 0.1f)
            {
                return TaskStatus.Success;
            }
            
            transform.GetComponent<NetworkPlayerControllerTest>()
                .InputAsix(1.0f, 0, Quaternion.LookRotation(lookat - origin));
            
            return TaskStatus.Running;
        }
    }
}