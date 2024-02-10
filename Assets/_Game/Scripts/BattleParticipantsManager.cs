using System.Collections.Generic;
using UnityEngine;

public class BattleParticipantsManager : MonoManager
{
    private List<IEyeParameters> _participants = new();

    public void Register(IEyeParameters param)
    {
        _participants.Add(param);
    }

    public void UnRegister(IEyeParameters param)
    {
        _participants.Remove(param);
    }

    public bool GetClosest(IEyeParameters checkTransform, out IEyeParameters result)
    {
        float _minDistance = Mathf.Infinity;

        IEyeParameters _closestParticipant = null;

        foreach (var item in _participants)
        {
            if (item == checkTransform) continue;

            var distance = (item.EyeTransform.IPosition - checkTransform.EyeTransform.IPosition).magnitude;

            if (distance < _minDistance)
            {
                _closestParticipant = item;
                _minDistance = distance;
            }
        }

        result = _closestParticipant;

        return _closestParticipant != null;
    }
}