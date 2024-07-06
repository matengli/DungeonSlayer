using System;
using Cysharp.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game.Map
{
    public class MapTriggerItem : NetworkBehaviour
    {
        private bool isGenerated = false;
        [SerializeField] public string addStateName;
        [SerializeField] private float time = 2.0f;

        [SerializeField] private Weapon weaponModel;
        

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if(!CheckVaild(other))
                return;
            
            var actor = other.GetComponent<ActorMgr>();
            
            RPC_OnUpdateActor(other.GetComponent<ActorMgr>(), true);

            if(addStateName.Length>0)
                SyncPerformStunForSeconds(actor, time);
        }

        private bool CheckVaild(Collider other)
        {
            var actor = other.GetComponent<ActorMgr>();
            
            if(actor == null)
                return false;
            
            if(!actor.CompareTag("Player"))
                return false;

            return true;
        }

        [Server]
        private void OnTriggerExit(Collider other)
        {
            if(!CheckVaild(other))
                return;

            RPC_OnUpdateActor(other.GetComponent<ActorMgr>(), false);
        }

        [ClientRpc]
        private void RPC_OnUpdateActor(ActorMgr owner, bool status)
        {
            OnUpdateActor(owner, status);
        }

        private void OnUpdateActor(ActorMgr owner, bool status)
        {
            if(!owner.isOwned)
                return;

            isActive = status;
            transform.GetChild(0).GetComponent<TextMeshPro>().text = GetDesc();
            transform.GetChild(0).GetComponent<TextMeshPro>().enabled = status;
            self = status?owner:null;
        }

        private ActorMgr self;

        private bool isActive = false;

        private void Update()
        {
            if(!isActive)
                return;
            
            if(!Input.GetKeyUp(KeyCode.F))
                return;

            if (weaponModel != null)
            {
                self.CMD_EquipWeapon(weaponModel.name);
                CMD_SetActiveFalse();
            }
        }
        
        [Command(requiresAuthority = false)]
        private void CMD_SetActiveFalse()
        {
            RPC_SetActiveFalse();
        }

        [ClientRpc]
        private void RPC_SetActiveFalse()
        {
            SetActiveFalse();
        }

        private void SetActiveFalse()
        {
            gameObject.SetActive(false);
            isActive = false;
        }

        private string GetDesc()
        {
            if (weaponModel != null)
            {
                return $"{weaponModel.Desc}\n[F] Pick Up";
            }

            return "";
        }

        private async UniTask SyncPerformStunForSeconds(ActorMgr actorMgr, float time)
        {
            actorMgr.RPC_PerformStateByName(addStateName);

            await UniTask.WaitForSeconds(time);
            
            actorMgr.RPC_PerformStateByName("idle");

        }

        private void LateUpdate()
        {
            transform.GetChild(0).transform.forward = Camera.main.transform.forward;
        }
    }
}