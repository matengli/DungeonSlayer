using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DamageInfo
{
    private ActorBattleMgr attacker = null;
    private ActorBattleMgr defender = null;
    private List<string> tags;
    private float damage;
    public float rawDamage;
    // private List<string> addBuffs;

    public ActorBattleMgr Attacker
    {
        get => attacker;
        set => attacker = value;
    }

    public ActorBattleMgr Defender
    {
        get => defender;
        set => defender = value;
    }

    public DamageInfo(ActorBattleMgr attacker, ActorBattleMgr defender, float rawDamage)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.damage = rawDamage;
        this.rawDamage = rawDamage;
        
        tags = new List<string>();
    }

    public void AddDamageTag(string tag)
    {
        tags.Add(tag);
    }

    public bool HasDamageTag(string tag)
    {
        return tags.Contains(tag);
    }

    public virtual float Damage
    {
        get => damage;

        set => damage = value;
    }
    
    
}