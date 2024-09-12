using UnityEngine;

namespace Bot.BotController
{
    public interface IColliderToBotConvertable
    {
        public MoveableBattleParticipantBaseController SearchBotWithCollider(Collider collider);
    }
}