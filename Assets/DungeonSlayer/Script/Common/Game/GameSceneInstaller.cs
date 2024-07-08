using System;
using System.Collections;
using System.Collections.Generic;
using DungeonSlayer.Script.Common.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

/// <summary>
/// 全局通用的一些Installer
/// </summary>
public class GameSceneInstaller : MonoInstaller
{
    [Title("全局游戏的配置文件")][SerializeField] private GameConfig _gameConfig;
    
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

        Container.Bind<GameConfig>().FromInstance(_gameConfig).AsSingle().NonLazy();
    }
}
