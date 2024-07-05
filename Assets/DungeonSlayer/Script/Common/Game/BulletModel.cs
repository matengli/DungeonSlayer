using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game
{
    [CreateAssetMenu(fileName = "Bullet Model", menuName = "Sunsgo/create bullet Model")]
    public class BulletModel : SerializedScriptableObject
    {
        public GameObject prefab;
        public float speed;
    }
}