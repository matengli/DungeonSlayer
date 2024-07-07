using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "DamageBuffModel", menuName = "CuteAnimal/Buff/Create New DamageBuffModel")]
    public class DamageBuffModel : BuffModelBase
    {
        [SerializeField] public float subAmount = 20.0f;

        public override void OnAddedBuff(BuffBase owner)
        {
            owner.carrier.Server_ApplyDamage(null, owner.carrier, subAmount * owner.stack);
        }
    }
}
