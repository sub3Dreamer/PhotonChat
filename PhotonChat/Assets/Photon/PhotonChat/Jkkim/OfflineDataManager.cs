using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로컬 데이터 저장(Player Prefab)
/// </summary>
public class OfflineDataManager
{
    const string USER_FORMAT = "{0}_";
    const int MAX_FRIEND_COUNT = 10;

    #region PlayerPrefs Key
    readonly string[] FriendKeys = new string[MAX_FRIEND_COUNT]
    {
        "Friend_0",
        "Friend_1",
        "Friend_2",
        "Friend_3",
        "Friend_4",
        "Friend_5",
        "Friend_6",
        "Friend_7",
        "Friend_8",
        "Friend_9",
    };
    #endregion

    static OfflineDataManager _instance;
    public static OfflineDataManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new OfflineDataManager();
            return _instance;
        }
    }

    List<string> _friendList = new List<string>();
    List<string> _friendDataKeyList = new List<string>();

    #region Initialize (ChatManager에서 호출)
    public void Init()
    {
        InitFriendDataKey();
        LoadFriendsData();
    }

    #endregion

    #region Friend Data
    void InitFriendDataKey()
    {
        _friendDataKeyList.Clear();
        for(int i = 0; i < FriendKeys.Length; i++)
        {
            string userThumb = string.Format(USER_FORMAT, ChatManager.Instance.UserID);
            string friendDataKey = userThumb + FriendKeys[i];
            _friendDataKeyList.Add(friendDataKey);
        }
    }

    void LoadFriendsData()
    {
        _friendList.Clear();

        for (int i = 0; i < _friendDataKeyList.Count; i++)
        {
            string friendValue = PlayerPrefs.GetString(_friendDataKeyList[i]);

            if (!string.IsNullOrEmpty(friendValue))
                _friendList.Add(friendValue);
        }
    }

    public void ClearFriendsData()
    {
        for (int i = 0; i < _friendDataKeyList.Count; i++)
        {
            PlayerPrefs.DeleteKey(_friendDataKeyList[i]);
        }
        PlayerPrefs.Save();

        _friendList.Clear();
    }

    public List<string> GetFriendsData()
    {
        return _friendList;
    }

    public bool IsMaxFriends()
    {
        return _friendList.Count >= MAX_FRIEND_COUNT;
    }

    public bool IsHaveFriends(string friendName)
    {
        return _friendList.Contains(friendName);
    }

    public void AddFriendsData(string friendID)
    {
        if(_friendList.Count >= MAX_FRIEND_COUNT)
        {
            CommonDebug.LogWarning("친구 목록이 가득 찼습니다.");
            return;
        }
        
        for(int i = 0; i < _friendDataKeyList.Count; i++)
        {
            string friendKey = _friendDataKeyList[i];
            string friendValue = PlayerPrefs.GetString(friendKey);

            if (string.IsNullOrEmpty(friendValue))
            {
                PlayerPrefs.SetString(friendKey, friendID);
                break;
            }
        }

        _friendList.Add(friendID);

        PlayerPrefs.Save();

        ChatManager.Instance.AddFriend(friendID);
    }

    public void RemoveFriendsData(string friendID)
    {
        if (_friendList.Count == 0)
        {
            CommonDebug.LogWarning("저장된 친구가 없습니다.");
            return;
        }

        for(int i = 0; i < _friendDataKeyList.Count; i++)
        {
            string friendKey = _friendDataKeyList[i];
            string friendValue = PlayerPrefs.GetString(friendKey);

            if (friendValue.Equals(friendID))
            {
                PlayerPrefs.SetString(friendKey, string.Empty);
                break;
            }
        }

        _friendList.Remove(friendID);

        PlayerPrefs.Save();

        ChatManager.Instance.RemoveFriend(friendID);
    }

    public void DebugLogSaveData()
    {
        for (int i = 0; i < _friendDataKeyList.Count; i++)
        {
            string friendKey = _friendDataKeyList[i];
            string friendValue = PlayerPrefs.GetString(friendKey);
            CommonDebug.Log($"PlayerPrefs에 저장된 친구 데이터 >> Key : {friendKey}, Value : {friendValue}");
        }

        for(int i = 0; i < _friendList.Count; i++)
        {
            CommonDebug.Log($"저장된 친구 이름 : {_friendList[i]}");
        }
    }
    #endregion
}
