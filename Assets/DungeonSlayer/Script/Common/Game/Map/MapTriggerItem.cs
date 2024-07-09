using System;
using Cysharp.Threading.Tasks;
using DungeonSlayer.Script.Common.Actor.Weapon;
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
        
        [SerializeField] private MapItemModel mapItemModel;

        [SerializeField] private bool IsInstant = false;

        public override void OnStartClient()
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = GetDesc();

            if (IsInstant)
            {
                transform.GetChild(0).GetComponent<TextMeshPro>().enabled = true;
            }

        }

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
            
            if(!IsInstant)
                transform.GetChild(0).GetComponent<TextMeshPro>().enabled = status;
            
            self = status?owner:null;
            
            if (IsInstant && isActive)
                ApplyMapItem();
        }

        private ActorMgr self;

        [SyncVar] private bool isActive = false;

        private void Update()
        {
            if(!isActive)
                return;
            
            if(IsInstant)
                return;
            
            if(!Input.GetKeyUp(KeyCode.F))
                return;

            ApplyMapItem();
        }

        private void ApplyMapItem()
        {
            if (weaponModel != null)
            {
                self.CMD_EquipWeapon(weaponModel.name);
                var old = self.GetCurrentWeapon();
                // weaponModel = old;
                CMD_ChangeWeapon(old.name);
                transform.GetChild(0).GetComponent<TextMeshPro>().text = GetDesc();
            }
            
            if(mapItemModel != null)
            {
                self.CMD_AddBuff(mapItemModel.addBuffInfos);
                CMD_SetActiveFalse();
            }
        }
        
        [Command(requiresAuthority = false)]
        private void CMD_ChangeWeapon(string weapon)
        {
            RPC_ChangeWeapon(weapon);
        }
        
        [ClientRpc]
        private void RPC_ChangeWeapon(string weapon)
        {
            ChangeWeapon(weapon);
        }

        private void ChangeWeapon(string weapon)
        {
            weaponModel = Resources.Load<Weapon>($"Weapon/{weapon}");
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
            string end = IsInstant ? "" : "\n[F] Pick Up";
            
            if (weaponModel != null)
            {
                return $"{weaponModel.Desc}{end}";
            }
            
            if (mapItemModel != null)
            {
                return $"{mapItemModel.Desc}{end}";
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