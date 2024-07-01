using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 是一个原型对象模式的对象类
/// 每个Entity的EntityID必定是唯一的
/// Entity可能是Actor,物品,或者其他的一些什么东西
/// </summary>
public class EntityMgr : MonoBehaviour
{
    private int ID = 0;
    private ActorMgr obj;
}
