using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using Pathfinding;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;

/// <summary>
/// 角色的Installer
/// </summary>
public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterModelBase _characterModelBase;
    
    public override void InstallBindings()
    {
        foreach (var pType in chaRelatedTypes)
        {
            var com = GetComponentInChildren(pType);
            if (com != null)
            {
                Container.Bind(pType).FromInstance(com).AsSingle().NonLazy();
            }
        }

        var obj = Instantiate(_characterModelBase.ModelPrefab, GetComponentInChildren<ActorViewContainer>().transform);
        
        Container.Bind<GameObject>().WithId("ViewGameObject").FromInstance(obj).NonLazy();
        Container.Bind<Transform>().WithId("ViewRootTransform").FromInstance(obj.transform.Find(_characterModelBase.rootTransformPath)).NonLazy();
        Container.Bind<CharacterModelBase>().FromInstance(_characterModelBase).AsSingle().NonLazy();
        Container.Bind<Animator>().FromInstance(obj.GetComponent<Animator>()).AsSingle().NonLazy();

        AfterInstalling();
    }

    private void AfterInstalling()
    {
        // GetComponent<ActorMgr>().Init();
        
        foreach (var ptype in chaRelatedTypes)
        {
           var method = ptype.GetMethod("Init");
           if (method != null)
           {
               method.Invoke(GetComponentInChildren(ptype), null);
           }
        }
    }
    
    static public readonly List<Type> chaRelatedTypesOnParent = new List<Type>()
    {
        typeof(ActorMgr),
        typeof(CharacterInstaller),
        typeof(GameObjectContext),
        typeof(Seeker),
        typeof(KinematicCharacterMotor),
        typeof(Rigidbody),
    };

    static public readonly List<Type> chaRelatedTypes = new List<Type>()
    {
        typeof(ActorMgr),
        typeof(ActorAbilityMgr),
        typeof(ActorAnimMgr),
        typeof(ActorAttributeMgr),
        typeof(ActorBattleMgr),
        typeof(ActorBindMgr),
        typeof(ActorCampMgr),
        typeof(ActorCollsionMgr),
        typeof(ActorCombatMgr),
        typeof(ActorModelMgr),
        typeof(ActorMoveMgr),
        typeof(ActorStateMgr),
        typeof(ActorUIContainer),
        typeof(ActorViewContainer),
        typeof(AutoActorController),
    };

    static public readonly Dictionary<Type, List<Type>> addtionalComponentDict = new Dictionary<Type, List<Type>>()
    {
        {typeof(ActorMoveMgr), new List<Type>(){ typeof(KCCMoveAgent) }},
    };

#if UNITY_EDITOR
    
    [MenuItem("Sunsgo/CreateCharacterEssentialComponet")]
    private static void CreateEssentialComponet()
    {
        Debug.Log("Create");
        var parent = (Selection.activeObject as GameObject).transform;
        
        foreach (var ptype in chaRelatedTypesOnParent)
        {
            if (parent.GetComponent(ptype) == null)
            {
                parent.gameObject.AddComponent(ptype);
            }
        }
        
        foreach (var ptype in chaRelatedTypes)
        {
            if (parent.GetComponentInChildren(ptype) == null)
            {
                var objc = new GameObject(ptype.ToString());
                objc.AddComponent(ptype);

                objc.transform.parent = parent;
                objc.transform.localPosition = Vector3.zero;
                objc.transform.localRotation = Quaternion.identity;
                objc.transform.localScale = Vector3.one;
                
                Debug.Log($"{ptype}:{addtionalComponentDict.ContainsKey(ptype)}");
                Debug.Log($"{ptype}=={typeof(ActorMoveMgr)==ptype}");
                
                if(!addtionalComponentDict.ContainsKey(ptype))
                    continue;

                foreach (var addPtype in addtionalComponentDict[ptype])
                {
                    if (objc.GetComponentInChildren(addPtype) == null)
                        objc.AddComponent(addPtype);
                }
                
            }
        }
        
        GameObjectContext com = parent.gameObject.GetComponent<GameObjectContext>();
        com.Installers = new List<MonoInstaller>(){parent.GetComponent<CharacterInstaller>()};
    }
#endif
    
    
}
