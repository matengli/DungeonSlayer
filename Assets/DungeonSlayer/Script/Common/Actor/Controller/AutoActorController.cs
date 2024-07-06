using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Zenject;

/// <summary>
/// Character的上的控制器
/// 逻辑上与PlayerController算是并列关系，但是事实上没有关联
/// </summary>
public class AutoActorController : NetworkBehaviour
{
    [Inject] private ActorBattleMgr _battleMgr;
    
    public override void OnStartServer()
    {
        var bh = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
        if(bh!=null)
            bh.enabled = true;
        
        _battleMgr.OnKilled += (DamageInfo info)=>
        {
            var bh = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            if(bh!=null)
                bh.enabled = false;
        };
    }
    
}
