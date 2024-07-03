using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace DungeonSlayer.Script.Test
{
    public class WithinSight : Conditional
    {
        public SharedTransform target;
        public float searchRange;

        public override TaskStatus OnUpdate()
        {
            var result = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
        
            foreach (var item in result)
            {
                if (item.transform != transform)
                {
                    target.Value = item.transform;
                    return TaskStatus.Success;
                }
            }

            target.Value = null;
            return TaskStatus.Failure;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.DrawWireSphere(transform.position, searchRange);
        }
    }
}