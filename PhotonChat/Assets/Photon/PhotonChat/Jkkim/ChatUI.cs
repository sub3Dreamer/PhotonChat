using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    const string CHAT_JOIN_FRIEND = "접속한 친구 : ";
    const string COMMA = ",";

    [SerializeField] GameObject _uiGroup;
    [SerializeField] Text _txtLoginFriends;
    [SerializeField] InputField _inputFieldMessage;
    [SerializeField] Text _txtChatMessage;
    [SerializeField] Dropdown _channelSelector;

    List<string> _loginFriendList = new List<string>();

    #region Property
    public Text TxtChatMessage
    {
        get
        {
            return _txtChatMessage;
        }
    }
    #endregion

    void Clear()
    {
        _channelSelector.options.Clear();
        _txtChatMessage.text = string.Empty;
        _loginFriendList.Clear();
    }

    void ResetFriendText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(CHAT_JOIN_FRIEND);

        for (int i = 0; i < _loginFriendList.Count; i++)
        {
            sb.Append(_loginFriendList[i]);
            if (i < _loginFriendList.Count - 1)
                sb.Append(COMMA);
        }

        _txtLoginFriends.text = sb.ToString();
    }

    #region Public Method
    public void OpenUI()
    {
        _uiGroup.SetActive(true);
        Clear();
    }

    public void CloseUI()
    {
        _uiGroup.SetActive(false);
        Clear();
    }

    public void AddChannelSelector(string channelName)
    {
        Dropdown.OptionData channel = new Dropdown.OptionData(channelName);
        _channelSelector.options.Add(channel);
    }

    public void SetCurrentChannel(string channelName)
    {
        if(!_channelSelector.captionText.Equals(channelName))
            _channelSelector.captionText.text = channelName;
    }

    public void AddLoginFriend(string friendID)
    {
        if (_loginFriendList.Contains(friendID))
            return;

        _loginFriendList.Add(friendID);
        ResetFriendText();
    }

    public void RemoveLoginFriend(string friendID)
    {
        if (!_loginFriendList.Contains(friendID))
            return;

        _loginFriendList.Remove(friendID);
        ResetFriendText();
    }
    #endregion

    #region Button Event

    public void OnClickBtnFriend()
    {
        ChatManager.Instance.OpenFriendPopup();
    }

    public void OnClickBtnLogout()
    {
        ChatManager.Instance.Logout();
    }

    public void OnClickBtnSend()
    {
        var inputMessage = _inputFieldMessage.text;
        if (!string.IsNullOrEmpty(inputMessage))
        {
            ChatManager.Instance.SendChatMessage(inputMessage);
            _inputFieldMessage.text = string.Empty;
        }
    }

    #endregion

    #region InputField Event
    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            var inputMessage = _inputFieldMessage.text;
            if (!string.IsNullOrEmpty(inputMessage))
            {
                ChatManager.Instance.SendChatMessage(inputMessage);
                _inputFieldMessage.text = string.Empty;
            }
        }
    }
    #endregion

    #region DropDown Event
    public void OnSelectChannel(int dropdownIndex)
    {
        string selectChannelName = _channelSelector.options[dropdownIndex].text;
        ChatManager.Instance.SetChannel(selectChannelName);
    }
    #endregion
}
