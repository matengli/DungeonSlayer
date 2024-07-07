using Sirenix.OdinInspector;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Sunsgo/create GameConfig")]
    public class GameConfig : SerializedScriptableObject
    {
        public enum DifficultyEnum
        {
            Easy,
            Normal,
            Hard,
        }
        
        [Title("是否展示位置等调试信息")] public bool IsShowDebugInfo;
        [Title("游戏难度")] public DifficultyEnum difficultyEnum;


    }
}