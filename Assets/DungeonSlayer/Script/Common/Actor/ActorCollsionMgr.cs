using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// 用来处理碰撞,主要是战斗时候的武器Trace
/// </summary>
public class ActorCollsionMgr : MonoBehaviour
{
    private HashSet<TraceComponent> _traceComponents;
    public void InitTraceObject(GameObject weapon)
    {
        if(weapon.GetComponentInChildren<BoxCollider>()==null)
            return;
        
        weapon.GetComponentInChildren<BoxCollider>().gameObject.AddComponent<TraceComponent>();
        var trace = weapon.GetComponentInChildren<TraceComponent>();
        trace.SetHandler(this);
        
        if (_traceComponents == null)
            _traceComponents = new HashSet<TraceComponent>();
        _traceComponents.Add(trace);

        weapon.GetComponentInChildren<BoxCollider>().enabled = false;
    }


    [Inject] private ActorStateMgr _stateMgr;
    [Inject] private ActorMgr _actorMgr;
    
    [Inject] private ActorCampMgr _campMgr;

    public void TraceTriggerEnter(TraceComponent traceComponent, Collider other, bool isIgnoreCamp = false)
    {
        if (_stateMgr.GetCurrentState().Name != "attack")
        {
            return;
        }

        Debug.Log("TriggerCommon:"+other);

        var actormgr = other.GetComponent<ActorMgr>();
        if(actormgr==null)
            return;
        
        Debug.Log("TriggerCommon:"+other);
        
        if(_campMgr.GetCamp()==actormgr.GetActorCamp() && !isIgnoreCamp)
            return;
        
        Debug.Log("TriggerCommon:"+other);

        _actorMgr.AttackWithCurWeapon(actormgr);
    }

    public void RemoveTraceObject(GameObject curWeaponGameObject)
    {
        _traceComponents.Remove(curWeaponGameObject.GetComponent<TraceComponent>());
    }
    
    public class TraceComponent : MonoBehaviour
    {
        private ActorCollsionMgr _handler;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Hurt:"+other.ToString());
            _handler.TraceTriggerEnter(this, other);
        }

        public void SetHandler(ActorCollsionMgr actorCollsionMgr)
        {
            _handler = actorCollsionMgr;
        }
    }

    private bool overLapResult;

    private bool HasTriggered = false;

    [Inject] private GameUtil _gameUtil;
    public void TriggerOverLap(bool status, float range, float rangeAngle)
    {
        // foreach (var com in _traceComponents)
        // {
        //     com.GetComponent<BoxCollider>().enabled = status;
        // }
        if(!status)
            return;
        
        var result = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Character"));
        // var result = _gameUtil.RaycastSectorRange(transform.position + Vector3.up, transform.forward, range, rangeAngle,
        //     LayerMask.GetMask("Character"), null);
        
        if (result == null || result.Length <= 1)
        {
            return;
        }
        
        foreach (var item in result)
        {
            if (item.transform.position == transform.position)
            {
                continue;
            }
            
            var hitPoint = new Vector3(item.transform.position.x, 0, item.transform.position.z);
            var startPoint = new Vector3(transform.position.x, 0, transform.position.z);
        
            var sd = Mathf.Cos(Mathf.Deg2Rad * rangeAngle * 0.5f);
            
            if (Vector3.Dot((hitPoint - startPoint).normalized , transform.forward) < sd)
            {
                continue;
            }
        
            Debug.Log(item);
            TraceTriggerEnter(null, item.GetComponent<Collider>());
        }
    }

    [Inject] private ActorBattleMgr _battleMgr;
    private void Start()
    {
        _battleMgr.OnKilled += DisableAllCollider;
    }

    private void DisableAllCollider(DamageInfo obj)
    {
        foreach (var collider in transform.parent.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
}
