using System.Collections.Generic;
using UnityEngine;

public class BattleParticipantsManager : MonoManager
{
    private List<IEyeParameters> _participants = new();
    private Dictionary<int, List<IEyeParameters>> _gameParticipants = new();

    public void Register(IEyeParameters param)
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

    public void UnRegister(IEyeParameters param)
    {
        if (_gameParticipants.TryGetValue(param.ClanId, out var result))
        {
            result.Remove(param);
        }
    }

    public bool GetClosest(IEyeParameters mineEyeParameters, out IEyeParameters result)
    {
        float minDistance = Mathf.Infinity;

        IEyeParameters closestParticipant = null;
        
        foreach (var value in _gameParticipants.Values)
        {
            if (value.Count > 0)
            {
                if (value[0] == null) continue;
                if (value[0].ClanId == mineEyeParameters.ClanId) continue;

                foreach (var item in value)
                {
                    if (item == mineEyeParameters) continue;

                    var distance = (item.EyeTransform.IPosition - mineEyeParameters.EyeTransform.IPosition).magnitude;

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