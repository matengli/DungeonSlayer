using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace DungeonSlayer.Script.BehaviourTree
{
    public class BTActionWonderAround : Action
    {
        private Vector3 dest;
        [SerializeField] private float MaxTime = 0;
        public SharedTransform target;

        public override void OnAwake()
        {
            dest = UnityEngine.Random.insideUnitSphere * 5.0f;
            transform.GetComponentInParent<ActorMgr>().RPC_MoveToPosition(dest, 0.1f);
        }

        public override TaskStatus OnUpdate()
        {
            if(target.GetValue() != null)
            {
                return TaskStatus.Failure;
            }
            
            MaxTime -= Time.deltaTime;
            if (MaxTime <= 0)
            {
                MaxTime = 5.0f;
                dest = UnityEngine.Random.insideUnitSphere * 5.0f;
                transform.GetComponentInParent<ActorMgr>().RPC_MoveToPosition(dest, 0.1f);
                return TaskStatus.Running;
            }
            
            if(Vector3.Distance(transform.position, dest) <= 0.1f)
            {
                return TaskStatus.Success;
            }
            
            return TaskStatus.Running;
        }
    }
}