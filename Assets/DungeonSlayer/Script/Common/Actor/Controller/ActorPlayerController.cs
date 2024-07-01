using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 角色控制器，用来处理控制相关的逻辑
/// </summary>
public class ActorPlayerController : MonoBehaviour
{
    [Inject] private ActorMoveMgr _moveMgr;
    [Inject] private ActorCombatMgr _combatMgr;

    [Inject] private ActorMgr _actorMgr;
    
    [Inject] private ActorModelMgr _modelMgr;

    /// <summary>
    /// 这里后续会改为新版本的InputSystem，现在暂时用原版的凑合一下
    /// </summary>
    void Update()
    {
        if(_actorMgr.IsActorDead())
            return;
        
        PlayerInput();
    }
    
    private void PlayerInput()
    {
        
    }
    
}
