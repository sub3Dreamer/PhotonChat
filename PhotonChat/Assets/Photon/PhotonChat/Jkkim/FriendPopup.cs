using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FriendPopup : MonoBehaviour, IPopup
{
    const string FRIEND_LIST_LABEL = "저장된 친구 : ";

    [SerializeField] GameObject _uiGroup;
    [SerializeField] Text _txtFriendList;
    [SerializeField] InputField _inputFieldAdd;
    [SerializeField] InputField _inputFieldRemove;

    #region Interface
    public void OpenPopup()
    {
        _uiGroup.SetActive(true);
        ResetFriendList();
    }

    public void ClosePopup()
    {
        Clear();
        _uiGroup.SetActive(false);
        ChatManager.Instance.ResetFriendText();
    }
    #endregion
    
    void Awake()
    {
        ClosePopup();
    }

    void Clear()
    {
        _txtFriendList.text = FRIEND_LIST_LABEL;
    }

    void ResetFriendList()
    {
        var friendList = OfflineDataManager.Instance.GetFriendsData();

        StringBuilder sb = new StringBuilder();
        sb.Append(FRIEND_LIST_LABEL);
        
        for(int i = 0; i < friendList.Count; i++)
        {
            sb.Append(friendList[i]);

            if (i < friendList.Count - 1)
                sb.Append(",");
        }

        _txtFriendList.text = sb.ToString();
    }

    #region Button Event
    public void OnClickAdd()
    {
        var friendName = _inputFieldAdd.text;
        if (string.IsNullOrEmpty(friendName))
            return;

        // 자기 자신을 추가 했을때
        if (ChatManager.Instance.UserName.Equals(friendName))
            return;

        // 친구 목록이 가득 찼을때
        if (OfflineDataManager.Instance.IsMaxFriends())
            return;

        // 중복된 친구를 추가했을때
        if (OfflineDataManager.Instance.IsOverlapFriends(friendName))
            return;

        ChatManager.Instance.AddFriend(friendName);
        _inputFieldAdd.text = string.Empty;

        OfflineDataManager.Instance.AddFriendsData(friendName);
        ResetFriendList();
    }

    public void OnClickRemove()
    {
        var friendName = _inputFieldRemove.text;
        if (string.IsNullOrEmpty(friendName))
            return;

        ChatManager.Instance.RemoveFriend(friendName);
        _inputFieldRemove.text = string.Empty;

        OfflineDataManager.Instance.RemoveFriendsData(friendName);
        ResetFriendList();
    }

    public void OnClickRemoveAll()
    {
        OfflineDataManager.Instance.ClearFriendsData();
        ResetFriendList();
    }

    public void OnClickDebug()
    {
        OfflineDataManager.Instance.DebugLogSaveData();
    }

    public void OnClickClose()
    {
        ClosePopup();
    }
    #endregion
}
