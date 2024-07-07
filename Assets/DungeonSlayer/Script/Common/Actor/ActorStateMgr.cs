using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Zenject;

/// <summary>
/// 用来管理状态，参考TCF里的StateManager
/// State存储了角色的当前的角色状态，用来判断哪些ability可以执行
/// </summary>
public class ActorStateMgr : NetworkBehaviour
{
    private HashSet<ActorState> stateList;
    private ActorState curState;

    public Weapon GetCurWeapon()
    {
        return _combatMgr.GetCurWeapon();
    }

    [Inject] private ActorBattleMgr _battleMgr;
    [Inject] private ActorCombatMgr _combatMgr;
    private void Start()
    {
        _battleMgr.OnKilled += PerformDeath;

        _battleMgr.OnApplyDamageEvent += PerformDamage;

    }

    private void PerformDamage(DamageInfo obj)
    {
        var damage = GetStateByName("hitReact") as HitReactState;
        damage.damageInfo = obj;
        
        TryPerformState(damage);
    }

    private void PerformDeath(DamageInfo info)
    {
        TryPerformState(GetStateByName("death"));
    }

    /// <summary>
    /// Tick当前的State
    /// </summary>
    void Update()
    {
        if(curState==null)
            return;
        
        curState.OnTick(this);
    }

    public void AddToStateList(ActorState state)
    {
        if (stateList == null)
            stateList = new HashSet<ActorState>();
        
        stateList.Add(state);
    }

    public ActorState GetStateByName(string name)
    {
        if (stateList == null)
            return null;
        
        foreach (var item in stateList)
        {
            if (item.Name == name)
                return item;
        }

        return null;
    }

    #region DIGetter

        [Inject] private ActorAnimMgr _animMgr;
        public ActorAnimMgr GetAnimMgr()
        {
            return _animMgr;
        }

        [Inject] private ActorAbilityMgr _abilityMgr;

        public ActorAbilityMgr GetAbilityMgr()
        {
            return _abilityMgr;
        }
        
        [Inject] private ActorMoveMgr _moveMgr;

        public ActorMoveMgr GetMoveMgr()
        {
            return _moveMgr;
        }
    

    #endregion

    public ActorState GetCurrentState()
    {
        return curState;
    }

    /// <summary>
    /// 尝试perform这个State，成功以后会直接变成这个状态，并且返回是否成功
    /// </summary>
    /// <param name="state"></param>
    /// <param name="isCheckState">是否检查条件</param>
    /// <returns></returns>
    public bool TryPerformState(ActorState state, bool isCheckState = true)
    {
        if (curState != null && curState is DeathState)
            return false;
        
        if (!state.CanPerform(this) && isCheckState)
            return false;

        if(curState!=null)
            curState.OnExit(this);
        
        curState = state;
        curState.OnEnter(this);

        return true;
    }

    [ClientRpc]
    public void RPC_TryPerformState(string stateName)
    {
        TryPerformState(GetStateByName(stateName));
    }
    
    /// <summary>
    /// 清除所有的状态，比如说切换武器的时候，要将原有的状态全部清除
    /// </summary>
    public void ClearAllCurrentState()
    {
        if (curState!=null)
            curState.OnExit(this);

        curState = null;
        
        if(stateList!=null)
            stateList.Clear();
    }
    
    private void OnGUI()
    {
        if(!gameObject.CompareTag("Player"))
            return;
        
        GUILayout.BeginArea(new Rect(0,200, 400,500));
        GUILayout.Label($"CurrentState:{(curState!=null?curState.Name:"Null")}");
        GUILayout.Label($"CurrentAbility:{(_abilityMgr.GetCurrentAbility()!=null?_abilityMgr.GetCurrentAbility().Name:"Null")}");
        GUILayout.EndArea();
    }
    
    #region State
    
        public class ActorState
        {
            protected ActorAbilityMgr.ActorAbility abilityToCreate = null;
            
            protected string abilityToCreateName = "";
            
            [Sirenix.OdinInspector.ShowInInspector]
            [Sirenix.OdinInspector.ReadOnly]
            public virtual string Name => "";

            virtual public void OnEnter(ActorStateMgr handler)
            {
                
            }

            virtual public void OnExit(ActorStateMgr handler)
            {
                
            }

            virtual public void OnTick(ActorStateMgr handler)
            {
                
            }

            virtual public bool CanPerform(ActorStateMgr stateMgr)
            {
                return true;
            }
        }
        
        /// <summary>
        /// Idle状态
        /// 不需要Perform任何的Ability,只需要保持这个状态
        /// 有速度的时候就切换为walk状态
        /// </summary>
        public class IdleState:ActorState
        {
            public override string Name => "idle";

            public override void OnTick(ActorStateMgr handler)
            {
                if (handler.GetMoveMgr().GetVelocity().magnitude>=0.001f)
                {
                    handler.TryPerformState(handler.GetStateByName("walk"));
                }
            }
        }
        
