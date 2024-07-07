using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game.Map
{
    
    [CreateAssetMenu(fileName = "MapItem", menuName = "Sunsgo/create Player MapItem")]
    public class MapItemModel : SerializedScriptableObject
    {
        //激活Buff效果
        public List<BuffBase.AddBuffInfo> addBuffInfos;

        public int ID = 0;

        public string Desc = "";
    }
}