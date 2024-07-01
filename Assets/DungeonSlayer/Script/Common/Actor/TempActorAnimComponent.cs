using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 唯一作用就是用来接受Animation上的动画事件
/// </summary>
public class TempActorAnimComponent : MonoBehaviour
{
    public void TriggerAnimatorEvent(string input)
    {
        _handler.TriggerAnimatorEvent(input);
    }

    public void TriggerTimelineEvent(string input)
    {
        _handler.TriggerTimelineEvent(input);
    }

    public void Effect_FootStep(AnimationEvent animationEvent)
    {
        if(animationEvent.animatorClipInfo.weight<=0.5f)
            return;
        var clip = Resources.Load<AudioClip>($"Effect/footsteps/wood/Wood footstep {Random.Range(1, 11)}");
        AudioSource.PlayClipAtPoint(clip,transform.position);
    }

    [SerializeField]private ActorAnimMgr _handler;

    public void SetHander(ActorAnimMgr handler)
    {
        _handler = handler;
    }
}
