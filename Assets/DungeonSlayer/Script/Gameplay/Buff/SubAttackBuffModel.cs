using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "SubAttackBuff", menuName = "CuteAnimal/Buff/Create New SubAttackBuff")]
    public class SubAttackBuffModel : BuffModelBase
    {
        [SerializeField] public float subAmount = 1.0f;

        public override void OnGetActualAttack(ActorBattleMgr handler, ref float rawAttack,BuffBase stack)
        {
            rawAttack -= subAmount*stack.stack;
        }
    }
}
