using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

/// <summary>
/// 类似于角色的Context，所有订阅发布的消息和接口都从这个类中来
/// </summary>
public class ActorMgr : MonoBehaviour
{
    [Inject] private ActorBattleMgr _battleMgr;
    public void AttackWithCurWeapon(ActorMgr other)
    {
        Debug.Log($"{other.name}收到了来自{gameObject.name}的伤害");
        
        _battleMgr.PunchAttackOther(other.GetBattleMgr());

        foreach (var weaponBuffConf in _combatMgr.GetCurWeapon().weaponAddBuffConfig)
        {
            switch (weaponBuffConf.BuffTarget)
            {
                case Weapon.WeaponBuffTarget.self:
                    AddBuff(weaponBuffConf.AddBuffInfo, _battleMgr);
                    break;
                case Weapon.WeaponBuffTarget.defender:
                    other.AddBuff(weaponBuffConf.AddBuffInfo, _battleMgr);
                    break;
            }
        }
        
    }

    public ActorBattleMgr GetBattleMgr()
    {
        return _battleMgr;
    }

    [Inject] private ActorAttributeMgr _attributeMgr;
    [Inject] private ActorModelMgr _modelMgr;
    [Inject] private ActorUIContainer _uiContainer;
    [Inject] private ActorStateMgr _stateMgr;

    private void Start()
    {
        isTakeTurn = false;
        
        _attributeMgr.CreateAttribute("hp", _modelMgr.GetInitHp(), _modelMgr.GetInitHp());
        _attributeMgr.CreateAttribute("san", _modelMgr.GetInitHp(), _modelMgr.GetInitHp());
        _uiContainer.InitUI();

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
    
    public  void AddBuff(BuffBase.AddBuffInfo info, ActorBattleMgr caster=null)
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
    public void MoveToDest(Transform pos)
    {
        MoveToDest(pos.position);
    }

    private Action<ActorMgr> endMoveCallback;
    
    public async UniTask MoveToDest(Vector3 pos, float stopDistance=0, Action<ActorMgr> endcallback = null)
    {
        endMoveCallback = endcallback;

        await _moveMgr.MoveToPositionAsync(pos, stopDistance);

        if (endMoveCallback != null)
            endMoveCallback(this);
    }


    public void LookAt(Transform tr)
    {
        _moveMgr.SetLookAt(tr);
    }

    public void FollowTarget(Transform target)
    {
        _moveMgr.SetFollowTarget(target);
    }

    public bool IsPlayerFriend()
    {
       return _campMgr.GetCamp()==ActorCampMgr.ActorCamp.Radiant;
    }
    

    //这个变量只是对Player有用
    public bool isTakeTurn;

    public bool IsPlayerTurn()
    {
        return isTakeTurn;
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
    
}
