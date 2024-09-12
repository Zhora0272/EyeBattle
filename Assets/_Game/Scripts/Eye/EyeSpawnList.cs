using System;
using UnityEngine.Serialization;

namespace Bot.BotController
{
    [Serializable]
    public class EyeSpawnList
    {
        public int SpawnDelay;
        public BotType BotType;
        public int SpawnCount;
        public BotBehaviourModel BotModel;
        public EyeCustomizeModel EyeCustomize;
        
        internal int localSpawnCount;
    }
}