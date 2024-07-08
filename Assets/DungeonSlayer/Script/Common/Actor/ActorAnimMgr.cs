using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DungeonSlayer.Script.Common.Actor.Weapon;
using DungeonSlayer.Script.Common.Playable;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Zenject;
using Random = UnityEngine.Random;

/// <summary>
/// 主要职责是管理动画
/// </summary>
public class ActorAnimMgr : NetworkBehaviour
{
    private Animator _animator;

    [Inject] private ActorMoveMgr _moveMgr;

    [Inject] private ActorCombatMgr _combatMgr;


    // Start is called before the first frame update
    public override void OnStartClient()
    {
        _animator = transform.parent.GetComponentInChildren<Animator>();
        
        _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AC/characer_base");

        lowerBody = Resources.Load<AvatarMask>("Mask/DownBody");
        fullBody = Resources.Load<AvatarMask>("Mask/FullBody");
        upperBody = Resources.Load<AvatarMask>("Mask/UpperBody");
        
        InitPlayableGraph();
    }

    /// <summary>
    /// 初始化动画Graph
    /// 创建一个节点来和Animator做混合
    /// 给Animatror添加一个Component专门用来接受动画事件
    /// </summary>

    [Inject] private DiContainer _container;
    private TempActorAnimComponent _tempActorAnim;

    [SerializeField] private AvatarMask lowerBody;
    [SerializeField] private AvatarMask fullBody;
    [SerializeField] private AvatarMask upperBody;
    private void InitPlayableGraph()
    {
        graph = PlayableGraph.Create();

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "InsertMontage", _animator);
        montagePose = AnimationClipPlayable.Create(graph, null);

        // _animator.playableGraph.
        // _animator.playableGraph.GetOutput(0);
        AnimatorControllerPlayable rawinput =
            AnimatorControllerPlayable.Create(graph, _animator.runtimeAnimatorController);
        // var rawInput = _animator.playableGraph.GetRootPlayable(0).GetInput(0);
        // var rawInput = _animator.playableGraph.GetOutput(0);
        var mixerPlayable = AnimationLayerMixerPlayable.Create(graph, 2);
        
        finalPose = mixerPlayable;
        
        graph.Connect(rawinput, 0, mixerPlayable, 0);
        graph.Connect(montagePose, 0, mixerPlayable, 1);
        
        output.SetSourcePlayable(mixerPlayable);

        _container.InstantiateComponent<TempActorAnimComponent>(_animator.gameObject);
        // _animator.AddComponent<TempActorAnimComponent>();
        _tempActorAnim = _animator.GetComponent<TempActorAnimComponent>();
        _tempActorAnim.SetHander(this);
        
