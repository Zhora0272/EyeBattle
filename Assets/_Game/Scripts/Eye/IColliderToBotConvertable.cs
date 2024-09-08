using UnityEngine;

namespace Bot.BotController
{
    public interface IColliderToBotConvertable
    {
        public MineNavMeshAgentController SearchBotAnCollider(Collider collider);
    }
}