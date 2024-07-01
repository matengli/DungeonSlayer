using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 自动行动类型的控制器
/// 角色控制器，用来处理控制相关的逻辑
/// AI相关的也在这个类里，是这个类的子类
/// </summary>
public class AutoActorController : MonoBehaviour
{
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

        if (!CompareTag("Player"))
        {
            AIInput();
        }
    }
    
    [Inject] private ActorMoveMgr _moveMgr;
    
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
                _combatMgr.SetAttackTarget(combatTarget);
                break;                    
            }
        }
        
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
