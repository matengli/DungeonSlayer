using System;
using UnityEngine;

namespace DungeonSlayer.Script.Gameplay
{
    public class ActorMoveMgrSimple : ActorMoveMgr
    {
        
        // Start is called before the first frame update
        void Start()
        {
            speed = _modelMgr.GetSpeed();
            _battleMgr.OnKilled += OnKilled;
        }

        private Vector3 velocity = Vector3.zero;

        public override Vector3 GetVelocity()
        {
            return velocity;
        }

        protected override void AppleTranslateAfterCheckCurrentPath(float distanceToWaypoint)
        {
            // Slow down smoothly upon approaching the end of the path
            // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

            // Direction to the next waypoint
            // Normalize it so that it has a length of 1 world unit
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            // Multiply the direction by our desired speed to get a velocity
            Vector3 velocity = dir * speedFactor;
            
            KCCMoveAgent.PlayerCharacterInputs characterInputs = new KCCMoveAgent.PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = velocity.magnitude;
            characterInputs.MoveAxisRight = 0;
            characterInputs.CameraRotation = Quaternion.LookRotation(velocity.normalized);

            SetInputs(ref characterInputs);
        }

        public override void SetInputs(ref KCCMoveAgent.PlayerCharacterInputs input)
        {
            transform.parent.rotation = input.CameraRotation;
            
            transform.parent.position += speed * Time.deltaTime * ((input.MoveAxisForward) * transform.parent.forward +
                                                                   input.MoveAxisRight * transform.parent.right);

            velocity = speed * ((input.MoveAxisForward) * transform.parent.forward +
                                input.MoveAxisRight * transform.parent.right);
        }
    }
}