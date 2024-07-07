using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "HealBuffModel", menuName = "CuteAnimal/Buff/Create New HealBuffModel")]
    public class HealBuffModel : BuffModelBase
    {
        [SerializeField] public float HealAmount = 1.0f;
    
        public override void OnAddedBuff(BuffBase buff)
        {
            var owner = buff.carrier;
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Effect/Revive heal"), owner.transform.position);
            owner.HealActor(HealAmount * buff.stack);
        }
    }
}
