using Mirror;
using UnityEngine;
using Zenject;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        private void Awake()
        {
            BindGamePlayer();
        }

        private ActorMgr player;

        public void BindGamePlayer()
        {
            player = GetComponent<ActorMgr>();
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
        }

        [Client]
        void Update()
        {
            if(!isLocalPlayer)
                return;
            
            if(player==null)
                return;
            
            if (!_gameUtil.CheckClickPos())
            {
                var origin = player.transform.position;
                var lookat = _gameUtil.GetMouseWorldPosition();
                lookat.y = origin.y;
            
                CMD_MoveByAsix(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Quaternion.LookRotation(lookat - origin));
                return;
            }
        
            CMD_MoveToDest(_gameUtil.GetMouseWorldPosition());     
        }

        [Inject] private GameUtil _gameUtil;
    
        [Command]
        private void CMD_MoveToDest(Vector3 dest)
        {
            RPC_MoveToDest(dest );
        }

        [Command]
        private void CMD_MoveByAsix(float forward, float right, Quaternion rotation)
        {
            RPC_MoveByAsix(forward, right, rotation);
        }
        
        [ClientRpc]
        private void RPC_MoveToDest(Vector3 dest)
        {
            player.MoveToPosition(dest);

        }

        [ClientRpc]
        private void RPC_MoveByAsix(float forward, float right, Quaternion rotation)
        {
            player.GetComponentInChildren<ActorMoveMgr>().SetMoveAsixInput(forward, right, rotation);
        }
        
    }
}