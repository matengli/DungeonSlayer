using Mirror;
using UnityEngine;
using Zenject;

namespace DungeonSlayer.Script.Gameplay
{
    public class DungeonSlayerNetworkManager : NetworkManager
    {
        private int insertCount = 0;
        
        [Inject] private DiContainer _container;
        
        public enum StarterCharacter
        {
            Melee,
            Raycast
        }
        
        public StarterCharacter starterCharacter = StarterCharacter.Melee;
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            var pos = UnityEngine.Random.insideUnitSphere * 5.0f + new Vector3(5,0,5);
            pos.y = 0.0f;
            
            GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
            
            insertCount++;
            
            conn.authenticationData = insertCount.ToString();
            
            NetworkServer.AddPlayerForConnection(conn, player);
            
            var ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == $"Player{starterCharacter}"), pos, Quaternion.identity);
            ball.GetComponent<ActorMgr>().SetAuthID(insertCount);
            NetworkServer.Spawn(ball, conn);
            
            starterCharacter = starterCharacter == StarterCharacter.Melee ? StarterCharacter.Raycast : StarterCharacter.Melee;
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}