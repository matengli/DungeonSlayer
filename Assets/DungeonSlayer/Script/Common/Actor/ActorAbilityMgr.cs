using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DungeonSlayer.Script.Common.Game;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

/// <summary>
/// Ability是一个具体的动作，比如跳跃，释放技能，死亡等等，一般会伴随一个具体的动画以及一系列判断条件
/// Ability必须依附于State来做执行
/// </summary>
public class ActorAbilityMgr : MonoBehaviour
{
    #region MgrGetters

    [Inject] private GameBulletMgr _gameBulletMgr;
    public GameBulletMgr GetBulletMgr()
    {
        return _gameBulletMgr;
    }
    
    [Inject] private ActorBattleMgr _battleMgr;

    public ActorBattleMgr GetBattleMgr()
    {
        return _battleMgr;
    }
    
    [Inject] private ActorCollsionMgr _collsionMgr;
    
    private ActorCollsionMgr GetCollsionMgr()
    {
        return _collsionMgr;
    }
    
    [Inject] private AudioMgr _audioMgr;
    private AudioMgr GetAudioMgr()
    {
        return _audioMgr;
    }

    [Inject] private CameraController _cameraController;

    public CameraController GetCameraController()
    {
        return _cameraController;
    }

    #endregion
    

    /// <summary>
    /// 构建一个新的Ability，注意一定要传入一个它所依附的state
    /// </summary>
    /// <param name="ptype"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public ActorAbility ConstructAbility(Type ptype, ActorStateMgr.ActorState state)
    {
        if(!ptype.IsSubclassOf(typeof(ActorAbility)))
            return null;
        
        var result = Activator.CreateInstance(ptype) as ActorAbility;
        result.owner = state;
        
        return result;
    }

    /// <summary>
    /// 我不是很确定是否要有abilitySet这玩意，先放这里注释起来
    /// </summary>
    /// <param name="state"></param>
    #region Depature_abilitySet

        // public void AddToAbilityList(ActorAbility state)
        // {
        //     if (abilitySet == null)
        //         abilitySet = new HashSet<ActorAbility>();
        //     
        //     abilitySet.Add(state);
        // }
        //
        // public ActorAbility GetAbilityByName(string name)
        // {
        //     if (abilitySet == null)
        //         return null;
        //     
        //     foreach (var item in abilitySet)
        //     {
        //         if (item.Name == name)
        //             return item;
        //     }
        //
        //     return null;
        // }
        //
        // private HashSet<ActorAbility> abilitySet;
        //
        // public void ClearAllCurrentAbility()
        // {
        //     if (curAbility!=null)
        //         curAbility.OnExit(this);
        //
        //     curAbility = null;
        //
        //     if(abilitySet!=null)
        //         abilitySet.Clear();
        // }

    #endregion
    

    [Inject] private ActorAnimMgr _animMgr;

    public ActorAnimMgr GetAnimMgr()
    {
        return _animMgr;
    }
    
    [Inject] private ActorStateMgr _stateMgr;

    public ActorStateMgr GetStateMgr()
    {
        return _stateMgr;
    }
    
    [Inject] private ActorCombatMgr _combatMgr;
    public ActorCombatMgr GetCombatMgr()
    {
        return _combatMgr;
    }
    
    private ActorAbility curAbility;

    /// <summary>
    /// 尝试perform这个Ability，成功以后会直接变成这个状态，并且返回是否成功
    /// ability可以尝试传入空，即为清除当前ability，但是需要isCheckState设置为false
    /// </summary>
    /// <param name="Ability"></param>
    /// <param name="isCheckState">是否检查条件</param>
    /// <returns></returns>
    public bool TryPerformAbility(ActorAbility ability, bool isCheckState = true)
    {
        if (isCheckState && !ability.CanPerform(this))
            return false;

        if(curAbility!=null)
            curAbility.OnExit(this);
        
        curAbility = ability;
        //有可能这个是一个空
        if(curAbility!=null)
            curAbility.OnEnter(this);

        return true;
    }

