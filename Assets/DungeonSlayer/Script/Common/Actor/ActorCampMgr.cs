using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 阵营管理
/// Radiant与Radiant相互主动敌对
/// Neutral为中立
/// </summary>
public class ActorCampMgr : MonoBehaviour
{
    public enum ActorCamp
    {
        Radiant,
        Dire,
        Neutral
    }

    [SerializeField] private ActorCamp _actorCamp = ActorCamp.Neutral;

    public void SetCamp(ActorCamp camp)
    {
        _actorCamp = camp;
    }

    public ActorCamp GetCamp()
    {
        return _actorCamp;
    }
}
