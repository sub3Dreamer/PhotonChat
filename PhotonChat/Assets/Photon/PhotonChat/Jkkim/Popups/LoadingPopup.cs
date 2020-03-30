using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPopup : MonoBehaviour, IPopup
{
    [SerializeField] GameObject _uiGroup;

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
}
