using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
        // if(aEvent.animationClipInfo.Weight>0.5f)
        //     作者：Meltin_Algol https://www.bilibili.com/read/cv11088826/ 出处：bilibili
        var clip = Resources.Load<AudioClip>($"Effect/footsteps/wood/Wood footstep {Random.Range(1, 11)}");
        AudioSource.PlayClipAtPoint(clip,transform.position);
    }

    [SerializeField]private ActorAnimMgr _handler;

    public void SetHander(ActorAnimMgr handler)
    {
        _handler = handler;
    }
}
