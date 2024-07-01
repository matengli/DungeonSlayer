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

    // [Range(1, 50)] public float VisionRange = 3;

    [Range(0, 20)] public int Speed;
    [Range(0, 20)] public int Strength;
    [Range(0, 20)] public int Knowledge;
    [Range(0, 20)] public int Mind;

    public string name;
    public string desc;

}
