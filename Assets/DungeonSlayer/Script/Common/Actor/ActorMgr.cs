using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DungeonSlayer.Script.Common.Actor.Weapon;
using DungeonSlayer.Script.Gameplay;
using KinematicCharacterController;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

/// <summary>
/// 作为Actor的Context，作为内部某些实现的装饰类
/// 外部类调用Actor上的相关接口必须通过ActorMgr
/// </summary>
public class ActorMgr : NetworkBehaviour
{
    private void Awake()
    {
        FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
    }

    [Inject] private ActorBattleMgr _battleMgr;
    
    [ServerCallback]
    public void AttackWithCurWeapon(ActorMgr other)
    {
        _battleMgr.PunchAttackOther(other.GetBattleMgr());
        
        CheckOtherBuff(other);
    }

    public void CheckOtherBuff(ActorMgr other)
    {
        foreach (var weaponBuffConf in _combatMgr.GetCurWeapon().weaponAddBuffConfig)
        {
            switch (weaponBuffConf.BuffTarget)
            {
                case Weapon.WeaponBuffTarget.self:
                    CMD_AddBuff(weaponBuffConf.AddBuffInfo);
                    break;
                case Weapon.WeaponBuffTarget.defender:
                    other.CMD_AddBuff(weaponBuffConf.AddBuffInfo);
                    break;
            }
        }
    }

    public ActorBattleMgr GetBattleMgr()
    {
        return _battleMgr;
    }
    
    public void RegisterBattlerKillOther(Action<DamageInfo> input)
    {
        _battleMgr.OnKillOther += input;
    }

    public void RegisterBattlerGetKilled(Action<DamageInfo> input)
    {
        _battleMgr.OnKilled += input;
    }

    [Inject] private ActorAttributeMgr _attributeMgr;
    [Inject] private ActorModelMgr _modelMgr;
    [Inject] private ActorUIContainer _uiContainer;
    [Inject] private ActorStateMgr _stateMgr;

    public override void OnStartServer()
    {
        _attributeMgr.CreateAttribute("hp", _modelMgr.GetInitHp());
        _attributeMgr.EncodeDataString();    
    }

    public override void OnStartClient()
    {
        _uiContainer.InitUI();

        GetComponent<KinematicCharacterMotor>().enabled = HasRightToUpdate();
        
        if(!gameObject.CompareTag("Player"))
            return;
        
        InstallUserDataMgr();
    }
    
    private NetworkUserDataMgr _userDataMgr;

    [SyncVar] private int AuthId;
    
    public void SetAuthID(int id)
    {
        AuthId = id;
    }

    private void InstallUserDataMgr()
    {
        foreach (var mgr in FindObjectsByType<NetworkUserDataMgr>(FindObjectsSortMode.None))
        {
            if (mgr.GetAuthID() == AuthId)
            {
                _userDataMgr = mgr;
                break;
            }
        }

        _userDataMgr.RegisterActorMgr(this);
    }
    
    public NetworkUserDataMgr GetUserDataMgr()
    {
        return _userDataMgr;
    }

    public bool HasRightToUpdate()
    {
        if(transform.CompareTag("Player"))
            return isOwned;

        return isServer;
    }

    public ActorAttributeMgr GetAttrMgr()
    {
        return _attributeMgr;
    }

    public bool IsActorDead()
    {
        return _battleMgr.IsDead();
    }

    public void SetMakingAnimation(bool status)
    {
        if (status)
        {
            _stateMgr.TryPerformState(_stateMgr.GetStateByName("making"));
        }
        else
        {
            _stateMgr.TryPerformState(_stateMgr.GetStateByName("idle"));
        }
    }

    public ActorStateMgr GetActorStateMgr()
    {
        return _stateMgr;
    }
    
    [Inject] private ActorCampMgr _campMgr;
    public ActorCampMgr.ActorCamp GetActorCamp()
    {
        return _campMgr.GetCamp();
    }
    
    public void SetActorCamp(ActorCampMgr.ActorCamp camp)
    {
        _campMgr.SetCamp(camp);
    }

