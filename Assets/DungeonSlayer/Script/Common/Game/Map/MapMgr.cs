using Mirror;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game.Map
{
    public class MapMgr : NetworkBehaviour
    {
        [SerializeField] private int initWidth;
        [SerializeField] private int initHeight;

        private MapGrid<int> mapGird;
        public override void OnStartServer()
        {
            mapGird = new MapGrid<int>(initWidth, initHeight, 1.0f, Vector3.zero );
            
            
        }
    }
}