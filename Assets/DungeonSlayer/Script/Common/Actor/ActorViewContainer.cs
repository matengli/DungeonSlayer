using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Zenject;
using Vector3 = System.Numerics.Vector3;

/// <summary>
/// 主要指责是处理内容物的可见性
/// </summary>
public class ActorViewContainer : MonoBehaviour
{
    [Inject(Id = "ViewGameObject")] private GameObject viewGameObject;

    public void SetViewActive(bool status)
    {
        viewGameObject.transform.localScale = status ? UnityEngine.Vector3.one : UnityEngine.Vector3.zero;
    }

    public GameObject GetViewObject()
    {
        return viewGameObject;
    }
}
