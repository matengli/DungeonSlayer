using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;

public class GamePlayAIAgent : MonoBehaviour
{
    [Header("PathFinding")]
    public Path path;
    public float speed = 2;
    public float nextWaypointDistance = 3;
    public int currentWaypoint = 0;
    public bool reachedEndOfPath;

    public bool HasPathToWalk()
    {
        return path != null;
    }

    public void ClearPath()
    {
        path = null;
        GamePlayCharacterController.PlayerCharacterInputs input = new GamePlayCharacterController.PlayerCharacterInputs();

        input.MoveAxisForward = 0;
        input.MoveAxisRight = 0;
        input.CameraRotation = transform.rotation;

        // Apply inputs to character
        GetComponent<GamePlayCharacterController>().SetInputs(ref input);
        // We have no path to follow yet, so don't do anything
    }
    
    /// <summary>
    /// 直接设置到某个具体位置
    /// </summary>
    /// <param name="dest"></param>
    public void SetMoveDest(Vector3 dest)
    {
        // Get a reference to the Seeker component we added earlier
        Seeker seeker = GetComponent<Seeker>();

        // Start to calculate a new path to the targetPosition object, return the result to the OnPathComplete method.
        // Path requests are asynchronous, so when the OnPathComplete method is called depends on how long it
        // takes to calculate the path. Usually it is called the next frame.
        seekCount += 1;
        
        seeker.StartPath(transform.position, dest, OnPathComplete);
    }

    private int seekCount = 0;
    public bool HasReachDest()
    {
        if (seekCount>0)
            return false;
        
        if (path == null)
            return true;
        
        return false;
    }
    
    private void OnPathComplete(Path p)
    {
        seekCount -= 1;
        
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
    }

    private void Update()
    {
        if (path == null) {

            return;
        }

        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true) {
            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;

                    ClearPath();

                    return;
                }
            } else {
                break;
            }
        }

        // Slow down smoothly upon approaching the end of the path
        // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;

        // Direction to the next waypoint
        // Normalize it so that it has a length of 1 world unit
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        // Multiply the direction by our desired speed to get a velocity
        Vector3 velocity = dir * speed * speedFactor;

        var ctr = GetComponent<GamePlayCharacterController>();

        GamePlayCharacterController.PlayerCharacterInputs characterInputs = new GamePlayCharacterController.PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = velocity.magnitude/ctr.MaxStableMoveSpeed;
        characterInputs.MoveAxisRight = 0;
        characterInputs.CameraRotation = Quaternion.LookRotation(velocity.normalized);

        // Apply inputs to character
        
        // Just some point far away
        var targetPoint = transform.position + velocity;

        // Set the desired point to move towards using a desired speed of 10 and a max speed of 12
        controller.SetTarget(targetPoint, 10, 12, targetPoint);

        // Calculate how much to move during this frame
        // This information is based on movement commands from earlier frames
        // as local avoidance is calculated globally at regular intervals by the RVOSimulator component
        var delta = controller.CalculateMovementDelta(transform.position, Time.deltaTime);
        characterInputs.CameraRotation = Quaternion.LookRotation(delta.normalized);
        
        Debug.Log(delta);

        rvoPosition = transform.position + delta;
        
        ctr.SetInputs(ref characterInputs);
    }

    private Vector3 rvoPosition;
    
    RVOController controller;

    // Use this for initialization
    void Awake () {
        controller = GetComponent<RVOController>();
    }

    private void OnDrawGizmos()
    {
        if(path==null)
            return;
        
        foreach (var item in path.vectorPath)       
        {
            if(path.vectorPath.IndexOf(item) == currentWaypoint)
                Gizmos.color = Color.green;
            Gizmos.DrawSphere(item, 0.1f);
            Gizmos.color = Color.red;
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rvoPosition, 0.3f);

    }
}
