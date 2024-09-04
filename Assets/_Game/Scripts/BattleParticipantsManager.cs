using System.Collections.Generic;
using UnityEngine;

public class BattleParticipantsManager : MonoManager
{
    private List<INpcParameters> _participants = new();
    private Dictionary<int, List<INpcParameters>> _gameParticipants = new();

    public void Register(INpcParameters param)
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

    public void UnRegister(INpcParameters param)
    {
        if (_gameParticipants.TryGetValue(param.ClanId, out var result))
        {
            result.Remove(param);
        }
    }

    public bool GetClosest(INpcParameters mineNpcParameters, out INpcParameters result)
    {
        float minDistance = Mathf.Infinity;

        INpcParameters closestParticipant = null;
        
        foreach (var value in _gameParticipants.Values)
        {
            if (value.Count > 0)
            {
                if (value[0] == null) continue;
                if (value[0].ClanId == mineNpcParameters.ClanId) continue;

                foreach (var item in value)
                {
                    if (item == mineNpcParameters) continue;

                    var distance = (item.EyeTransform.IPosition - mineNpcParameters.EyeTransform.IPosition).magnitude;

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