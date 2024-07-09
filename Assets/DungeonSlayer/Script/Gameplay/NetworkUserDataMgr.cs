using Mirror;

namespace DungeonSlayer.Script.Gameplay
{
    public class NetworkUserDataMgr : NetworkBehaviour
    {
        [SyncVar] public int Score = 0;
        [SyncVar] public int AuthID;

        public override void OnStartServer()
        {
            AuthID = int.Parse(connectionToClient.authenticationData.ToString());
        }

        public int GetAuthID()
        {
            return AuthID;
        }

        public void RegisterActorMgr(ActorMgr actorMgr)
        {
            actorMgr.RegisterBattlerKillOther(OnKilledEnemy);
        }

        [ServerCallback]
        private void OnKilledEnemy(DamageInfo obj)
        {
            Score++;
        }
    }
}