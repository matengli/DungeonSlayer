using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonSlayer.Script.Common.Game.Map
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapBreakableWall : NetworkBehaviour
    {
        public UnityEvent OnBreak;

        public override void OnStartServer()
        {
            gameObject.layer = LayerMask.NameToLayer("Character");
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.GetMask("Projectile"))
            {
                RPC_TriggerBreakEvent();
                return; 
            }
        }

        [ClientRpc]
        public void RPC_TriggerBreakEvent()
        {
            TriggerBreakEvent();
        }

        private void TriggerBreakEvent()
        {
            OnBreak?.Invoke();
            gameObject.SetActive(false);
        }
    }
}