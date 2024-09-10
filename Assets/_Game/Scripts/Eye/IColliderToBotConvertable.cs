using UnityEngine;

namespace Bot.BotController
{
    public interface IColliderToBotConvertable
    {
        public MineNavMeshAgentController SearchBotWithCollider(Collider collider);
    }
}