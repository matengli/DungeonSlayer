using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Sunsgo/create Player Weapon")]
public class Weapon : SerializedScriptableObject
{
    public WeaponDataAsset _weaponDataAsset;
    public float cd = 1.0f;
    public float range = 1.0f;
    [Range(0,360)]public float rangeAngle = 45.0f;
    public float baseAtk = 10.0f;
    public GameObject weaponObject;
    public string socketName = "";

    [Title("当这把武器被使用的时候会给敌人上的Buff")]
    public List<WeaponAddBuffConfig> weaponAddBuffConfig;

    [Title("是否是远程武器")] public bool isRaycastWeapon = false;
    
    public enum WeaponBuffTarget
    {
        self,
        defender,
    }
    
    [Serializable]
    public class WeaponAddBuffConfig
    {
        public WeaponBuffTarget BuffTarget;
        public BuffBase.AddBuffInfo AddBuffInfo;
    }
}
