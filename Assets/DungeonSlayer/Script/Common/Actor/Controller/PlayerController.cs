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


    [Inject] private ActorMgr player;

    void Update()
    {
        PlayerInput();
    }

    [Inject] private GameUtil _gameUtil;
    
    private void PlayerInput()
    {
        if(player==null)
            return;
        
        if (!_gameUtil.CheckClickPos())
        {
            var origin = player.transform.position;
            var lookat = _gameUtil.GetMouseWorldPosition();
            lookat.y = origin.y;
            
            player.GetComponentInChildren<ActorMoveMgr>().SetMoveAsixInput(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Quaternion.LookRotation(lookat - origin));
            
            return;
        }
        
        player.MoveToPosition(_gameUtil.GetMouseWorldPosition());

    }
    
}
