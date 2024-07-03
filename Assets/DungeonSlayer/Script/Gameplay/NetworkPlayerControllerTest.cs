using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkPlayerControllerTest : MonoBehaviour
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

                InputAsix(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Quaternion.LookRotation(lookat - origin));
            }
        }

        public void InputAsix(float v,float h,Quaternion rotation)
        {
            transform.rotation = rotation;
            transform.position += speed * Time.deltaTime * (v * transform.forward +
                                                            h * transform.right);
        }
    }
}