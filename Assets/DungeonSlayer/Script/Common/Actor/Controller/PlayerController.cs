using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 角色控制器，用来处理控制相关的逻辑
/// </summary>
public class PlayerController : MonoBehaviour
{
    private void Awake()
    {
        BindGamePlayer();
    }

    private ActorMgr player;

    private void BindGamePlayer()
    {
        foreach (var item in FindObjectsByType<ActorMgr>(FindObjectsSortMode.None))
        {
            if (item.CompareTag("Player"))
            {
                player = item;
                return;
            }
        }
        
    }

    void Update()
    {
        PlayerInput();
    }

    [Inject] private GameUtil _gameUtil;
    
    private void PlayerInput()
    {
        if (!_gameUtil.CheckClickPos())
        {
            // KCCMoveAgent.PlayerCharacterInputs characterInputs = new KCCMoveAgent.PlayerCharacterInputs();
            //
            var origin = player.transform.position;
            var lookat = _gameUtil.GetMouseWorldPosition();
            //
            lookat.y = origin.y;
            //
            // // Build the CharacterInputs struct
            // characterInputs.MoveAxisForward = Input.GetAxisRaw("Vertical");
            // characterInputs.MoveAxisRight = Input.GetAxisRaw("Horizontal");
            //
            //
            // // Apply inputs to character
            // player.GetComponentInChildren<KCCMoveAgent>().SetInputs(ref characterInputs);
            
            player.GetComponentInChildren<ActorMoveMgr>().SetMoveAsixInput(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Quaternion.LookRotation(lookat - origin));
            
            return;
        }
        
        player.MoveToPosition(_gameUtil.GetMouseWorldPosition());

    }
    
}
