using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : MonoBehaviour, IPopup
{
    public enum PopupType
    {
        Confirm = 0,
        Selectable = 1
    }

    [SerializeField] GameObject _uiGroup;
    [SerializeField] Text _txtMessage;
    [SerializeField] Button _btnConfirm;
    [SerializeField] Button _btnCancel;

    System.Action _confirmCallback;
    System.Action _cancelCallback;

    #region Interface
    public void OpenPopup()
    {
        _uiGroup.SetActive(true);
    }

    public void ClosePopup()
    {
        _uiGroup.SetActive(false);
    }
    #endregion

    #region Public Method
    public void OpenConfirmPopup(string msg, System.Action confirmCallback = null)
    {
        OpenPopup();
        _txtMessage.text = msg;

        _btnConfirm.gameObject.SetActive(true);
        _btnCancel.gameObject.SetActive(false);

        if (confirmCallback != null)
            _confirmCallback = confirmCallback;
    }

    public void OpenSelectPopup(string msg, System.Action confirmCallback = null, System.Action cancelCallback = null)
    {
        OpenPopup();
        _txtMessage.text = msg;

        _btnConfirm.gameObject.SetActive(true);
        _btnCancel.gameObject.SetActive(true);

        if (confirmCallback != null)
            _confirmCallback = confirmCallback;

        if (cancelCallback != null)
            _cancelCallback = cancelCallback;
    }
    #endregion
    
    #region Button Event
    public void OnClickConfirm()
    {
        ClosePopup();

        if (_confirmCallback != null)
        {
            _confirmCallback.Invoke();
            _confirmCallback = null;
        }
    }

    public void OnClickCancel()
    {
        ClosePopup();

        if (_cancelCallback != null)
        {
            _cancelCallback.Invoke();
            _cancelCallback = null;
        }
    }
    #endregion
}
