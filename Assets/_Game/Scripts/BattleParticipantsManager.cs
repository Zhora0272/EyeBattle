using System.Collections.Generic;
using UnityEngine;

public class BattleParticipantsManager : MonoManager
{
    private List<IBattleParticipantParameters> _participants = new();
    private Dictionary<int, List<IBattleParticipantParameters>> _gameParticipants = new();

    public void Register(IBattleParticipantParameters param)
    {
        if (_gameParticipants.TryGetValue(param.ClanId, out var result))
        {
            result.Add(param);
        }
        else
        {
            result = new(){param};
            _gameParticipants.Add(param.ClanId, result);
        }
    }

    public void UnRegister(IBattleParticipantParameters param)
    {
        if (_gameParticipants.TryGetValue(param.ClanId, out var result))
        {
            result.Remove(param);
        }
    }

    public bool GetClosest(IBattleParticipantParameters mineBattleParticipantParameters, out IBattleParticipantParameters result)
    {
        float minDistance = Mathf.Infinity;

        IBattleParticipantParameters closestParticipant = null;
        
        foreach (var value in _gameParticipants.Values)
        {
            if (value.Count > 0)
            {
                if (value[0] == null) continue;
                if (value[0].ClanId == mineBattleParticipantParameters.ClanId) continue;

                foreach (var item in value)
                {
                    if (item == mineBattleParticipantParameters) continue;

                    var distance = (item.BotTransform.IPosition - mineBattleParticipantParameters.BotTransform.IPosition).magnitude;

                    if (distance < minDistance)
                    {
                        closestParticipant = item;
                        minDistance = distance;
                    }
                }
            }
        }

        result = closestParticipant;

        return closestParticipant != null;
    }
}