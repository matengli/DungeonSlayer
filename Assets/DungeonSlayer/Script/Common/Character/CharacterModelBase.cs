using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[CreateAssetMenu(fileName = "CharacterModel", menuName = "Sunsgo/Character/create Character Config")]
public class CharacterModelBase : ScriptableObject
{
    public float InitMoveSpeed { get =>  4+0.2f*Speed; }
    public float InitMaxHp { get => Strength*5;}
    
    public Weapon initWeapon;
    
    public GameObject ModelPrefab;

    public string rootTransformPath;
    
    [Range(0, 20)] public int Speed;
    [Range(0, 20)] public int Strength;

    public string desc;

    [Range(0,20)]public float SearchRange = 5.0f;
}
