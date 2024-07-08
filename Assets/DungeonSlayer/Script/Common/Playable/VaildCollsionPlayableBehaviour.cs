using UnityEngine.Playables;

// A behaviour that is attached to a playable
namespace DungeonSlayer.Script.Common.Playable
{
    public class VaildCollsionPlayableBehaviour : PlayableBehaviour
    {
        // Called when the owning graph starts playing
        public override void OnGraphStart(UnityEngine.Playables.Playable playable)
        {
            // Debug.Log("Start");
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(UnityEngine.Playables.Playable playable)
        {
            // Debug.Log("End");
        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(UnityEngine.Playables.Playable playable, FrameData info)
        {
            // Debug.Log("Start1");

        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(UnityEngine.Playables.Playable playable, FrameData info)
        {
        
        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(UnityEngine.Playables.Playable playable, FrameData info)
        {
        
        }
    
    }
}
