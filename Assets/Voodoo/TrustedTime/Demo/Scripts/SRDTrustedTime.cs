// docs at https://www.stompyrobot.uk/tools/srdebugger/documentation/#configuration

using System;
using System.ComponentModel;
using Voodoo.Utils;

public partial class SROptions
{
    [NonSerialized] private const string TRUSTEDTIME = "TrustedTime";

    [Category(TRUSTEDTIME)]
    public void ResetToNow()
    {
        
        TrustedTime.DEBUG_resetTime();
    }
    
    [Category(TRUSTEDTIME)]
    public void AddOneDay()
    {
        TrustedTime.DEBUG_addHours(24);
    }
    
    [Category(TRUSTEDTIME)]
    public void AddOneHour()
    {
        TrustedTime.DEBUG_addTime(TimeSpan.FromHours(1));
    }
    
    [Category(TRUSTEDTIME)]
    public void MinusOneHour()
    {
        TrustedTime.DEBUG_addHours(-1);
    }
}