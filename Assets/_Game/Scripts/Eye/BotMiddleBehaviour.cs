
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
            model.Distance = (mineBot.Position - closestElement.Position).magnitude;

            if (model.Distance > 35)
            {
                return BotState.Attack;
            }
            else if (model.Distance < model.AttackRadius)
            {
                if (mineBot.Force < (closestElement.Force * .5f))
                {
                    return BotState.GoAwayFromEnemy;
                }
                else
                {
                    return BotState.Attack;
                }
            }
        }

        return BotState.RandomWalk;
    }

    public BotMiddleBehaviour(BotBehaviourModel model) : base(model) { }
}