using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

/// <summary>
/// 用来做绑点的管理类
/// 其实就是一个映射Transform的类
/// </summary>
public class ActorBindMgr : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<string, Transform> binderDict = new Dictionary<string, Transform>();
    
    [Inject(Id = "ViewRootTransform")]
    private Transform rootTransform;
    
    private List<string> binderNameList;
    
    [Button("自动绑定给定的点")]
    private void AutoBindBinders()
    {
        if (rootTransform == null)
        {
            return;
        }
    
        binderDict = new Dictionary<string, Transform>();
        binderNameList = new List<string>();
    
        BindTransform(rootTransform);
    }
    
    private void BindTransform(Transform tf)
    {
        if (!binderNameList.Contains(tf.name))
        {
            binderDict.Add(tf.name, tf);
            binderNameList.Add(tf.name);
        }
    
        for (int i = 0; i < tf.childCount; i++)
        {
            BindTransform(tf.GetChild(i));
        }
    }

    /// <summary>
    /// 通过名字获取绑点
    /// </summary>
    public Transform GetBinderByName(string name)
    {
        if (!binderNameList.Contains(name))
            return null;

        return binderDict[name];
    }

    private void Awake()
    {
        AutoBindBinders();
    }
}
