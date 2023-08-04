# Trusted Time
## Purpose
A simple interface to get the server UTC time so users can't cheat the system. 

## Summary
Returns, and manipulates a Timestamp (it's really a `long` so you can serialize easily).  The time stamp comes from a server time so clients can't cheat it.  It is always in UTC (so you need to convert to local time if needed, though usually it's the delta you need not the actual time).

### Steps To Setup:
1. If using Casual Base Game then it's already set up.
2. If using DI system, create a new TrustedTime in your ProjectInstaller.cs
```
             FromNewComponent<TrustedTime>();
```
3. If using a basic Unity Set up, add the TrustedTime component to your scene (in a do not destroy section)

As extras a prefab to display the current time is included as are SRD buttons to help debug the internal time. 

### Examples: Daily reward chest example

    // set the next reward time for 24 hours from now. 
    long nextReward = TrustedTime.TimestampFromNow(24*60*60);
    // or 
    long nextReward = TrustedTime.TimestampFromNow(TimeSpan.FromHours(24));

    // in your update see if it's ready?
    bool IsClaimable = TrustedTime.IsExpired(nextReward);

    // otherwise show how long is left: 
    var text = TrustedTime.GetRemainingTimeFormatted(nextReward, @"H\:mm\:ss");

    // before awarding the chest check if we're in a trusted state (i.e. got time from the server or using the local time)
    bool possibleCheat = !TrustedTime.Trusted;

### API:

**Properties** - these are all statics so you can access them from anywhere. 
```
     static bool Trusted // true if server responded
     static bool DebugUsed // true if cheats were used
     static long Timestamp // seconds since unix epoch 
```

**Utility functions** - also statics so you can access them from anywhere.  
```
// returns a formatted timestamp 
static string ToString(long timestamp, string format)

// returns a timestamp from now (UTC time) plus the number of seconds specififed
static long TimestampFromNow(long secs)

// returns a timestamp from now (UTC time) plus a TimeSpan specififed
static long TimestampFromNow(TimeSpan span)

// returns a string of the time left (i.e. to open a chest) clampped to zero. 
static string GetRemainingTimeFormatted(long futureTime, string format = @"HH\:mm\:ss")

// true if futureTimestamp is in the past. 
static bool IsExpired(long futureTimestamp)
```

**Debug Functions** - so it's easier to test chests, daily rewards by changing the internal time.  Warning - rest on restart so if you're serializing debug dates be careful.  

```
static void DEBUG_addTime(TimeSpan ts)

static void DEBUG_addHours(int hours)

static void DEBUG_resetTime()
```
