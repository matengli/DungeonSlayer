using Mirror;
using UnityEngine;
using Zenject;

namespace DungeonSlayer.Script.Gameplay
{
    public class DungeonSlayerNetworkManager : NetworkManager
    {
        private int insertCount = 0;
        
        [Inject] private DiContainer _container;
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            var pos = UnityEngine.Random.insideUnitSphere * 5.0f;
            pos.y = 0.0f;
            
            GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
            
            insertCount++;
            
            NetworkServer.AddPlayerForConnection(conn, player);
            
            var ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "CommonPlayerNet"), pos, Quaternion.identity);
            conn.authenticationData = insertCount.ToString();
            NetworkServer.Spawn(ball, conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);

            insertCount--;
        }
    }
}