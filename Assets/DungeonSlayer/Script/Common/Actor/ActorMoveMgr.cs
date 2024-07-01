using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
// using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// 负责管理移动
/// 注意，当前的transform并不是真正的navmeshagent所在的transform
/// </summary>
public class ActorMoveMgr : MonoBehaviour
{
    // [Inject] private AIPath _navMeshAgent;
    [Inject] private ActorModelMgr _modelMgr;
    [Inject] private ActorBattleMgr _battleMgr;

    [SerializeField] private float rawMoveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rawMoveSpeed = _modelMgr.GetMoveSpeed();
        // _navMeshAgent.maxSpeed = _modelMgr.GetMoveSpeed();
        _battleMgr.OnKilled += OnKilled;
    }

    public void SetSpeedPercentage(float per)
    {
        // _navMeshAgent.maxSpeed = per * rawMoveSpeed;
    }

    [SerializeField] private float followDistance = 2.0f;

    // [Inject] private RoundBattleMgr _roundBattleMgr;
    [Inject] private ActorMgr _actorMgr;
    
    private void Update()
    {
        
        // if (_roundBattleMgr.IsBattle(_actorMgr))
        // {
        //     SetIsStopped(true);
        //     return;
        // }
        
        if(target==null)
            return;
        
        if((target.position-transform.position).magnitude<=followDistance)
            return;
        
        MoveToPosition(target.transform.position - target.forward*followDistance);
    }


    public void OnKilled(DamageInfo info)
    {
        // _navMeshAgent.enabled = false;
    }

    [SerializeField] private bool IsStopped = false;
    
    public void SetIsStopped(bool isStopped,bool isClearPath=false)
    {
        // _navMeshAgent.isStopped = isStopped;
        IsStopped = isStopped;
    }

    [SerializeField] private Vector3 destination;

    // [Inject] private BattlePathBlockMgr _battlePathBlockMgr;
    
    public void MoveToPosition(Vector3 input)
    {
        // _navMeshAgent.destination = input;
        destination = input;

        // _navMeshAgent.GetComponent<Seeker>().traversalProvider = _battlePathBlockMgr.GetTraversalProvider();
        Debug.Log(input);
        // _navMeshAgent.GetComponent<Seeker>().StartPath(transform.position, input, null);
    }

    public async UniTask MoveToPositionAsync(Vector3 input, float endReachDistance = 0)
    {
        destination = input;

        // _navMeshAgent.GetComponent<Seeker>().StartPath(transform.position, input, null);
        //
        // _navMeshAgent.endReachedDistance = endReachDistance;
        //
        // await UniTask.WaitUntil(()=>_navMeshAgent.reachedDestination);
        //
        // _navMeshAgent.endReachedDistance = 0;
        
    }
    
    public void SetLookAt(Transform transform)
    {
        // _navMeshAgent.transform.LookAt(transform);
        
    }

    [SerializeField]private Transform target;
    public void SetFollowTarget(Transform _target, float followDis=2.0f)
    {
        target = _target;
        followDistance = followDis;
    }

    public void EnableRotation(bool status)
    {
        // _navMeshAgent.enableRotation = status;
    }
    
    public Vector3 GetVelocity()
    {
        // return _navMeshAgent.velocity;
        return Vector3.zero;
    }
}
