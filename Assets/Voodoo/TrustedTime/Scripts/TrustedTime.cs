using System;
using JetBrains.Annotations;
using UnityEngine;
using Voodoo.Network;


namespace Voodoo.Utils
{
    public class TrustedTime : MonoBehaviour
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly string ServerEndpoint = "https://time.voodoo-gaming.io:7350/v2/server_time";
        private static readonly float ServerRefreshInSec = 60 * 60 * 4; // 4 hours. 

        private static float _localSecs = 0f;
        private static long _epochSecs = 0;

        [PublicAPI] public static bool Trusted { get; private set; } = false;
        [PublicAPI] public static bool DebugUsed { get; private set; } = false;

        [PublicAPI] public static long Timestamp => (_epochSecs + (long) _localSecs);

        [PublicAPI] public static long TimestampFromNow(long secs)
        {
            return TimestampFromNow(TimeSpan.FromSeconds(secs));
        }
        
        [PublicAPI] public static long TimestampFromNow(TimeSpan span)
        {
            return (Timestamp + (long) span.TotalSeconds);
        }

        
        [PublicAPI] public static string ToString(long timestamp, string format)
        {
            return (UnixEpoch + TimeSpan.FromSeconds(timestamp)).ToString(format);
        }
        
        // returns 0 if it's in the past. 
        [PublicAPI] public static string GetRemainingTimeFormatted(long futureTime, string format = @"HH\:mm\:ss")
        {
            var secondsLeft = Math.Max(0,futureTime - Timestamp);
            return (UnixEpoch + TimeSpan.FromSeconds(secondsLeft)).ToString(format);
        }
        
        [PublicAPI] public static bool IsExpired(long futureTimestamp)
        {
            return (Timestamp > futureTimestamp);
        }

        private void Awake()
        {
            ResetTime();
        }
        
        private static void ResetTime()
        {
            Trusted = false;
            _epochSecs = (long) (DateTime.UtcNow - UnixEpoch).TotalSeconds;
            _localSecs = 0f;
            // init with local time, but force an update on the first frame by pretending it happened in the past.
            _epochSecs -= (long) ServerRefreshInSec;
            _localSecs += ServerRefreshInSec;
        }

        private void Update()
        {
            _localSecs += Time.deltaTime;

            if (_localSecs > ServerRefreshInSec)
            {
                WebRequest.Get(ServerEndpoint,
                    (result) =>
                    {
                        var res = JsonUtility.FromJson<TimeResponse>(result.downloadHandler.text);
                        _epochSecs = res.epoch_seconds;
                        Trusted = true;
                    },
                    (result) => { Debug.LogError($"ERROR in Trusted Time.{result.ToString()}"); });
                _epochSecs += (long) _localSecs; // in case we're offline fix the init state
                _localSecs = 0f;
            }
        }

        //
        // DEBUG functions that allow for testing things like daily offers/quest unlocks/etc.
        //
        public static void DEBUG_addTime(TimeSpan ts)
        {
            DebugUsed = true;
            _epochSecs += (long) ts.TotalSeconds;
        }

        public static void DEBUG_addHours(int hours)
        {
            DebugUsed = true;
            _epochSecs += hours * 60 * 60;
        }

        public static void DEBUG_resetTime()
        {
            ResetTime();
            DebugUsed = true;
        }
    }

    struct TimeResponse
    {
        public long epoch_seconds;
    }
}