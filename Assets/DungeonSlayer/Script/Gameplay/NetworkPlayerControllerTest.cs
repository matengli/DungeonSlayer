using Mirror;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkPlayerControllerTest : NetworkBehaviour
    {
        [Range(1.0f,10.0f)][SerializeField] private float speed = 5.0f;
        void Update()
        {
            PlayerInput();
        }

        private GameUtil _gameUtil;
        private KCCMoveAgent _kccMoveAgent;
        private void PlayerInput()
        {
            if(!isLocalPlayer)
                return;

            if (_gameUtil == null)
            {
                _gameUtil = FindObjectOfType<GameUtil>();
                _kccMoveAgent = GetComponent<KCCMoveAgent>();
            }
        
            if (!_gameUtil.CheckClickPos())
            {
                var origin = transform.position;
                var lookat = _gameUtil.GetMouseWorldPosition();
                lookat.y = origin.y;

                transform.rotation = Quaternion.LookRotation(lookat - origin);
                transform.position += Time.deltaTime * (Input.GetAxisRaw("Vertical") * transform.forward +
                                                        Input.GetAxisRaw("Horizontal") * transform.right);
            }
        }
    }
}