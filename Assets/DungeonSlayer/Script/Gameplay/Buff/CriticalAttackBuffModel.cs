using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "CriticalAttackBuffModel", menuName = "CuteAnimal/Buff/Create New CriticalAttackBuffModel")]
    public class CriticalAttackBuffModel : BuffModelBase
    {
        [InfoBox("暴击伤害倍率")][Range(1.0f, 5.0f)][SerializeField] public float criticalFactor = 2.0f;

        public override void OnGetActualAttack(ActorBattleMgr handler, ref float rawAttack, BuffBase buff)
        {
            rawAttack *= criticalFactor;
        }

        public override void OnCauseDamage(ActorBattleMgr attacker, ActorBattleMgr defender, ref DamageInfo info)
        {
            var add = new BuffBase.AddBuffInfo();
            add.model = this;
            add.addStack = -1;
            _buffMgr.AddBuffToActor(attacker,null, add);
        }
    }
}
