using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// 阵营管理
/// Radiant与Radiant相互主动敌对
/// Neutral为中立
/// </summary>
public class ActorCampMgr : NetworkBehaviour
{
    public enum ActorCamp
    {
        Radiant,
        Dire,
        Neutral
    }

    [SyncVar][SerializeField] protected ActorCamp _actorCamp = ActorCamp.Neutral;

    public void SetCamp(ActorCamp camp)
    {
        _actorCamp = camp;
    }

    public ActorCamp GetCamp()
    {
        return _actorCamp;
    }
}
