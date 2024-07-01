using System;
using System.Timers;
using R3;
using R3.Triggers;
using UnityEngine;
using Zenject;

public class GameBulletMgr : MonoBehaviour
{
    public class BulletModel
    {
        public GameObject prefab;
    }
    
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

    [Inject] private CameraController _cameraController;

    public void FireBullet(BulletModel model, ActorBattleMgr caster, Action<BulletObject,ActorMgr> hitCallback, float maxDistance=1000.0f)
    {
        var obj = Instantiate(model.prefab);
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.transform.position = caster.transform.position + Vector3.up*0.5f;
        obj.transform.rotation = caster.transform.rotation;
        
        var bulletObj = new BulletObject(obj, model, caster);
        var ignoreActor = caster.GetComponentInParent<ActorMgr>();

        var oldFollow = _cameraController.GetFollowTarget();
        _cameraController.SetFollowTarget(obj.transform);
        
        obj.GetComponentInChildren<BoxCollider>().OnTriggerEnterAsObservable().Subscribe((t) =>
        {
            ActorMgr hitActor = t.GetComponent<ActorMgr>();
        
            if(ignoreActor==hitActor)
                return;
            
            if(bulletObj.hasHit)
                return;
            
            bulletObj.hasHit = true;
            hitCallback(bulletObj, hitActor);
            _cameraController.SetFollowTarget(oldFollow);
        }).AddTo(obj);
        
        
        obj.transform.UpdateAsObservable().Subscribe((t) =>
        {
            obj.transform.position += obj.transform.forward * Time.deltaTime *50.0f;
            maxDistance -= Time.deltaTime * 50.0f;
            if (maxDistance <= 0)
            {
                hitCallback(bulletObj, null);
                _cameraController.SetFollowTarget(oldFollow);
                Destroy(obj);
            }
        }).AddTo(obj);
    }
}
