using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로컬 데이터 저장(Player Prefab)
/// </summary>
public class OfflineDataManager
{
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

    #region Initialize (ChatManager에서 호출)
    public void Init()
    {
        LoadFriendsData();
    }

    #endregion

    #region Friend Data
    void LoadFriendsData()
    {
        _friendList.Clear();
        for (int i = 0; i < FriendKeys.Length; i++)
        {
            string friendValue = PlayerPrefs.GetString(FriendKeys[i]);

            if (!string.IsNullOrEmpty(friendValue))
                _friendList.Add(friendValue);
        }
    }

    public void ClearFriendsData()
    {
        for (int i = 0; i < FriendKeys.Length; i++)
        {
            PlayerPrefs.DeleteKey(FriendKeys[i]);
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

    public bool IsOverlapFriends(string friendName)
    {
        return _friendList.Contains(friendName);
    }

    public void AddFriendsData(string friendName)
    {
        if(_friendList.Count >= MAX_FRIEND_COUNT)
        {
            Debug.LogWarning("친구 목록이 가득 찼습니다.");
            return;
        }
        
        for(int i = 0; i < FriendKeys.Length; i++)
        {
            string friendValue = PlayerPrefs.GetString(FriendKeys[i]);

            if (string.IsNullOrEmpty(friendValue))
            {
                PlayerPrefs.SetString(FriendKeys[i], friendName);
                break;
            }
        }

        _friendList.Add(friendName);

        PlayerPrefs.Save();
    }

    public void RemoveFriendsData(string friendName)
    {
        if (_friendList.Count == 0)
        {
            Debug.LogWarning("저장된 친구가 없습니다.");
            return;
        }

        for(int i = 0; i < FriendKeys.Length; i++)
        {
            string friendValue = PlayerPrefs.GetString(FriendKeys[i]);

            if (friendValue.Equals(friendName))
            {
                PlayerPrefs.SetString(FriendKeys[i], string.Empty);
                break;
            }
        }

        _friendList.Remove(friendName);

        PlayerPrefs.Save();
    }

    public void DebugLogSaveData()
    {
        for (int i = 0; i < FriendKeys.Length; i++)
        {
            string friendValue = PlayerPrefs.GetString(FriendKeys[i]);
            Debug.Log($"PlayerPrefs에 저장된 친구 데이터 >> Key : {FriendKeys[i]}, Value : {friendValue}");
        }

        for(int i = 0; i < _friendList.Count; i++)
        {
            Debug.Log($"저장된 친구 이름 : {_friendList[i]}");
        }
    }
    #endregion
}
