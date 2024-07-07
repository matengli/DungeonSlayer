using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "ModifySpeedBuffModel", menuName = "CuteAnimal/Buff/Create New ModifySpeedBuffModel")]
    public class ModifySpeedBuffModel : BuffModelBase
    {
        [SerializeField][Range(0,2)] public float modifyFactor = 0.5f;

        public override void OnAddedBuff(BuffBase owner)
        {
            owner.carrier.GetMoveMgr().SetSpeedPercentage(modifyFactor);
        }

        public override void OnRemoved(ActorBattleMgr owner)
        {
            owner.GetMoveMgr().SetSpeedPercentage(1);
        }
    }
}