    [Inject] private BuffMgr _buffMgr;
    
    public void AddBuff(BuffBase.AddBuffInfo info, ActorBattleMgr caster=null)
    {
        _buffMgr.AddBuffToActor(_battleMgr, caster, info);
    }

    [Inject] private ActorViewContainer _actorViewContainer;

    public void SetCharacterMaterial(Material mat)
    {
        _actorViewContainer.GetViewObject().GetComponentInChildren<Renderer>().material = mat;
    }

    public ActorModelMgr GetModelMgr()
    {
        return _modelMgr;
    }

    [Inject] private ActorMoveMgr _moveMgr;

    public bool IsPlayerFriend()
    {
       return _campMgr.GetCamp()==ActorCampMgr.ActorCamp.Radiant;
    }

    public string GetDisplayName()
    {
        return _modelMgr.GetName();
    }

    public void SetIsStopped(bool b)
    {
        _moveMgr.SetIsStopped(b);
    }

    [Inject] private ActorCombatMgr _combatMgr;

    public Weapon GetCurrentWeapon()
    {
        return _combatMgr.GetCurWeapon();
    }

    public GameObject GetViewContainerContent()
    {
        return _actorViewContainer.GetViewObject();
    }

    public AnimationClip GetAttackAnimationClip()
    {
        foreach (var pair in  _combatMgr.GetCurWeapon()._weaponDataAsset.abilityAnimClipsPair)
        {
            if (pair.actorAbilityName.ToUpper().Contains("ATTACK"))
            {
                return pair.clips[0];
            }
        }

        return null;
    }
    

    [Inject] private ActorAnimMgr _actorAnimMgr;
    public void SetMontagePause(bool status)
    {
        _actorAnimMgr.ChangeMontagePauseState(status);
    }

    public void AfterRoundBattler()
    {
        _battleMgr.AfterRoundBattler();
    }

    public ActorViewContainer GetViewContainer()
    {
        return _actorViewContainer;
    }

    public void MoveToPosition(Vector3 pos, float remainDistance = 0.1f)
    {
        _moveMgr.MoveToPosition(pos, remainDistance);
    }
    
    [ClientRpc]
    public void RPC_MoveToPosition(Vector3 pos, float remainDistance)
    {
        MoveToPosition(pos, remainDistance);
    }
    

    [ClientRpc]
    public void RPC_PerformAttack()
    {
        PerformAttack();
    }

    public void PerformAttack()
    {
        _combatMgr.TryPerformAttack();
    }
    
    public void RPC_PerformStateByName(string state)
    {
        _stateMgr.RPC_TryPerformState(state);
    }
    
    [Command]
    public void CMD_EquipWeapon(string weaponModel)
    {
        RPC_EquipWeapon(weaponModel);
    }

    [ClientRpc]
    public void RPC_EquipWeapon(string weaponModel)
    {
        _combatMgr.EquipWeaponByName(weaponModel);
    }

    public float GetVisionRange()
    {
        return _modelMgr.GetModel().SearchRange;
    }

    [ClientRpc]
    public void RPC_LookAt(Transform input)
    {
        SetLookAt(input.position);
    }

    public void SetLookAt(Vector3 input)
    {
        _moveMgr.SetLookAt(input);
    }

    [Command(requiresAuthority = false)]
    public void CMD_AddBuff(List<BuffBase.AddBuffInfo> addBuffInfos)
    {
        RPC_AddBuff(addBuffInfos);
    }
    
    [ClientRpc]
    public void RPC_AddBuff(List<BuffBase.AddBuffInfo> addBuffInfos)
    {
        foreach (var addBuffInfo in addBuffInfos)
        {
            AddBuff(addBuffInfo);
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CMD_AddBuff(BuffBase.AddBuffInfo addBuffInfo)
    {
        RPC_AddBuff(addBuffInfo);
    }
    
    [ClientRpc]
    public void RPC_AddBuff(BuffBase.AddBuffInfo addBuffInfo)
    {
        AddBuff(addBuffInfo);
    }
}