        /// <summary>
        /// walk状态
        /// 速度小于一定值类的时候切换为idle状态
        /// </summary>
        public class WalkingState:ActorState
        {
            public override string Name => "walk";
            
            public override void OnTick(ActorStateMgr handler)
            {
                if (handler.GetMoveMgr().GetVelocity().magnitude<0.001f)
                {
                    handler.TryPerformState(handler.GetStateByName("idle"));
                }
            }
        }
        
        
        /// <summary>
        /// 攻击状态
        /// 因为没有具体按键输入之类的，所以走的是固定的Ability
        /// </summary>
        public class AttackState:ActorState
        {
            public override string Name => "attack";

            public override bool CanPerform(ActorStateMgr stateMgr)
            {
                if (stateMgr.GetCurrentState() is StunState | stateMgr.GetCurrentState() is DeathState)
                    return false;
                
                return true;
            }

            public int combatCount = 0;

            public override void OnEnter(ActorStateMgr handler)
            {
                combatCount = 0;
                
                var abilityMgr = handler.GetAbilityMgr();

                bool isRaycast = handler.GetCurWeapon().isRaycastWeapon;
                
                ActorAbilityMgr.ActorAbility ability = null;
                if (isRaycast)
                {
                    ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalAttackCastAbility), this);
                }
                else
                {
                    // handler.GetMoveMgr().SetIsStopped(true);
                    ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalAttackAbility), this);
                }
                
                abilityMgr.TryPerformAbility(ability);
                combatCount++;
            }

            public override void OnExit(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(false);
            }

            /// <summary>
            /// 攻击状态下接收到了攻击的输入
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public bool ApplyAttackInput(ActorStateMgr handler)
            {
                var abilityMgr = handler.GetAbilityMgr();
                var combat = abilityMgr.GetCurrentAbility() as ActorAbilityMgr.NormalAttackAbility;
                if(combat==null)
                    return false;
                
                if(combat.currentStage != ActorAbilityMgr.NormalAttackAbility.NormalAttackAbilityStatusEnum.AfterTrace)
                    return false;
                
                ActorAbilityMgr.ActorAbility ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalAttackAbility), this);
                
                abilityMgr.TryPerformAbility(ability);
                combatCount++;
                
                return true;
            }
        }
        
        /// <summary>
        /// 释放技能状态
        /// 这里可以是丢东西,使用手上的物体攻击之类的.
        /// </summary>
        public class CastSkillState:ActorState
        {
            public override string Name => "castSkill";

            public override bool CanPerform(ActorStateMgr stateMgr)
            {
                return true;
            }

            public override void OnEnter(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(true);
                    
                var abilityMgr = handler.GetAbilityMgr();
                
                // ActorAbilityMgr.ActorAbility ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalCastSkillAbility), this);
                //
                // abilityMgr.TryPerformAbility(ability);
            }

            public override void OnExit(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(false);
            }
        }
        
        /// <summary>
        /// 死亡状态
        /// </summary>
        public class DeathState:ActorState
        {
            public override string Name => "death";

            public override void OnEnter(ActorStateMgr handler)
            {

                var abilityMgr = handler.GetAbilityMgr();
                var ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalDeathAbility), this);
                abilityMgr.TryPerformAbility(ability);
                
                handler.GetMoveMgr().SetIsStopped(true);
            }
        }
        
        /// <summary>
        /// 忙碌状态,制作什么东西的状态
        /// </summary>
        public class MakingState:ActorState
        {
            public override string Name => "making";

            public override void OnEnter(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(true);

                var abilityMgr = handler.GetAbilityMgr();
                var ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalMakingAbility), this);
                abilityMgr.TryPerformAbility(ability);
            }
            
            public override void OnExit(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(false);

                handler._abilityMgr.TryPerformAbility(null, false);
            }
        }
        
        public class HitReactState:ActorState
        {
            public override string Name => "hitReact";

            public DamageInfo damageInfo;
            
            public override void OnEnter(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(true);
                
                var abilityMgr = handler.GetAbilityMgr();

                //这个游戏不太需要这一点
                // if (damageInfo.Attacker != null)
                // {
                //     handler.transform.parent.LookAt(new Vector3(damageInfo.Attacker.transform.position.x,handler.transform.parent.position.y ,damageInfo.Attacker.transform.position.z));
                // }
                var ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.HitReactAbility), this);
                abilityMgr.TryPerformAbility(ability);
            }
            
            public override void OnExit(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(false);
                
                handler._abilityMgr.TryPerformAbility(null, false);
            }
        }
        
        public class StunState:ActorState
        {
            public override string Name => "stun";

            public override void OnEnter(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(true);

                var abilityMgr = handler.GetAbilityMgr();
                var ability = abilityMgr.ConstructAbility(typeof(ActorAbilityMgr.NormalStunAbility), this);
                abilityMgr.TryPerformAbility(ability);
            }
            
            public override void OnExit(ActorStateMgr handler)
            {
                handler.GetMoveMgr().SetIsStopped(false);

                handler._abilityMgr.TryPerformAbility(null, false);
            }
        }
    
    #endregion
}
