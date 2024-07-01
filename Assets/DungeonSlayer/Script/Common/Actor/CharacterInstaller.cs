using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterModelBase _characterModelBase;
    
    public override void InstallBindings()
    {
        foreach (var pType in chaRelatedTypes)
        {
            var com = GetComponentInChildren(pType);
            if(com!=null)
                Container.Bind(pType).FromInstance(com).AsSingle().NonLazy();
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
    
    [MenuItem("Sunsgo/CreateCharacterEssentialComponet")]
    private static void CreateEssentialComponet()
    {
        Debug.Log("Create");
        var parent = (Selection.activeObject as GameObject).transform;

        parent.AddComponent<CharacterInstaller>();

        bool isfirst = true;
        
        foreach (var ptype in chaRelatedTypes)
        {
            if (parent.GetComponentInChildren(ptype) == null)
            {
                if (isfirst)
                {
                    parent.gameObject.AddComponent(ptype);
                    isfirst = false;
                    continue;
                }
                var objc = new GameObject(ptype.ToString());
                objc.AddComponent(ptype);

                objc.transform.parent = parent;
                objc.transform.localPosition = Vector3.zero;
                objc.transform.localRotation = Quaternion.identity;
                objc.transform.localScale = Vector3.one;
            }
        }
        
        parent.gameObject.AddComponent<Rigidbody>();
        parent.gameObject.AddComponent<CapsuleCollider>();
        GameObjectContext com = parent.gameObject.AddComponent<GameObjectContext>();
        com.Installers = new List<MonoInstaller>(){parent.GetComponent<CharacterInstaller>()};
    }

    //如果有对Start初始化顺序敏感的可以放到这里控制
    private void Start()
    {
        
    }
    
    
}
