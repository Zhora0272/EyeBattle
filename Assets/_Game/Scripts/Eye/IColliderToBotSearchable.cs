using UnityEngine;

namespace Bot.BotController
{
    public interface IColliderToBotSearchable
    {
        public void SearchBotAnCollider(Collider collider);
    }
}