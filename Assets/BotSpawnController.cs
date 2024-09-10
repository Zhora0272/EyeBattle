public class BotSpawnController
{
    private BotParameters _botParameters;
    
    internal bool TryUpdateParameters(out BotParameters? botParameters)
    {
        if (true) // ?? finance request to pay and complete operation
        {
            botParameters = UpdateBotParameters();
            return true;
        }
        else
        {
            botParameters = null;
            return false;
        }
    }
    
    private BotParameters UpdateBotParameters()
    {
        _botParameters.Health += 10;
        _botParameters.Shield += 5;
        _botParameters.AttackPower += 5;
        _botParameters.AttackRadius += 3;
        _botParameters.Speed += 2;

        return _botParameters;
    }
}