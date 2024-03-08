using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class TriggerCheckController : MonoBehaviour
{
    //Trigger List
    private readonly Dictionary<Trigger, UnityAction<Collider>> _onTriggerEnterSubjectList = new();
    private readonly Dictionary<Trigger, UnityAction<Collider>> _onTriggerExitSubjectList = new();

    private readonly Dictionary<Layer, UnityAction<Collider>> _onLayerEnterSubjectList = new();
    private readonly Dictionary<Layer, UnityAction<Collider>> _onLayerExitSubjectList = new();

    [SerializeField] private Collider _colliderTrigger;

    private void OnTriggerEnter(Collider other)
    {
        CallSubject(_onTriggerEnterSubjectList, other);

        CallLayerSubject(_onLayerEnterSubjectList, other);
    }
    private void OnTriggerExit(Collider other)
    {
        CallSubject(_onTriggerExitSubjectList, other);

        CallLayerSubject(_onLayerExitSubjectList, other);
    }

    public void EnableCollider()
    {
        _colliderTrigger.enabled = true;
    }

    public void DisableCollider()
    {
        _colliderTrigger.enabled = false;
    }

    /*private void CallSubject<T>(Dictionary<T, UnityAction<Collider>> subjectList, Collider other, string targetName)
    where T : Enum
    {
        if (Enum.TryParse<T>(targetName, out T trigger))
        {
            if (subjectList.TryGetValue(trigger, out var subjects))
            {
                subjects.Invoke(other);
            }
        }
    }*/

    private void CallSubject(Dictionary<Trigger, UnityAction<Collider>> subjectList, Collider other)
    {
        if (Enum.TryParse<Trigger>(other.tag, out var trigger))
        {
            if (subjectList.TryGetValue(trigger, out var subjects))
            {
                subjects.Invoke(other);
            }
        }
    }

    private void CallLayerSubject(Dictionary<Layer, UnityAction<Collider>> subjectList, Collider other)
    {
        if (Enum.TryParse<Layer>(LayerMask.LayerToName(other.gameObject.layer), out var trigger))
        {
            if (subjectList.TryGetValue(trigger, out var subjects))
            {
                subjects.Invoke(other);
            }
        }
    }

    #region Layer Register

    public void TriggerLayerEnterRegister(
        Layer layer,
        UnityAction<Collider> subject
    )
    {
        Register(_onLayerEnterSubjectList, layer, subject);
    }

    public void TriggerLayerExitRegister(
        Layer layer,
        UnityAction<Collider> subject
    )
    {
        Register(_onLayerExitSubjectList, layer, subject);
    }

    #endregion

    #region Trigger Register

    //check with trigger
    public void TriggerEnterRegister(
        Trigger trigger,
        UnityAction<Collider> subject
    )
    {
        Register(_onTriggerEnterSubjectList, trigger, subject);
    }

    public void TriggerExitRegister(
        Trigger trigger,
        UnityAction<Collider> subject
    )
    {
        Register(_onTriggerExitSubjectList, trigger, subject);
    }

    //

    #endregion
    
    #region Register Base

    private void Register<T, C>(
        T subjectList,
        C layer,
        UnityAction<Collider> subject
    ) where T : Dictionary<C, UnityAction<Collider>>
    {
        if (subjectList.TryGetValue(layer, out var value))
        {
            value += subject;
            subjectList.TryAdd(layer, value);
        }
        else
        {
            subjectList.TryAdd(layer, subject);
        }
    }

    #endregion
    
    //test mode
    #region UnRegister Base
    private void UnRegister<T, C>(
        T subjectList,
        C layer,
        UnityAction<Collider> subject
    ) where T : Dictionary<C, UnityAction<Collider>>
    {
        if (subjectList.TryGetValue(layer, out var value))
        {
            value -= subject;
            subjectList.TryAdd(layer, value);
        }
    }

    #endregion

}

public enum Layer
{
    Eye,
    BrokenEye,
    Ground
}

public enum Trigger
{
    UpdateElement,
}