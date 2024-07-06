using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace DungeonSlayer.Script.BehaviourTree
{
    public class BTActionWithInSight : Conditional
    {
        public SharedTransform target;
        public SharedFloat ramainDistance;
        public float searchRange;
        public ActorCampMgr.ActorCamp targetCamp;

        public override void OnStart()
        {
            var actor = transform.GetComponentInParent<ActorMgr>();
            var weapon = actor.GetCurrentWeapon();
            var range = weapon.range;
            ramainDistance.Value = range;
            searchRange = transform.GetComponentInParent<ActorMgr>().GetVisionRange();

        }

        public override TaskStatus OnUpdate()
        {
            var result = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
        
            foreach (var item in result)
            {
                var actor = item.GetComponentInParent<ActorMgr>();
                if(actor==null)
                    continue;
                
                if(actor.IsActorDead())
                    continue;
                
                if(actor.GetActorCamp()!=targetCamp)
                    continue;
                
                target.Value = item.transform;
                return TaskStatus.Success;
            }

            target.Value = null;
            return TaskStatus.Failure;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, searchRange);
        }
    }
}