        graph.Play();
    }

    /// <summary>
    /// 混合比例，为1则全部为Montage动画
    /// </summary>
    [Range(0, 1)] public float mixFactor;

    private PlayableGraph graph;
    private AnimationClipPlayable montagePose;
    private AnimationLayerMixerPlayable finalPose;

    public enum MontageEventEnum
    {
        End, //动画播放完毕
        VaildCollsionPlayableAssetStart, //VaildCollsion开始（参考SkillConfig目录下的Timeline配置）
        VaildCollsionPlayableAssetEnd,//VaildCollsion结束（参考SkillConfig目录下的Timeline配置）
    }

    private Action<MontageEventEnum, string> montageEventCallback = null;

    /// <summary>
    /// 加载动画的配置，目前主要是攻击动画的配置Timeline
    /// </summary>
    /// <param name="name"></param>
    private void InitTimelineFunctionalConfig(string name)
    {
        var asset = Resources.Load<TimelineAsset>("SkillConfig/"+name);
        
        if(asset==null)
            return;

        var track = asset.GetOutputTrack(1);

        double start = 0;
        double end = 0;
        
        foreach (var timelineClip in track.GetClips())
        {
            // Debug.Log("Clip Name: " + timelineClip.displayName);
            // Debug.Log("Clip Start Time: " + timelineClip.start);
            // Debug.Log("Clip End Time: " + timelineClip.end);
            start = timelineClip.start;
            end = timelineClip.end;

        }
        
        var track1 = asset.GetOutputTrack(0) as AnimationTrack;
        
        foreach (var anim in track1.GetClips())
        {
            AddTimelineEvent(anim.animationClip, (float)start, (float) end);
        }
    }

    /// <summary>
    /// 播放指定Ablity的动画
    /// </summary>
    /// <param name="ability"></param>
    public void PlayClip(AnimationClip clip, bool isPlayAtOnce = false, Action<MontageEventEnum,string> callback = null, float minTime = 0.0f)
    {
        AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, clip);
        
        graph.Disconnect(finalPose, 1);
        graph.Connect(animationClipPlayable, 0, finalPose, 1);
        graph.DestroyPlayable(montagePose);
        
        montagePose = animationClipPlayable;

        InitTimelineFunctionalConfig(clip.name);

        graph.Play();

        if (minTime != 0.0f)
            animationClipPlayable.SetSpeed(clip.length/minTime);

        SetMontageState(true);
        
        if(isPlayAtOnce)
            SetPlayAtOnce(clip);

        if (callback!=null)
            montageEventCallback = callback;
    }
    
    public void PlayAbilityClipByAbility(ActorAbilityMgr.ActorAbility ability, bool isPlayAtOnce, Action<MontageEventEnum,string> callback, int animationIndex, float minTime = 0.0f)
    {
        var clips = _combatMgr.GetCurrentWeaponAbilityAnims(ability);
        
        var clip = clips[animationIndex%clips.Length];

        PlayClip(clip,isPlayAtOnce , callback, minTime);
    }
    
    public void PlayAbilityClipByAbility(ActorAbilityMgr.ActorAbility ability, bool isPlayAtOnce = false, Action<MontageEventEnum,string> callback = null, float minTime = 0.0f)
    {
        var clips = _combatMgr.GetCurrentWeaponAbilityAnims(ability);
        PlayAbilityClipByAbility(ability, isPlayAtOnce, callback, Random.Range(0, clips.Length), minTime);
    }

    public void GetMontageState(out AnimationClip clip, out double time )
    {
        clip = montagePose.GetAnimationClip();
        time = montagePose.GetTime() / montagePose.GetSpeed();
    }

    public void ChangeMontagePauseState(bool isPause)
    {
        if (isPause)
        {
            montagePose.Pause();
        }
        else
        {
            montagePose.Play();
        }
        
    }
    
    public double GetCurrentMontageTime()
    {
        return montagePose.GetTime()/(montagePose.GetAnimationClip().length);
    }
    
    private void Update()
    {
        if(_animator==null)
            return;
        
        if (_moveMgr != null && isSetMoveFactor )
        {
            var vel = _moveMgr.GetVelocity();
            
            var faceDir = transform.forward;
            var rightDir = transform.right;
            _animator.SetFloat("vel_v", Vector3.Dot(faceDir, vel) );
            _animator.SetFloat("vel_h", Vector3.Dot(rightDir, vel));
        }

        if (isOnlyBlendUpperPose)
        {
            finalPose.SetInputWeight(0, 1);
            finalPose.SetInputWeight(1, 1);
        }
        else
        {
            finalPose.SetInputWeight(0, 1.0f - mixFactor);
            finalPose.SetInputWeight(1, mixFactor);
        }


    }

    private bool isSetMoveFactor = true;

    public void SetMoveFactor(bool status)
    {
        _animator.enabled = status;
        isSetMoveFactor = status;
    }

    public void SetMontageState(bool state)
    {
        mixFactor = state ? 1 : 0;
    }
    
    public void SetOnlyBlendUpperPose(bool p0)
    {
        isOnlyBlendUpperPose = p0;
        
        if(finalPose.IsNull())
            return;
        
        finalPose.SetLayerMaskFromAvatarMask(0, isOnlyBlendUpperPose? lowerBody:fullBody);
        finalPose.SetLayerMaskFromAvatarMask(1, isOnlyBlendUpperPose? upperBody:fullBody);
    }
    
    //动画事件相关
    #region AnimationEvent

        /// <summary>
        /// 设置了一次以后会永远生效，如果要Clip复用在其他场合会有问题
        /// </summary>
        /// <param name="clip"></param>
        public void SetPlayAtOnce(AnimationClip clip)
        {
            foreach (var item in montagePose.GetAnimationClip().events)  
            {
                //已经设置过了
                if(item.functionName=="TriggerAnimatorEvent")
                    return;
            }
            
            var aniEvent = new AnimationEvent();
            aniEvent.time = clip.length;
            aniEvent.functionName = "TriggerAnimatorEvent";
            aniEvent.stringParameter = clip.name;
            montagePose.GetAnimationClip().AddEvent(aniEvent);
        }
        
        public void TriggerAnimatorEvent(string input)
        {
            // Debug.Log("动画播放完毕"+input);
            
            if (montageEventCallback!=null)
            {
                SetMontageState(false);
                
                montageEventCallback(MontageEventEnum.End,input);
                montageEventCallback = null;
            }
        }
        
        /// <summary>
        /// 设置了一次以后会永远生效,如果要Clip复用在其他场合会有问题
        /// </summary>
        /// <param name="clip"></param>
        public void AddTimelineEvent(AnimationClip clip, float start, float end)
        {
            foreach (var item in montagePose.GetAnimationClip().events)  
            {
                //已经设置过了
                if(item.functionName=="TriggerTimelineEvent")
                    return;
            }
            
            var aniEvent = new AnimationEvent();
            aniEvent.time = start;
            aniEvent.functionName = "TriggerTimelineEvent";
            aniEvent.stringParameter = "VaildCollsionPlayableAsset_Start";
            montagePose.GetAnimationClip().AddEvent(aniEvent);
            
            var aniEvent2 = new AnimationEvent();
            aniEvent2.time = end;
            aniEvent2.functionName = "TriggerTimelineEvent";
            aniEvent2.stringParameter = "VaildCollsionPlayableAsset_End";
            montagePose.GetAnimationClip().AddEvent(aniEvent2);
        }

        [Inject] private ActorCollsionMgr _collsionMgr;
        [Inject] private AudioMgr _audioMgr;
        [SerializeField] private bool isOnlyBlendUpperPose = false;

        public void TriggerTimelineEvent(string input)
        {
            // Debug.Log("动画Timeline事件触发"+input);

            bool result = false;

            if (input == "VaildCollsionPlayableAsset_Start")
            {
                if(montageEventCallback!=null)
                    montageEventCallback(MontageEventEnum.VaildCollsionPlayableAssetStart, "");
                result = true;
            }else if (input == "VaildCollsionPlayableAsset_End")
            {
                if(montageEventCallback!=null)
                    montageEventCallback(MontageEventEnum.VaildCollsionPlayableAssetEnd, "");
                result = false;
            }
            

        }

    #endregion
    
    #region RegionEditorConfig

