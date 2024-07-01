using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
