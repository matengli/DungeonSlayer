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
    [Inject] private ActorCombatMgr _combatMgr;

    [Inject] private ActorMgr _actorMgr;
    [Inject] private ActorMoveMgr _moveMgr;
    
    [Server]
    void Update()
    {
        if(_actorMgr.CompareTag("Player"))
            return;
        
        if(_actorMgr.IsActorDead())
            return;

        AIInput();
        CheckWonder();
    }

    [SerializeField]private float wonderCD = 0;

    private void CheckWonder()
    {
        if(combatTarget!=null)
            return;

        wonderCD -= Time.deltaTime;
        if (wonderCD > 0f)
        {
            return;
        }

        wonderCD = 5.0f;
        RPC_MoveToDest(UnityEngine.Random.insideUnitSphere * 5.0f);
    }
    
    [ClientRpc]
    private void RPC_MoveToDest(Vector3 dest)
    {
        _actorMgr.MoveToPosition(dest);
    }
    
    void OnChangeCombatTarget(ActorMgr _old, ActorMgr _new)
    {
        _combatMgr.SetAttackTarget(_new);
    }

    [Server]
    void AIInput()
    {
        bool isSet = false;
        
        var result = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
        
        foreach (var item in result)
        {
            if (item.transform != transform)
            {
                var otherActorMgr = item.GetComponent<ActorMgr>();
                
                if(otherActorMgr==null)
                    continue;
                
                if(otherActorMgr.IsActorDead())
                    continue;
        
                if (otherActorMgr.GetActorCamp() == _actorCamp.GetCamp() || otherActorMgr.GetActorCamp() == ActorCampMgr.ActorCamp.Neutral)
                {
                    continue;
                }
        
                combatTarget = otherActorMgr;
                isSet = true;

                break;
            }
        }

        if (!isSet)
        {
            combatTarget = null;
        }

        _combatMgr.SetAttackTarget(combatTarget);

    }

    [Inject] private ActorCampMgr _actorCamp;

    [SyncVar(hook = nameof(OnChangeCombatTarget))][SerializeField] private ActorMgr combatTarget;
    
    [SerializeField] private float searchRange = 5.0f;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;;
        Gizmos.DrawWireSphere(transform.position, searchRange);
        
        Gizmos.color = Color.red;;
        if(_combatMgr!=null && _combatMgr.GetCurWeapon()!=null)
            Gizmos.DrawWireSphere(transform.position, _combatMgr.GetCurWeapon().range);
    }
    
}
