using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

/// <summary>
/// 管理属性
/// 属性是一种可以增减的值类型
/// </summary>
public class ActorAttributeMgr : SerializedMonoBehaviour
{
   [SerializeField] private Dictionary<string,ActorAttribute> attributeDict;

    public void CreateAttribute(string name, float maxVal, float curVal=-1999.9f)
    {
        if (attributeDict == null)
            attributeDict = new Dictionary<string, ActorAttribute>();
        
        if (attributeDict.ContainsKey(name))
        {
            throw new Exception($"你已经有了这个Attribute了{name}");
            return;
        }

        var attr = new ActorAttribute();
        attr.name = name;
        attr.maxVal = maxVal;
        if (-1999.9f != curVal)
            curVal = maxVal;
        
        attr.val = curVal;
        
        attributeDict.Add(name, attr);
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

    public void SetVal(string name, float value)
    {
        if (attributeDict == null)
            return;

        var old = attributeDict[name].val;
        attributeDict[name].val = Mathf.Min(value, attributeDict[name].maxVal);
        
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
