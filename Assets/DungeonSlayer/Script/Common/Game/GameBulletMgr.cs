using System;
using System.Timers;
using DungeonSlayer.Script.Common.Game;
using R3;
using R3.Triggers;
using UnityEngine;
using Zenject;

/// <summary>
/// 子弹管理类，一切需要进行碰撞判定的飞行道具都由这个工厂生产
/// 目前没实现对象池
/// </summary>
public class GameBulletMgr : MonoBehaviour
{
    
    public class BulletObject
    {
        public GameObject go;
        public BulletModel model;
        public ActorBattleMgr caster;
        public bool hasHit;

        public BulletObject(GameObject _go, BulletModel _model, ActorBattleMgr _caster)
        {
            go = _go;
            model = _model;
            caster = _caster;
            hasHit = false;
        }
    }
    
    //TODO 对象池
    public void FireBullet(BulletModel model, ActorBattleMgr caster, Action<BulletObject,ActorMgr> hitCallback, float maxDistance=1000.0f)
    {
        var obj = Instantiate(model.prefab);
        obj.layer = model.prefab.layer;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.transform.position = caster.transform.position + Vector3.up*0.5f;
        obj.transform.rotation = caster.transform.rotation;
        
        var bulletObj = new BulletObject(obj, model, caster);
        var ignoreActor = caster.GetComponentInParent<ActorMgr>();
        
        obj.GetComponentInChildren<BoxCollider>().OnTriggerEnterAsObservable().Subscribe((t) =>
        {
            ActorMgr hitActor = t.GetComponent<ActorMgr>();
        
            if(ignoreActor==hitActor)
                return;
            
            if(ignoreActor.GetActorCamp() == hitActor.GetActorCamp())
                return;
            
            if(bulletObj.hasHit)
                return;
            
            bulletObj.hasHit = true;
            hitCallback(bulletObj, hitActor);
        }).AddTo(obj);
        
        
        obj.transform.UpdateAsObservable().Subscribe((t) =>
        {
            obj.transform.position += obj.transform.forward * Time.deltaTime * model.speed;
            maxDistance -= Time.deltaTime * model.speed;
            if (maxDistance <= 0)
            {
                hitCallback(bulletObj, null);
                Destroy(obj);
            }
        }).AddTo(obj);
    }
}
