using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class VaildCollsionPlayableAsset : PlayableAsset
{
    [SerializeField] private string traceSocketName;
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<NewPlayableBehaviour>.Create(graph);
        
        return playable;
    }
}
