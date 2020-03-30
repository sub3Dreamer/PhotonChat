using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : MonoBehaviour, IPopup
{
    [SerializeField] GameObject _uiGroup;
    [SerializeField] Text _txtMessage;

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
    public void Open(string msg)
    {
        OpenPopup();
        _txtMessage.text = msg;
    }
    #endregion
    
    #region Button Event
    public void OnClickConfirm()
    {
        ClosePopup();
    }
    #endregion
}
