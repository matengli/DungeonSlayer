using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "StunStatusBuffModel", menuName = "CuteAnimal/Buff/Create New StunStatusBuffModel")]
    public class StunStatusBuffModel : BuffModelBase
    {
        public override void OnAddedBuff(BuffBase owner)
        {
            // owner
            owner.carrier.GetStateMgr().TryPerformState(owner.carrier.GetStateMgr().GetStateByName("stun"));
        }
    
        public override void OnRemoved(ActorBattleMgr owner)
        {
            // owner
            owner.GetStateMgr().TryPerformState(owner.GetStateMgr().GetStateByName("idle"));
        }
    }
}