#if UNITY_EDITOR
    
    [MenuItem("Sunsgo/CreateCharacterAnimationConfig")]
    private static void CreateEssentialComponet()
    {
        var parent = (Selection.activeObject as WeaponDataAsset);

        Type[] needToCreate = new Type[]
        {
            typeof(ActorAbilityMgr.NormalAttackAbility),
        };
        
        foreach (var pair in parent.abilityAnimClipsPair)
        {
            // Application.data
            // AssetDatabase.is
            // if (needToCreate.Any((t) => pair.actorAbilityName == t.Name))
            {
                foreach (var clip in pair.clips)
                {
                    string assetPath = "Assets/DungeonSlayer/Res/Character/Resources/SkillConfig/" + clip.name+ ".asset";

                    if (AssetDatabase.LoadAssetAtPath<TimelineAsset>(assetPath) != null)
                    {
                        Debug.Log(assetPath+"::Exist");
                        continue;
                    }
                    // 创建一个TimelineAsset实例
                    TimelineAsset timelineAsset = TimelineAsset.CreateInstance<TimelineAsset>();
                    // 将TimelineAsset保存为.asset文件
                    
                    AssetDatabase.CreateAsset(timelineAsset, assetPath);
        
                    // 创建一个TrackAsset实例（此处以AudioTrack为例）
                    AnimationTrack audioTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "Animation Track");

                    
                    // 创建一个TimelineClip实例
                    TimelineClip timelineClip = audioTrack.CreateClip(clip);
                    
                    // 设置TimelineClip的属性（如开始时间、结束时间）
                    timelineClip.start = 0.0;
                    // timelineClip.end = clip.length;
                    
                    
                    PlayableTrack pbTrack = timelineAsset.CreateTrack<PlayableTrack>(null, "Animation Track");
                    var behaviourClip = pbTrack.CreateClip<VaildCollsionPlayableAsset>();
                    behaviourClip.start = 0;
                    behaviourClip.duration = clip.length;

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    
                }
            }
        }
    }
#endif

    #endregion
}

