using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "ModifyMaxHpModel", menuName = "CuteAnimal/Buff/Create New ModifyMaxHpModel")]
    public class ModifyMaxHpModel : BuffModelBase
    {
        [SerializeField] public float AddCount = 10.0f;

        public override void OnAddedBuff(BuffBase buff)
        {
            var owner = buff.carrier;
            
            owner.AddMaxHp(AddCount * buff.stack);
        }
    }
}
