using UnityEngine;
using UnityEngine.Playables;

namespace DungeonSlayer.Script.Common.Playable
{
    [System.Serializable]
    public class VaildCollsionPlayableAsset : PlayableAsset
    {
        [SerializeField] private string traceSocketName;
        // Factory method that generates a playable based on this asset
        public override UnityEngine.Playables.Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<VaildCollsionPlayableBehaviour>.Create(graph);
        
            return playable;
        }
    }
}