    public ActorAbility GetCurrentAbility()
    {
        return curAbility;
    }

    #region Ablity

    public class ActorAbility
    {
        public ActorStateMgr.ActorState owner;
        public virtual string Name => "";

        virtual public void OnEnter(ActorAbilityMgr handler)
        {
            
        }

        virtual public void OnExit(ActorAbilityMgr handler)
        {
            
        }

        virtual public void OnTick(ActorAbilityMgr handler)
        {
            
        }

        virtual public bool CanPerform(ActorAbilityMgr stateMgr)
        {
            return true;
        }
    }

    public class NormalAttackAbility:ActorAbility
    {
        public override string Name => "normalAttack";

        public enum NormalAttackAbilityStatusEnum
        {
            BeforeTrace,//伤害判定之前
            AfterTrace,//伤害判定之后，Combo窗口结束之前，可以连击，不能移动
            AfterComboTimeWindow,//Combo窗口结束，可以移动
        }

        public NormalAttackAbilityStatusEnum currentStage;

        public override void OnEnter(ActorAbilityMgr handler)
        {
            currentStage = NormalAttackAbilityStatusEnum.BeforeTrace;
            
            handler.GetAnimMgr().SetOnlyBlendUpperPose(true);
            handler.GetAnimMgr().PlayAbilityClipByAbility(this, true, (ActorAnimMgr.MontageEventEnum ptype,string name) =>
            {
                if (ptype == ActorAnimMgr.MontageEventEnum.VaildCollsionPlayableAssetStart)
                {
                    currentStage = NormalAttackAbilityStatusEnum.AfterTrace;
                    handler.GetCollsionMgr().TriggerOverLap(true, handler.GetCombatMgr().GetCurWeapon().range, handler.GetCombatMgr().GetCurWeapon().rangeAngle);
                    handler.GetAudioMgr().PlayBattleEffect(AudioMgr.BattleEffectSEEnum.swordSwing, handler.transform.position);
                }

                if (ptype == ActorAnimMgr.MontageEventEnum.VaildCollsionPlayableAssetEnd)
                {
                    currentStage = NormalAttackAbilityStatusEnum.AfterComboTimeWindow;
                }

                if (ptype == ActorAnimMgr.MontageEventEnum.End)
                {
                    handler.GetStateMgr().TryPerformState(handler.GetStateMgr().GetStateByName("idle"));
                    handler.TryPerformAbility(null, false);
                }
            }, (owner as ActorStateMgr.AttackState).combatCount, handler.GetCombatMgr().GetModifiedAnimationTime() );
        }

        public override void OnExit(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().SetOnlyBlendUpperPose(false);
        }
    }

    public class NormalAttackCastAbility:ActorAbility
    {
        public override string Name => "normalAttackCast";

        private bool isAlive = true;

        public override void OnEnter(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().SetOnlyBlendUpperPose(true);
            
            handler.GetAnimMgr().PlayAbilityClipByAbility(this, true, (ptype, a) =>
            {
                if (ptype == ActorAnimMgr.MontageEventEnum.VaildCollsionPlayableAssetStart)
                {
                    BulletModel model = handler.GetCombatMgr().GetCurWeapon().bulletModel;
                    
                    handler.GetAudioMgr().PlayBattleEffect(AudioMgr.BattleEffectSEEnum.laser, handler.transform.position);
            
                    handler.GetBulletMgr().FireBullet(model ,handler.GetBattleMgr(), (bulletObj, hitActor) =>
                    {
                        Destroy(bulletObj.go);

                        if (hitActor != null)
                        {
                            handler.GetCollsionMgr().CheckCommonHitOther(hitActor.GetComponent<Collider>(), true);
                        }
                        
                        if(!isAlive)
                            return;
                        
                        handler.GetStateMgr().TryPerformState(handler.GetStateMgr().GetStateByName("idle"));
                        handler.TryPerformAbility(null, false);
                    }, handler.GetCombatMgr().GetCurWeapon().range);
                }else if (ptype == ActorAnimMgr.MontageEventEnum.End)
                {
                    handler.GetAnimMgr().SetOnlyBlendUpperPose(false);
                    handler.GetStateMgr().TryPerformState(handler.GetStateMgr().GetStateByName("idle"));
                    handler.TryPerformAbility(null, false);
                }
            }, handler.GetCombatMgr().GetModifiedAnimationTime());
        }

