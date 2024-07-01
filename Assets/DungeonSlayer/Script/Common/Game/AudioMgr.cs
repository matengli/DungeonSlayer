using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioMgr : SerializedMonoBehaviour
{
    // [Title("主场景BGM")][SerializeField] private AudioClip mainBgm;
    
    [Title("发现事件音效")][SerializeField] private AudioClip findSE;
    [Title("触发事件音效")][SerializeField] private AudioClip triggerSE;
    [Title("BGM配置表")][SerializeField] public Dictionary<AudioBGMEnum, AudioClip> bgmDict;

    public enum AudioSEEnum
    {
        find,
        trigger,
    }
    
    public void PlayEffect(AudioSEEnum ptype)
    {
        var clip = findSE;
        var volume = 100.0f;
        switch (ptype)
        {
            case AudioSEEnum.find:
                volume = 100.0f;
                break;
            case AudioSEEnum.trigger:
                clip = triggerSE;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ptype), ptype, null);
        }
        
        AudioSource.PlayClipAtPoint(clip, GameObject.FindWithTag("Player").transform.position, volume);
    }
    
    public enum BattleEffectSEEnum
    {
        swordSwing,
        damage,
    }
    
    public void PlayBattleEffect(BattleEffectSEEnum ptype, Vector3 pos)
    {
        switch (ptype)
        {
            case BattleEffectSEEnum.swordSwing:
                var clip = Resources.Load<AudioClip>($"Effect/sword_swing/Light Sword Swing {Random.Range(1, 16)}");
                AudioSource.PlayClipAtPoint(clip, GameObject.FindWithTag("Player").transform.position);
                break;
            
            case BattleEffectSEEnum.damage:
                clip = Resources.Load<AudioClip>($"Effect/hit/Game Punch {Random.Range(1, 31)}");
                AudioSource.PlayClipAtPoint(clip, GameObject.FindWithTag("Player").transform.position);
                break;
        }
    }
    
    
    public enum AudioBGMEnum
    {
        battle,
        normal,
    }

    public void PlayMusic(AudioBGMEnum bgmEnum)
    {
        GetComponent<AudioSource>().clip = bgmDict[bgmEnum];
        GetComponent<AudioSource>().Play();
    }
    
}
