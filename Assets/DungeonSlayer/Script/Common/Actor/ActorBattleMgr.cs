using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// 这里管理了战斗相关的逻辑，
/// 区别与CombatMgr，CombatMgr更多是管理武器相关的逻辑，这里处理的逻辑更多是数值逻辑，周期回调等等
/// </summary>
public class ActorBattleMgr : MonoBehaviour
{
    [SerializeField] public List<BuffBase> _buffList;
    [SerializeField] public List<BuffBase.AddBuffInfo> _addBuffList;

    [Inject] private ActorAttributeMgr _attributeMgr;
    [Inject] private ActorCombatMgr _combatMgr;
    [Inject] private ActorMoveMgr _moveMgr;
    public float Hp
    {
        get => _attributeMgr.GetVal("hp");
        set => _attributeMgr.SetVal("hp", value);
    }
    
    #region Attribute

    private void Awake()
    {
        _buffList = new List<BuffBase>();
        _addBuffList = new List<BuffBase.AddBuffInfo>();
        
    }

    public bool IsDead()
    {
        return Hp <= 0;
    }

    public bool IsGoingDead(DamageInfo info)
    {
        if ((Hp - info.Damage) <= 0)
        {
            return true;
        }
        
        return false;
    }
    
    
    private static readonly string DEBUG_FLAG = "<color=yellow>[BattleTest]</color>";

    #endregion
    
    
    //关于伤害流程相关的回调和事件
    #region DamageZone

    //产生伤害或击杀前
    //造成伤害
    public event Action<DamageInfo> OnCauseDamage;
    //接受伤害
    public event Action<DamageInfo> OnReceiveDamage;
    //结算前生命值<=0
    public event Action<DamageInfo> OnBeforeKilled;
    //Kill其他Actor
    public event Action<DamageInfo> OnKillOther;
    //被干掉
    public event Action<DamageInfo> OnKilled;
    
    //真正产生伤害或击杀
    public event Action<DamageInfo> OnApplyDamageEvent;

    //事件相关的触发器

    public virtual void OnTriggerCauseDamage(ref DamageInfo obj)
    {
        OnCauseDamage?.Invoke(obj);
        
        foreach (var buff in _buffList)
        {
            buff.model.OnCauseDamage(this, obj.Defender, ref obj);
        }
    }

    public virtual void OnTriggerReceiveDamage(ref DamageInfo obj)
    {
        OnReceiveDamage?.Invoke(obj);
        
        foreach (var buff in _buffList)
        {
            buff.model.OnReceiveDamage(this, obj.Defender,ref obj);
        }
    }

    public virtual void OnTriggerBeforeKilled(ref DamageInfo obj)
    {
        OnBeforeKilled?.Invoke(obj);
    }

    public virtual void OnTriggerKillOther(ref DamageInfo obj)
    {
        OnKillOther?.Invoke(obj);
        

        foreach (var buff in _buffList)
        {
            buff.model.OnKill(ref obj);
        }
    }

    public virtual void OnTriggerKilled(ref DamageInfo obj)
    {
        OnKilled?.Invoke(obj);
        
        foreach (var buff in _buffList)
        {
            buff.model.OnKilled( ref obj);
        }
    }
    
    protected virtual void OnTriggerApplyDamageEvent(DamageInfo obj)
    {
        OnApplyDamageEvent?.Invoke(obj);
    }

    /// <summary>
    /// 不可用于加血或者加护盾，应该有其他接口来实现该功能
    /// </summary>
    /// <param name="info"></param>
    public void ApplyDamage(DamageInfo info)
    {
        float dmg = info.Damage;

        Hp -= dmg;

        OnTriggerApplyDamageEvent(info);
    }


    /// <summary>
    /// 用来给角色加血
    /// </summary>
    public void HealActor(float amount)
    {
        Hp += amount;
    }

    [Inject] private ActorStateMgr _stateMgr;
    