        public override void OnExit(ActorAbilityMgr handler)
        {
            isAlive = false;
        }
    }
    
    public class NormalDeathAbility:ActorAbility
    {
        public override string Name => "normalDeath";

        public override void OnEnter(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().PlayAbilityClipByAbility(this, true, (ptype, ss) =>
            {
                if(ptype==ActorAnimMgr.MontageEventEnum.End)
                    handler.GetAnimMgr().SetMontageState(true);
            });
        }
    }
    
    public class NormalMakingAbility:ActorAbility
    {
        public override string Name => "normalMaking";

        public override void OnEnter(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().PlayAbilityClipByAbility(this);
        }

        public override void OnExit(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().SetMontageState(false);
        }
    }

    public class NormalStunAbility : ActorAbility
    {
        public override string Name => "normalStun";
        
        public override void OnEnter(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().PlayAbilityClipByAbility(this);
        }
        
        public override void OnExit(ActorAbilityMgr handler)
        {
            handler.GetAnimMgr().SetMontageState(false);
        }
    }
    
    public class HitReactAbility : ActorAbility
    {
        public override string Name => "hitReact";

        public bool isBack;

        private ActorAbilityMgr Handler;
        public override void OnEnter(ActorAbilityMgr handler)
        {
            var dmgInfo = (owner as ActorStateMgr.HitReactState).damageInfo;
            
            handler.GetAudioMgr().PlayBattleEffect(AudioMgr.BattleEffectSEEnum.damage, handler.transform.position);

            
            if(!dmgInfo.Defender.transform.parent.CompareTag("Player"))
                handler.GetCameraController().ShakeCamera();

            Handler = handler;

            PauseForHit();
        }

        //一个简单的卡肉的效果
        public async UniTask PauseForHit()
        {
            var info = (owner as ActorStateMgr.HitReactState).damageInfo;
            
            if(info.Attacker!=null)
                info.Attacker.GetComponentInParent<ActorMgr>().SetMontagePause(true);

            var handler = Handler;
            
            float recoverTime = 0;
            var dmgInfo = (owner as ActorStateMgr.HitReactState).damageInfo;
            if (dmgInfo.Attacker != null)
            {
                recoverTime = dmgInfo.Attacker.GetHitRecoverTime();
            }
            
            handler.GetAnimMgr().PlayAbilityClipByAbility(this, true, (ptype, ss) =>
            {
                if (ptype == ActorAnimMgr.MontageEventEnum.End)
                {
                    handler.GetStateMgr().TryPerformState(handler.GetStateMgr().GetStateByName("idle"));
                    handler.TryPerformAbility(null, false);
                }
            }, recoverTime);

            var go = Instantiate(Resources.Load<GameObject>("Prefab/Punch Hit"), handler.transform);
            go.transform.position = handler.transform.position + Vector3.up*0.5f;
            go.transform.DOScale(go.transform.localScale.x, 3).onComplete= () =>
            {
                Destroy(go);
            };
            
            info.Defender.GetComponentInParent<ActorMgr>().SetMontagePause(true);
            
            await UniTask.WaitForSeconds(0.2f);
            
            if(info.Attacker!=null)
                info.Attacker.GetComponentInParent<ActorMgr>().SetMontagePause(false);
            info.Defender.GetComponentInParent<ActorMgr>().SetMontagePause(false);
        }
    }

    #endregion


}
