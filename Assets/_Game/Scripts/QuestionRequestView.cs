using System;
using TMPro;
using UnityEngine.UI;

public class QuestionRequestView : ElementalViewMonoBehaviour<QuestionRequestViewElement>
{
    private Button _confirmButton;
    private Button _cancelButton;

    private TextMeshProUGUI _headerText;
    private TextMeshProUGUI _cancelButtonText;
    private TextMeshProUGUI _confirmButtonText;
    
    public override void Awake()
    {
        base.Awake();
        SerializedFieldsInit();
        Init();
    }

    private void SerializedFieldsInit()
    {
        foreach (var item in Elements)
        {
            switch (item.ElementType)
            {
                case QuestionRequestElement.CancelButton:
                    _cancelButton = item.GetComponent<Button>();
                    break;
                case QuestionRequestElement.ConfirmButton:
                    _confirmButton = item.GetComponent<Button>();
                    break;
                case QuestionRequestElement.CancelButtonText:
                    _cancelButtonText = item.GetComponent<TextMeshProUGUI>();
                    break;
                case QuestionRequestElement.ConfirmButtonText:
                    _confirmButtonText = item.GetComponent<TextMeshProUGUI>();
                    break;
                case QuestionRequestElement.HeaderText:
                    _headerText = item.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }
    }

    private void Init()
    {
    }

    public void Activate
    (
        string headerText,
        string cancelText,
        string confirmText = "",
        Action callBack = null
    )
    {
        _cancelButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.RemoveAllListeners();

        _confirmButton.gameObject.SetActive(confirmText != "");

        _headerText.text = headerText;
        _cancelButtonText.text = cancelText;
        _confirmButtonText.text = confirmText;

        _cancelButton.onClick.AddListener(CloseConfirmPage);
        _confirmButton.onClick.AddListener(() =>
        {
            callBack?.Invoke();
            CloseConfirmPage();
        });
    }

    private void CloseConfirmPage()
    {
        UiManager.Activate(UISubPageType.Empty);
    }
}

public enum QuestionRequestElement
{
    ConfirmButton,
    CancelButton,
    CancelButtonText,
    ConfirmButtonText,
    HeaderText
}