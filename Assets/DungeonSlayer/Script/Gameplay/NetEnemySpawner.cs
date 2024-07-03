using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetEnemySpawner : NetworkBehaviour
    {
        private int isGenerated = 0;

        private ActorMgr spawnedEnemy = null;

        [SerializeField] private int GeneratedCharacterCount = 1;
        [SerializeField] private float GenerateRange = 10.0f;
        
        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if(isGenerated>0)
                return;

            for (int i = 0; i < GeneratedCharacterCount; i++)
            {
                var pos = UnityEngine.Random.insideUnitSphere * GenerateRange + transform.position;

                pos.y = 0;

                var ball = Instantiate(FindObjectOfType<DungeonSlayerNetworkManager>().spawnPrefabs.Find(prefab => prefab.name == "CommonEnemyNet"), pos, Quaternion.identity);
                NetworkServer.Spawn(ball);

                spawnedEnemy = ball.GetComponent<ActorMgr>();
                spawnedEnemy.GetComponentInChildren<ActorBattleMgr>().OnKilled += OnSpawnedEnemyGetKilled;
                
                spawnedEnemy.SetActorCamp(Random.Range(0,1.0f)>0.5f? ActorCampMgr.ActorCamp.Dire: ActorCampMgr.ActorCamp.Radiant);

                isGenerated++;
            }
            

        }

        private void OnSpawnedEnemyGetKilled(DamageInfo obj)
        {
            NetworkServer.Destroy(obj.Defender.transform.parent.gameObject);

            isGenerated--;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, GenerateRange);
        }
    }
}