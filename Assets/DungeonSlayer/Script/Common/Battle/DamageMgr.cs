using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class DamageMgr
{
    /// <summary>
    /// 执行伤害事件
    /// </summary>
    /// <param name="attacker">攻击者，可能为空</param>
    /// <param name="defender">承受者，必不为空</param>
    /// <param name="damage">伤害</param>
    public void ApplyDamage(ActorBattleMgr attacker, ActorBattleMgr defender, float damage)
    {
        if (defender.IsDead())
        {
            return;
        }
        
        DamageInfo info = new DamageInfo(attacker, defender, damage);

        float factor = 1;

        bool isBack = attacker!=null && IsBackAttack(attacker.transform, defender.transform);
        if (isBack)
            factor = GetBackAttackFactor();
        
        info.Damage *= factor;
        
        ///暴击和增伤先不算吧
        ApplyDamage(info);
    }

    /// <summary>
    /// 执行伤害事件,直接使用构造好的DamageInfo
    /// </summary>
    /// <param name="attacker">攻击者，可能为空</param>
    /// <param name="defender">承受者，必不为空</param>
    /// <param name="damage">伤害</param>
    public void ApplyDamage(DamageInfo info)
    {
        var attacker = info.Attacker;
        var defender = info.Defender;
        
        //先Receive，因为可能减伤导致伤害变成0
        defender.OnTriggerReceiveDamage(ref info);

        //attacker可能没有，因为有可能是地形伤害buff伤害等等,因此我先处理Defender的逻辑
        if(attacker)
            attacker.OnTriggerCauseDamage(ref info);
        
        //是否有可能会被这次伤害杀死
        if (defender.IsGoingDead(info))
        {
            //结算可能的死亡前
            defender.OnTriggerBeforeKilled(ref info);

        }
        
        //应用伤害
        defender.ApplyDamage(info);

        //真正死亡
        if (defender.IsDead())
        {
            if(attacker)
                attacker.OnTriggerKillOther(ref info);
        
            defender.OnTriggerKilled(ref info);
        }
    }

    /// <summary>
    /// 这里看需求可以Inject进来一个设定好的值，看情况修改
    /// </summary>
    /// <returns></returns>
    public float GetBackAttackFactor()
    {
        return 1.5f;
    }

    public bool IsBackAttack(Transform attacker, Transform defender)
    {
        if (attacker == null)
            return false;
        
        bool isBack = Vector3.Dot(defender.forward.normalized, (defender.position - attacker.position).normalized)>0;
        return isBack;
    }
}
