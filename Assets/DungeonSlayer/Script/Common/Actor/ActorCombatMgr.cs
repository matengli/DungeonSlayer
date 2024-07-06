using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    private void Awake()
    {
        EquipWeapon(_modelMgr.GetInitWeapon());
    }

    public override void OnStartClient()
    {
        SetBattleStatus(true);

        if (_battleMgr.Hp <= 0)
        {
            SyncCheckDeath();
        }
    }

    async UniTask SyncCheckDeath()
    {
        await UniTask.WaitForSeconds(1.0f);
        
        _stateMgr.TryPerformState(_stateMgr.GetStateByName("death"), false);
        DamageInfo info = new DamageInfo(null, _battleMgr, 0);
        _battleMgr.OnTriggerKilled(ref info);
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
        curWeapon = weapon;
        _stateMgr.ClearAllCurrentState();
        
        foreach (var item in weapon._weaponDataAsset.StatesToCreate)
        {
            _stateMgr.AddToStateList(item);   
        }

        _stateMgr.TryPerformState(_stateMgr.GetStateByName("idle"));
        Clear();

        if (curWeaponGameObject != null)
        {
            Destroy(curWeaponGameObject);
        }
        
        curWeaponGameObject = Instantiate(weapon.weaponObject, _bindMgr.GetBinderByName(weapon.socketName));
        curWeaponGameObject.transform.localPosition = Vector3.zero;
        curWeaponGameObject.transform.localRotation = quaternion.identity;
        _collsionMgr.InitTraceObject(curWeaponGameObject);

        attackCdTimer = GetAttackCd();

        UpdatePlayerDirectionUI();
    }

    private void UpdatePlayerDirectionUI()
    {
        if(!transform.parent.CompareTag("Player"))
            return;
        
        var weapon = GetCurWeapon();
        var direction = transform.parent.Find("Direction");
        direction.GetComponentInChildren<Canvas>().enabled = weapon.isRaycastWeapon;
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

    [Inject] private ActorMgr _actorMgr;
    
    private void Clear()
    {
        
        if(curWeaponGameObject==null)
            return;
        
        _collsionMgr.RemoveTraceObject(curWeaponGameObject);
        Destroy(curWeaponGameObject);
    }

    public float GetAttackCd()
    {
        return curWeapon.cd;
    }
    
    public float GetModifiedAnimationTime()
    {
        return curWeapon.modifiedAniTime;
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

        if (attackCdTimer < GetAttackCd())
            return false;

        attackCdTimer = 0;
            
        if (_stateMgr.GetCurrentState().Name == "attack")
        {
            var state = _stateMgr.GetCurrentState() as ActorStateMgr.AttackState;
            state.ApplyAttackInput(_stateMgr);
            return true;
        }
        
        _stateMgr.TryPerformState(_stateMgr.GetStateByName("attack"));
        return true;
    }
    
    [Server]
    private void Update()
    {
        if(attackCdTimer < GetAttackCd())
            attackCdTimer += Time.deltaTime;
        
        if(transform.parent.CompareTag("Player"))
            return;
    }
    
    public Weapon GetCurWeapon()
    {
        return curWeapon;
    }

    public void EquipWeaponByName(string weaponModel)
    {
        var weapon = Resources.Load<Weapon>($"Weapon/{weaponModel}");
        EquipWeapon(weapon);
    }
}