    public ActorStateMgr GetStateMgr()
    {
        return _stateMgr;
    }
    
    public ActorMoveMgr GetMoveMgr()
    {
        return _moveMgr;
    }

    [Inject] private ActorMgr _actorMgr;
    public ActorMgr GetActorMgr()
    {
        return _actorMgr;
    }
    
    #endregion

    #region Battle

    [Inject] private DamageMgr _damageMgr;

    //最基础的战斗手段，直接攻击力造成伤害,具体伤害取决于角色的武器
    public virtual void PunchAttackOther(ActorBattleMgr other)
    {
        var BaseAtk = GetActualAttack();

        float factor = 1.0f;

        float baseDmg = BaseAtk * factor;
        
        _damageMgr.ApplyDamage(this, other, baseDmg);
        
    }

    [SerializeField]
    private float buffedAtk = 0.0f;

    public float BuffedAtk
    {
        get => buffedAtk;
        set => buffedAtk = value;
    }

    public float GetActualAttack()
    {
        float attack = _combatMgr.GetCurWeapon().baseAtk + BuffedAtk;
        foreach (var buff in _buffList)
        {
            buff.model.OnGetActualAttack(this, ref attack);
        }
        
        return Mathf.Max(attack,0);
    }
    
    #endregion
    

    private void Update()
    {
        UpdateAddBuffList();
        UpdateCurBuffStatus();
    }

    private void UpdateAddBuffList()
    {
        if (_addBuffList.Count <= 0)
        {
            return;
        }

        //暂时没考虑循环嵌套的问题，如在增加buff的时候触发事件又增加了buff
        
        for (int i = 0; i < _addBuffList.Count; i++)
        {
            var addBuffInfo = _addBuffList[i];
            BuffBase buff = null;

            bool hasBuff = false;
            foreach (var oldBuff in _buffList)
            {
                if (oldBuff.model == addBuffInfo.model)
                {
                    buff = oldBuff;
                    hasBuff = true;
                    break;
                }
            }

            if (hasBuff){
            
                //超过最大层数
                if (buff.model.maxStack < buff.stack+addBuffInfo.addStack)
                {
                    continue;
                }

                //正常增加层数
                buff.stack += addBuffInfo.addStack;
                
            }else{
                buff = new BuffBase();
                buff.model = addBuffInfo.model;
                buff.buffTime = addBuffInfo.buffTime;
                buff.carrier = addBuffInfo.carrier;
                buff.caster = addBuffInfo.caster;
                buff.stack = addBuffInfo.addStack;
                buff.param = addBuffInfo.param;
                
                _buffList.Add(buff);
                buff.model.OnAddedBuff(buff);
            }

        }

        _addBuffList.Clear();
    }

    private void UpdateCurBuffStatus()
    {
        for (int i = _buffList.Count-1; i >=0 ; i--)
        {
            if (_buffList[i].stack <= 0)
            {
                RemoveBuff(_buffList[i]);
                continue;
            }

            _buffList[i].buffTime.timeElapsed += Time.deltaTime;

            //buff到时间了
            if (!_buffList[i].buffTime.isLoop && _buffList[i].buffTime.existTime<=_buffList[i].buffTime.timeElapsed)
            {
                RemoveBuff(_buffList[i]);
            }
        }
    }

    private void RemoveBuff(BuffBase buff)
    {
        buff.model.OnRemoved(this);
        _buffList.Remove(buff);
    }

    public void RemoveAllBuff()
    {
        for (int i = _buffList.Count-1; i >=0 ; i--)
        {
            RemoveBuff(_buffList[i]);
        }
    }

    public void AfterRoundBattler()
    {
        RemoveBuffWithTag("battle");
    }

    public void RemoveBuffWithTag(string tag)
    {
        for (int i = _buffList.Count-1; i >=0 ; i--)
        {
            var buff = _buffList[i];
            if(buff.model.tags.Contains(tag))
                RemoveBuff(buff);
        }
    }
}
