using System;

namespace Bot.BotController
{
    [Serializable]
    public class EyeSpawnList
    {
        public BotType BotType;
        public int SpawnCount;
        public BotBehaviourModel BotModel;
    }
}