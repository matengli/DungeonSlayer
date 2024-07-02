using Mirror;
using UnityEngine;
using Zenject;

namespace DungeonSlayer.Script.Gameplay
{
    public class DungeonSlayerNetworkManager : NetworkManager
    {
        [Inject] private DiContainer _container;
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            var pos = UnityEngine.Random.insideUnitSphere * 5.0f;
            pos.y = 0.0f;
            // GameObject player = _container.InstantiatePrefab(playerPrefab, UnityEngine.Random.insideUnitSphere * 5.0f, Quaternion.identity, transform);
            
            // GameObject player = FindObjectOfType<SceneContext>().Container.InstantiatePrefab(playerPrefab, UnityEngine.Random.insideUnitSphere * 5.0f, Quaternion.identity, transform);
            GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity, transform);
            
            // FindObjectOfType<SceneContext>().Container.InjectGameObject(player);
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}