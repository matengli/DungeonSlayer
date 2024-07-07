using UnityEngine;

namespace DungeonSlayer.Script.Gameplay.Buff
{
    [CreateAssetMenu(fileName = "ApplyAllEnemyBuffModel", menuName = "CuteAnimal/Buff/Create New ApplyAllEnemyBuffModel")]
    public class ApplyAllEnemyBuffModel : BuffModelBase
    {
        [SerializeField] public BuffBase.AddBuffInfo AddBuffInfo;
        [SerializeField] public ActorCampMgr.ActorCamp enemyCamp;

        public override void OnAddedBuff(BuffBase owner)
        {
            foreach (var actormgr in FindObjectsByType<ActorMgr>(FindObjectsSortMode.None))
            {
                if (actormgr.GetComponentInChildren<ActorCampMgr>().GetCamp() == enemyCamp)
                {
                    actormgr.AddBuff(AddBuffInfo);
                
                }
            }
        }
    }
}
