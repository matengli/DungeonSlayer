using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 基础的Buff类型
/// </summary>
[Serializable]
public class BuffBase
{
    public BuffModelBase model;
    
    public ActorBattleMgr caster;

    public ActorBattleMgr carrier;

    public int stack;

    public Dictionary<string, object> param;
    
    public BuffTime buffTime;

    [Serializable]
    public class AddBuffInfo
    {
        public BuffModelBase model;
        public ActorBattleMgr caster;
        public ActorBattleMgr carrier;
        public int addStack = 1;
        public Dictionary<string, object> param;
        public BuffTime buffTime;

        public AddBuffInfo()
        {
            buffTime = new BuffTime();
        }
    }
    
    [Serializable]
    public class BuffTime
    {
        public bool isLoop = false; //是否一直存在
        public float existTime = 0; //剩下还能留多久
        public float timeElapsed = 0; //已经存在了多久
    }
    
}
