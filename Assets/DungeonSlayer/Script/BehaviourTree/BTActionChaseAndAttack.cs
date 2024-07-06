using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace DungeonSlayer.Script.BehaviourTree
{
    public class BTActionChaseAndAttack : Action
    {
        public SharedTransform target;
        public SharedFloat ramainDistance;

        public ActorMgr self;
        private float nextTimeCheck = 1.0f;

        public override void OnStart()
        {
            self = transform.GetComponentInParent<ActorMgr>();
            
            ramainDistance.SetValue(self.GetCurrentWeapon().range-0.1f);
        }

        public override TaskStatus OnUpdate()
        {
            var input = (target.GetValue() as Transform);
            
            if (input == null)
            {
                return TaskStatus.Failure;
            }
            
            var targetActor = input.GetComponent<ActorMgr>();
            
            if (targetActor.IsActorDead())
            {
                target.SetValue(null);
                return TaskStatus.Success;
            }

            var distance = Vector3.Distance(transform.position, input.position);
            if (distance <= (float)ramainDistance.GetValue())
            {
                self.RPC_LookAt(input);
                self.RPC_PerformAttack();
            }
            else
            {
                nextTimeCheck -= Time.deltaTime;
                if(nextTimeCheck <= 0)
                {
                    nextTimeCheck = 1.0f;
                    self.RPC_MoveToPosition(input.position, (float) ramainDistance.GetValue());
                }
            }
            
            return TaskStatus.Running;
        }
    }
}