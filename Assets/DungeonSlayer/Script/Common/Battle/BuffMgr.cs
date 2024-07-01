using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BuffMgr
{
    [Inject] public DamageMgr _damageMgr;

    public BuffMgr(DamageMgr dmgMgr)
    {
        _damageMgr = dmgMgr;
    }

    public void AddBuffToActor(ActorBattleMgr receiver, ActorBattleMgr causer, BuffBase.AddBuffInfo info)
    {
        var addBuff = new BuffBase.AddBuffInfo();
        addBuff.carrier = receiver;
        addBuff.caster = causer;
        addBuff.model = info.model;
        addBuff.model.InjectBaseDp( _damageMgr, this);
        addBuff.addStack = info.addStack;
        addBuff.buffTime = new BuffBase.BuffTime();
        addBuff.buffTime.existTime = info.buffTime.existTime;
        addBuff.buffTime.isLoop = info.buffTime.isLoop;
        addBuff.buffTime.timeElapsed = 0;
        receiver._addBuffList.Add(addBuff);
        
    }
}
