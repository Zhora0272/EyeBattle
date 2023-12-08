
public class BotAggressiveBehaviour : BotBehaviourBase, IBotMonoBehaviour
{
    public BotAggressiveBehaviour(BotBehaviourModel model) : base(model) { }

    public BotState BotBehaviourUpdate(IEyeParameters mineBot, IEyeParameters closestElement)
    {
        if (closestElement != null)
        {
            return BotState.Attack;
        }

        return BotState.RandomWalk;
    }
}
public class BotMiddleBehaviour : BotBehaviourBase, IBotMonoBehaviour
{
    public BotState BotBehaviourUpdate(IEyeParameters mineBot, IEyeParameters closestElement)
    {
        if (closestElement != null)
        {
            var attackRadius = model.AttackRadius;

            distance = (mineBot.Position - closestElement.Position).magnitude;

            if (distance < attackRadius)
            {
                if (mineBot.Force < closestElement.Force)
                {
                    return BotState.GoAwayFromEnemy;
                }
                else
                {
                    return BotState.Attack;
                }
            }

            if (distance > attackRadius)
            {
                return BotState.RandomWalk;
            }
        }

        return BotState.Idle;
    }

    public BotMiddleBehaviour(BotBehaviourModel model) : base(model) { }
}