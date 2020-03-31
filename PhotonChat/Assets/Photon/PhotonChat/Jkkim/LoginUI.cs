using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    const string APP_EXIT_MSG = "앱을 종료하시겠습니까?";

    [SerializeField] GameObject _uiGroup;
    [SerializeField] InputField _inputFieldID;
    
    public void OpenUI()
    {
        _uiGroup.SetActive(true);
    }

    public void CloseUI()
    {
        _uiGroup.SetActive(false);
    }

    #region Button Event
    public void OnClickBtnLogin()
    {
        var inputUserID = _inputFieldID.text.Trim();
        if (!string.IsNullOrEmpty(inputUserID))
        {
            ChatManager.Instance.Login(inputUserID);
            _inputFieldID.text = string.Empty;
        }
        else
            CommonDebug.Log("로그인 오류! 아이디가 입력되지 않았습니다!");
    }

    public void OnClickBtnExit()
    {
        ChatManager.Instance.OpenAlertPopup(APP_EXIT_MSG, AlertPopup.PopupType.Selectable, 
            () => 
            {
                Application.Quit();
            });
    }
    #endregion
}
