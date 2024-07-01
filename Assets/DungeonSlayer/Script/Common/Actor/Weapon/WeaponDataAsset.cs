using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponDataAsset", menuName = "CuteAnimal/create Player Weapon Config")]
public class WeaponDataAsset : SerializedScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    public List<ActorStateMgr.ActorState> StatesToCreate = new List<ActorStateMgr.ActorState>();
    
    [TableList(ShowIndexLabels = true)]
    public List<AbilityAnimClipsPair> abilityAnimClipsPair;

    [Serializable]
    public class AbilityAnimClipsPair
    {
        
        private IEnumerable<string> GetListOfMonoBehaviours()
        {
            var parentType = typeof(ActorAbilityMgr.ActorAbility);
            
            List<string> subclasses = new List<string>();

            Assembly assembly = Assembly.GetAssembly(parentType);
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (type.IsSubclassOf(parentType))
                {
                    subclasses.Add(type.ToString());
                }
            }

            return subclasses.ToArray();
        }

        // public Type GetAbilityType()
        // {
        //     var parentType = typeof(ActorAbilityMgr.ActorAbility);
        //     
        //     List<string> subclasses = new List<string>();
        //
        //     Assembly assembly = Assembly.GetAssembly(parentType);
        //     Type[] types = assembly.GetTypes();
        //
        //     foreach (Type type in types)
        //     {
        //         // type
        //     }
        //
        //     return null;
        // }
        
        [ValueDropdown("GetListOfMonoBehaviours")]
        [TableColumnWidth(400, Resizable = false)]
        public string actorAbilityName;
        
        [HorizontalGroup("AnimationClip"), LabelWidth(22)]
        public List<AnimationClip> clips;

        [OnInspectorInit]
        private void CreateData()
        {
            
        }
    }
    
    
}
