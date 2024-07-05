using System;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game.Map
{
    public class MapTriggerItem : NetworkBehaviour
    {
        private bool isGenerated = false;
        [SerializeField] public string addStateName;
        [SerializeField] private float time = 2.0f;

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<ActorMgr>()== null)
                return;

            var actor = other.GetComponent<ActorMgr>();

            SyncPerformStunForSeconds(actor, time);
            
        }

        private async UniTask SyncPerformStunForSeconds(ActorMgr actorMgr, float time)
        {
            actorMgr.RPC_PerformStateByName(addStateName);

            await UniTask.WaitForSeconds(time);
            
            actorMgr.RPC_PerformStateByName("idle");

        }
    }
}