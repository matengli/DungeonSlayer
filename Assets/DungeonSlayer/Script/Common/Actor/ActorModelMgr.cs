using System;
using System.Collections;
using System.Collections.Generic;
using DungeonSlayer.Script.Common.Actor.Weapon;
using UnityEngine;
using Zenject;

/// <summary>
/// 用来管理角色配置
/// </summary>
public class ActorModelMgr : MonoBehaviour
{
    [Inject] private CharacterModelBase _modelBase;

    public string GetName()
    {
        return _modelBase.name;
    }

    public float GetMoveSpeed()
    {
        return _modelBase.InitMoveSpeed;
    }

    public int GetSpeed()
    {
        return _modelBase.Speed;
    }

    public CharacterModelBase GetModel()
    {
        return _modelBase;
    }

    public Weapon GetInitWeapon()
    {
        return _modelBase.initWeapon;
    }

    public float GetInitHp()
    {
        return _modelBase.InitMaxHp;
    }
}
