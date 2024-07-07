using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetEnemySpawner : NetworkBehaviour
    {
        [Serializable]
        public class EnemeyGroupConfig
        {
            public GameObject prefab;
            public int Count = 1;
            public bool isRandomPosition = true;
        }
        
        [SerializeField]private int isGenerated = 0;
        [SerializeField] private bool isGeneratedOnce = true;
        
        [SerializeField] private float GenerateRange = 10.0f;
        [SerializeField] private ActorCampMgr.ActorCamp actorCamp;
        
        [SerializeField] private List<EnemeyGroupConfig> enemeyGroupConfig;
        
        private bool HasGenerated = false;

        public UnityEvent OnKilledAllEnemy;

        public UnityEvent OnTriggerCurrentEvent;
        
        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if(HasGenerated)
                return;
            
            if(isGenerated>0)
                return;

            int PositionIndex = 0;
            
            foreach (var group in enemeyGroupConfig)
            {
                var generatedCharacter = group.prefab;
                
                for (int i = 0; i < group.Count; i++)
                {
                    var pos = UnityEngine.Random.insideUnitSphere * GenerateRange + transform.position;

                    pos.y = 0;

                    if (!group.isRandomPosition)
                    {
                        pos = transform.GetChild(PositionIndex).position;
                        PositionIndex++;
                    }
                    
                    var ball = Instantiate(group.prefab, pos, Quaternion.identity);
                
                    NetworkServer.Spawn(ball);
                    
                    var spawnedEnemy = ball.GetComponent<ActorMgr>();

                    spawnedEnemy.GetComponentInChildren<ActorBattleMgr>().OnKilled += OnSpawnedEnemyGetKilled;
                
                    spawnedEnemy.SetActorCamp(actorCamp);

                    isGenerated++;
                }
            }

            if(isGeneratedOnce)
                HasGenerated = true;

            RPC_InvokeOnTrggerStartEvent();
        }

        private void OnSpawnedEnemyGetKilled(DamageInfo obj)
        {
            isGenerated--;

            if (isGenerated == 0)
                RPC_InvokeKillEnemyEvent();
        }

        [ClientRpc]
        private void RPC_InvokeKillEnemyEvent()
        {
            InvokeKillEnemyEvent();
        }

        private void InvokeKillEnemyEvent()
        {
            OnKilledAllEnemy?.Invoke();
        }
        
        [ClientRpc]
        private void RPC_InvokeOnTrggerStartEvent()
        {
            InvokeOnTrggerStartEvent();
        }

        private void InvokeOnTrggerStartEvent()
        {
            OnTriggerCurrentEvent?.Invoke();
        }

        [Command(requiresAuthority = false)]
        public void CMD_ResetSpawnerStatus(bool isRemoveAllListener)
        {
            //必须要触发过才行
            if(!HasGenerated)
                return;
            
            HasGenerated = false;
            if (isRemoveAllListener)
            {
                RPC_RemoveAllListeners();
            }

        }

        [ClientRpc]
        public void RPC_RemoveAllListeners()
        {
            RemoveAllListeners();
        }

        public void RemoveAllListeners()
        {
            OnKilledAllEnemy.RemoveAllListeners();
            OnTriggerCurrentEvent.RemoveAllListeners();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, GenerateRange);
        }
    }
}