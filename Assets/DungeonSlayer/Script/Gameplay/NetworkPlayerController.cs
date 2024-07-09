using Cinemachine;
using Mirror;
using UnityEngine;
using Zenject;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SyncVar]
        public string playerName;

        public override void OnStartServer()
        {
            playerName = (string)connectionToClient.authenticationData;
        }

        [Inject] private CameraController _cameraController;

        public override void OnStartClient()
        {
            if (isOwned)
            {
                _cameraController.SetFollowTarget(transform);
            }

        }

        private void Awake()
        {
            BindGamePlayer();
        }

        private ActorMgr player;

        public void BindGamePlayer()
        {
            player = GetComponent<ActorMgr>();
            // FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
        }

        [ClientCallback]
        void Update()
        {
            if(!isOwned)
                return;
            
            if(player==null)
                return;
            
            if(player.IsActorDead())
                return;
            
            if (!_gameUtil.CheckClickPos())
            {
                var origin = player.transform.position;
                var lookat = _gameUtil.GetMouseWorldPosition();
                lookat.y = origin.y;
            
                RPC_MoveByAsix(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Quaternion.LookRotation(lookat - origin));
                return;
            }

            CMD_Attack();
        }

        [Inject] private GameUtil _gameUtil;
        
        [Command]
        private void CMD_Attack()
        {
            // connectionToClient.identity;
            RPC_Attack();
        }
        
        [ClientRpc]
        private void RPC_Attack()
        {
            player.PerformAttack();
        }

        private void RPC_MoveByAsix(float forward, float right, Quaternion rotation)
        {
            player.GetComponentInChildren<ActorMoveMgr>().SetMoveAsixInput(forward, right, rotation);
        }
        
    }
}