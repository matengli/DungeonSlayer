using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Zenject;

/// <summary>
/// 目前只用来控制行为树
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
