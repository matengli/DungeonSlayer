using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "ClearBuffModel", menuName = "CuteAnimal/Buff/Create New ClearBuffModel")]
    public class ClearBuffModel : BuffModelBase
    {
        public override void OnAddedBuff(BuffBase owner)
        {
            owner.carrier.RemoveAllBuff();
        }
    }
}
