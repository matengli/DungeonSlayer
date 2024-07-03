using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

/// <summary>
/// 管理属性
/// 属性是一种可以增减的值类型
/// </summary>
public class ActorAttributeMgr : NetworkBehaviour
{
   [SerializeField] private Dictionary<string,ActorAttribute> attributeDict;
   //sample:"hp:0,100|sp:100,1000|mp:1000,10000"
   [SyncVar(hook = nameof(DecodeDataString))] [SerializeField] public string DataString = "";

   [Server]
   public void EncodeDataString()
   {
       string result = "";
       var keyCol = attributeDict.Keys.ToList();
       keyCol.Sort();
       
       foreach (var key in keyCol)
       {
           result += $"{key}:{attributeDict[key].val},{attributeDict[key].maxVal}";
           if (keyCol.IndexOf(key) != keyCol.Count - 1)
               result += "|";
       }

       DataString = result;
   }
   
   public void DecodeDataString(string _old,string _new)
   {
       if(_new.Length<=0)
           return;
       
       DataString = _new;
       var arr = DataString.Split("|");
       foreach (var dataStr in arr)
       {
           var spArr = dataStr.Split(":");
           var name = spArr[0];
           var factorStr = spArr[1];
           var group = factorStr.Split(",");
           var cur = float.Parse(group[0]);
           var max = float.Parse(group[1]);

           var old = GetVal(name);
           var oldMax = GetMaxVal(name);

           if (attributeDict == null || !attributeDict.ContainsKey(name))
           {
               CreateAttribute(name, max, cur);
               continue;
           }
           
           if(old!=cur)
               RealSetVal(name, cur);
           
           if(oldMax!=max)
               RealSetMaxVal(name, max);
       }
   }

    public void CreateAttribute(string name, float maxVal, float curVal)
    {
        if (attributeDict == null)
            attributeDict = new Dictionary<string, ActorAttribute>();
        
        if (attributeDict.ContainsKey(name))
        {
            // throw new Exception($"你已经有了这个Attribute了{name}");
            return;
        }

        var attr = new ActorAttribute();
        attr.name = name;
        attr.maxVal = maxVal;
        
        attr.val = curVal;
        
        attributeDict.Add(name, attr);
        
        if (OnModifyAttrEvent != null)
            OnModifyAttrEvent(name, attributeDict[name].val, attributeDict[name].val, attributeDict[name].maxVal);

    }
    
    public void CreateAttribute(string name, float maxVal)
    {
        CreateAttribute(name, maxVal, maxVal);
    }

    [Serializable]
    public class ActorAttribute
    {
        public string name = "";
        public float val = 0.0f;
        public float maxVal = 100.0f;
    }

    public event Action<string, float, float, float> OnModifyAttrEvent;

    public float GetMaxVal(string name)
    {
        if (attributeDict == null)
            return 0;

        return attributeDict[name].maxVal;
    }

    public float GetVal(string name)
    {
        if (attributeDict == null)
            return 0;

        return attributeDict[name].val;
    }

    [Server]
    public void SetVal(string name, float value)
    {
        RealSetVal(name, value);
        
        EncodeDataString();
    }
    

    public void RealSetVal(string name, float value)
    {
        if (attributeDict == null || !attributeDict.ContainsKey(name))
        {
            return;
        }

        var old = attributeDict[name].val;
        attributeDict[name].val = Mathf.Min(value, attributeDict[name].maxVal);
        
        if (OnModifyAttrEvent != null)
            OnModifyAttrEvent(name, attributeDict[name].val, old, attributeDict[name].maxVal);

    }
    
    public void RealSetMaxVal(string name, float value)
    {
        if (attributeDict == null)
            return;

        var old = attributeDict[name].val;
        attributeDict[name].maxVal = value;
        
        if (OnModifyAttrEvent != null)
            OnModifyAttrEvent(name, attributeDict[name].val, old, attributeDict[name].maxVal);

    }

    public string[] GetAttrNames()
    {
        return attributeDict.Keys.ToArray();
    }

    public float GetValPercent(string name)
    {
        return attributeDict[name].val / attributeDict[name].maxVal;
    }
    
}
