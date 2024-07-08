using System;
using System.Collections.Generic;
using DungeonSlayer.Script.Common.Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Actor.Weapon
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Sunsgo/create Player Weapon")]
    public class Weapon : SerializedScriptableObject
    {
        public WeaponDataAsset _weaponDataAsset;

        [Title("强制修改动画的播放时间。默认为0，则表示不做任何修改，否则改为对应的值")][Range(0,5)]public float modifiedAniTime = 0;
        [Title("武器使用者的攻击间隔")]public float cd = 1.0f;
        [Title("攻击范围")]public float range = 1.0f;
        public float baseAtk = 10.0f;
        public GameObject weaponObject;
        [Title("武器所附着绑点的位置")]public string socketName = "";

        public string Desc = "";

        [Title("当这把武器被使用的时候会给使用者或者敌人上的Buff")]
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

        [ShowIf(nameof(isRaycastWeapon))] public BulletModel bulletModel;
        [Title("近战武器的攻击角度范围")] [HideIf(nameof(isRaycastWeapon))][Range(0,360)]public float rangeAngle = 45.0f;
    }
}
