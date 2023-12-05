using UnityEngine;
using System;

public class QuestionRequestManager : MonoManager
{
    [SerializeField] private QuestionRequestView _requestView;

    public void Activate
    (
        string headerText,
        string cancelText,
        string confirmText = "",
        Action callBack = null
    )
    {
        _requestView.Activate(headerText, cancelText, confirmText, callBack);
    }
}