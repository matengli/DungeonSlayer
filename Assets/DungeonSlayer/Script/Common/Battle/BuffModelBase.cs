using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuffModelBase : ScriptableObject
{
    [SerializeField]public List<string> tags;
    [SerializeField]public uint priority = 0;
    [SerializeField]public uint maxStack = 1;
    
    [Inject] public DamageMgr _damageMgr;
    [Inject] public BuffMgr _buffMgr;
    
    public void InjectBaseDp(DamageMgr dm, BuffMgr bm)
    {
        _damageMgr = dm;
        _buffMgr = bm;
    }
    
    public BuffModelBase()
    {
        
    }

    /// <summary>
    /// 挂上buff以后对属性的直接改变
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnAddedBuff(BuffBase owner)
    {
        
    }

    /// <summary>
    /// 当干掉了其他角色的时候的回调
    /// </summary>
    public virtual void OnKill(ref DamageInfo info)
    {
        
    }

    /// <summary>
    /// 当被其他角色干掉的时候
    /// </summary>
    /// <param name="deadActor"></param>
    /// <param name="other"></param>
    public virtual void OnKilled( ref DamageInfo info)
    {
        
    }

    /// <summary>
    /// 当自己走到新的地块的时候
    /// </summary>
    /// <param name="owener"></param>
    /// <param name="gridType"></param>
    public virtual void OnStartCheckGrid(ActorBattleMgr owener, object gridType)
    {
        
    }

    /// <summary>
    /// 当受到伤害的时候
    /// </summary>
    public virtual void OnReceiveDamage(ActorBattleMgr attacker, ActorBattleMgr defender, ref DamageInfo info)
    {
        
    }

    /// <summary>
    /// 造成了伤害的时候
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public virtual void OnCauseDamage(ActorBattleMgr attacker, ActorBattleMgr defender, ref DamageInfo info)
    {
        
    }
    
    /// <summary>
    /// 获取攻击力的时候,rawAttack为那个攻击力的ref,可以自由操作
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public virtual void OnGetActualAttack(ActorBattleMgr handler, ref float rawAttack)
    {
        
    }
    
    /// <summary>
    /// 当角色身上移除这个buff的时候
    /// </summary>
    /// <param name="handler"></param>
    public virtual void OnRemoved(ActorBattleMgr handler)
    {
        
    }



}


