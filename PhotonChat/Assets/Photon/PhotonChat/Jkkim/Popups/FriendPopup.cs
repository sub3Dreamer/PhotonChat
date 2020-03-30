using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FriendPopup : MonoBehaviour, IPopup
{
    const string FRIEND_LIST_LABEL = "저장된 친구 : ";
    const string ADD_MINE = "자기 자신을 추가할 수 없습니다.";
    const string FRIEND_CNT_FULL = "친구 목록이 가득 찼습니다.";
    const string FRIEND_OVERLAP = "이미 추가된 친구입니다.";
    const string FRIEND_NOT_EXIST = "존재하지 않는 친구입니다.";

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
    }
    #endregion

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
        var friendID = _inputFieldAdd.text;
        if (string.IsNullOrEmpty(friendID))
            return;

        // 자기 자신을 추가 했을때
        if (ChatManager.Instance.UserID.Equals(friendID))
        {
            ChatManager.Instance.OpenAlertPopup(ADD_MINE);
            return;
        }

        // 친구 목록이 가득 찼을때
        if (OfflineDataManager.Instance.IsMaxFriends())
        {
            ChatManager.Instance.OpenAlertPopup(FRIEND_CNT_FULL);
            return;
        }

        // 중복된 친구를 추가했을때
        if (OfflineDataManager.Instance.IsHaveFriends(friendID))
        {
            ChatManager.Instance.OpenAlertPopup(FRIEND_OVERLAP);
            return;
        }

        ChatManager.Instance.AddFriend(friendID);
        _inputFieldAdd.text = string.Empty;

        OfflineDataManager.Instance.AddFriendsData(friendID);
        ResetFriendList();
    }

    public void OnClickRemove()
    {
        var friendID = _inputFieldRemove.text;
        if (string.IsNullOrEmpty(friendID))
            return;

        // 친구 목록에 없는 친구를 입력했을때
        if (OfflineDataManager.Instance.IsHaveFriends(friendID) == false)
        {
            ChatManager.Instance.OpenAlertPopup(FRIEND_NOT_EXIST);
            return;
        }

        ChatManager.Instance.RemoveFriend(friendID);
        _inputFieldRemove.text = string.Empty;

        OfflineDataManager.Instance.RemoveFriendsData(friendID);

        ResetFriendList();
    }

    public void OnClickRemoveAll()
    {
        if (OfflineDataManager.Instance.GetFriendsData().Count == 0)
            return;

        ChatManager.Instance.RemoveAllFriends();
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
