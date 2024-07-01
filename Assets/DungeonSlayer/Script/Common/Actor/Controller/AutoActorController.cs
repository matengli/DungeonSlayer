using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Character的上的控制器
/// 逻辑上与PlayerController算是并列关系，但是事实上没有关联
/// </summary>
public class AutoActorController : MonoBehaviour
{
    [Inject] private ActorCombatMgr _combatMgr;

    [Inject] private ActorMgr _actorMgr;
    
    void Update()
    {
        if(_actorMgr.CompareTag("Player"))
            return;
        
        if(_actorMgr.IsActorDead())
            return;

        AIInput();
    }
    
    void AIInput()
    {
        combatTarget = null;
        
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
                break;                    
            }
        }
        
        _combatMgr.SetAttackTarget(combatTarget);

    }

    [Inject] private ActorCampMgr _actorCamp;

    [SerializeField] private ActorMgr combatTarget;
    
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
