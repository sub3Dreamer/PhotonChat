using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] GameObject _uiGroup;
    [SerializeField] Text _txtLoginFriends;
    [SerializeField] InputField _inputFieldMessage;
    [SerializeField] Text _txtChatMessage;
    [SerializeField] Dropdown _channelSelector;

    #region Property
    public Text TxtChatMessage
    {
        get
        {
            return _txtChatMessage;
        }
    }
    #endregion

   

    public void OpenUI()
    {
        _uiGroup.SetActive(true);
        Clear();
    }

    public void CloseUI()
    {
        _uiGroup.SetActive(false);
    }

    public void AddChannelSelector(string channelName)
    {
        Dropdown.OptionData channel = new Dropdown.OptionData(channelName);
        _channelSelector.options.Add(channel);
    }

    public void SetCurrentChannelCaption(string channelName)
    {
        if(!_channelSelector.captionText.Equals(channelName))
            _channelSelector.captionText.text = channelName;
    }

    public void ResetFriendText(string text)
    {
        _txtLoginFriends.text = text;
    }

    void Clear()
    {
        _channelSelector.options.Clear();
        _txtChatMessage.text = string.Empty;
    }

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
