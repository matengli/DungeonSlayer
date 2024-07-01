using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Zenject;
using Vector3 = System.Numerics.Vector3;

/// <summary>
/// 负责管理角色的模型
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
