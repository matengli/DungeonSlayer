using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

/// <summary>
/// 处理武器，攻击目标相关的逻辑
/// </summary>
public class ActorCombatMgr : NetworkBehaviour
{
    [Inject] private ActorStateMgr _stateMgr;
    [Inject] private ActorModelMgr _modelMgr;
    [Inject] private ActorBattleMgr _battleMgr;
    
    void Start()
    {
        curWeapon = _modelMgr.GetInitWeapon();
        EquipWeapon(curWeapon);
        SetBattleStatus(false);
        
        _battleMgr.OnKillOther += (_)=>
        {
            SetAttackTarget(null);
        };

    }

    private GameObject curWeaponGameObject;

    [Inject] private ActorBindMgr _bindMgr;
    /// <summary>
    /// 装备武器
    /// 装备武器的时候会将整个StateMgr清空，为了适用于不同武器可能带来的状态不同
    /// </summary>
    /// <param name="weapon"></param>
    public void EquipWeapon(Weapon weapon)
    {
        _stateMgr.ClearAllCurrentState();
        
        foreach (var item in weapon._weaponDataAsset.StatesToCreate)
        {
            _stateMgr.AddToStateList(item);   
        }

        _stateMgr.TryPerformState(_stateMgr.GetStateByName("idle"));
        Clear();

        curWeaponGameObject = Instantiate(weapon.weaponObject, _bindMgr.GetBinderByName(weapon.socketName));
        curWeaponGameObject.transform.localPosition = Vector3.zero;
        curWeaponGameObject.transform.localRotation = quaternion.identity;
        _collsionMgr.InitTraceObject(curWeaponGameObject);
    }

    private bool isBattleStatus = false;
    /// <summary>
    /// 是否在战斗状态中
    /// </summary>
    public void SetBattleStatus(bool state)
    {
        isBattleStatus = state;
        
        if(curWeaponGameObject==null)
            return;
        
        curWeaponGameObject.SetActive(isBattleStatus);
        
    }

    [Inject]private ActorCollsionMgr _collsionMgr;

    /// <summary>
    /// 通过Ability获得PDA中配置的动画Clips
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    public AnimationClip[] GetCurrentWeaponAbilityAnims(ActorAbilityMgr.ActorAbility ability)
    {
        return GetCurrentWeaponAbilityAnims(ability.GetType());
    }
    
    public AnimationClip[] GetCurrentWeaponAbilityAnims(Type abilityType)
    {
        if (curWeapon == null)
            return null;

        foreach (var item in curWeapon._weaponDataAsset.abilityAnimClipsPair)
        {
            if(abilityType.ToString() == item.actorAbilityName)
                return item.clips.ToArray();
        }

        return null;
    }

    [SerializeField]
    private Weapon curWeapon;

    [Inject] private ActorMoveMgr _moveMgr;

    [SyncVar][SerializeField] private ActorMgr _combatTarget;
    [Inject] private ActorMgr _actorMgr;

    public void SetAttackTarget(ActorMgr combatTarget)
    {
        _combatTarget = combatTarget;
    }

    private void Clear()
    {
        
        if(curWeaponGameObject==null)
            return;
        
        Destroy(curWeaponGameObject);
        _collsionMgr.RemoveTraceObject(curWeaponGameObject);
    }

    public float GetAttackCd()
    {
        return curWeapon.cd;
    }

    private float attackCdTimer = 0.0f;


    [ClientRpc]
    private void RPC_TryPerformAttack()
    {
        TryPerformAttack();
    }
    
    public bool TryPerformAttack()
    {
        if(_actorMgr.IsActorDead() ||  _stateMgr?.GetCurrentState()?.Name == "stun")
            return false;
        
        if (_stateMgr.GetCurrentState().Name == "attack")
        {
            var state = _stateMgr.GetCurrentState() as ActorStateMgr.AttackState;
            state.ApplyAttackInput(_stateMgr);
            return false;
        }
        
        _stateMgr.TryPerformState(_stateMgr.GetStateByName("attack"));
        return true;
    }
    
    [Server]
    private void Update()
    {
        if(transform.parent.CompareTag("Player"))
            return;
        
        if(_actorMgr.IsActorDead() ||  _stateMgr?.GetCurrentState()?.Name == "stun")
            return;

        if(_combatTarget==null)
            return;

        if (_combatTarget.IsActorDead())
        {
            SetAttackTarget(null);
            return;
        }

        if(attackCdTimer < GetAttackCd())
            attackCdTimer += Time.deltaTime;
        
        var distance = (_combatTarget.transform.position - transform.position).magnitude;
        if (distance > curWeapon.range)
        {
            _moveMgr.RPC_MoveToPosition(_combatTarget.transform.position, curWeapon.range);
            return;
        }
        
        _moveMgr.RPC_ClearPath();
        _moveMgr.RPC_SetLookAt(_combatTarget.transform);
        
        if (_stateMgr.GetCurrentState().Name == "attack")
        {
            return;
        }
        
        
        if(attackCdTimer < GetAttackCd())
            return;

        attackCdTimer = 0;

        RPC_TryPerformAttack();
        // _stateMgr.RPC_TryPerformState("attack");
    }
    
    public Weapon GetCurWeapon()
    {
        return curWeapon;
    }
}
