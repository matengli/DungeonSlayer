using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkPlayerControllerTest : NetworkBehaviour
    {
        [Range(1.0f,10.0f)][SerializeField] private float speed = 5.0f;
        
        void FixedUpdate()
        {
            PlayerInput();
        }

        private Canvas _canvas;

        private Camera _camera;
        
        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _camera = Camera.main;
        }

        void LateUpdate()
        {
            _canvas.transform.forward = _camera.transform.forward;

            _canvas.transform.Find("name").GetComponent<TextMeshProUGUI>().text =
                $"Pos:{transform.position}\nRotation:{transform.rotation}";
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
                transform.position += speed * Time.deltaTime * (Input.GetAxisRaw("Vertical") * transform.forward +
                                                               Input.GetAxisRaw("Horizontal") * transform.right);
            }
        }
    }
}