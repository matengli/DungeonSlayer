using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class GameSceneInstaller : MonoInstaller
{
    
    static public readonly List<Type> CommonGlobalMgrTypes = new List<Type>()
    {
        typeof(GameBulletMgr),
        typeof(BuffMgr),
        typeof(DamageMgr),
        typeof(AudioMgr),
        typeof(CameraController),
        typeof(GameUtil),
        typeof(CinemachineCameraOffset),
    
    };
    public override void InstallBindings()
    {
        foreach (var pType in CommonGlobalMgrTypes)
        {
            if (!pType.IsSubclassOf(typeof(Component)))
            {
                Container.Bind(pType).FromNew().AsSingle().NonLazy();
                continue;
            }
            
            var com = FindObjectOfType(pType);
            if (com == null)
            {
                gameObject.AddComponent(pType);
                com = GetComponent(pType);
            }
            
            Container.Bind(pType).FromInstance(com).AsSingle().NonLazy();
        }
    }
}
