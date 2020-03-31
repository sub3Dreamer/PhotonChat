using ExitGames.Client.Photon;
using Photon.Chat;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [Header("Login UI")]
    [SerializeField] LoginUI _loginUI;

    [Header("Chat UI")]
    [SerializeField] ChatUI _chatUI;

    [Header("Popup")]
    [SerializeField] FriendPopup _friendPopup;
    [SerializeField] LoadingPopup _loadingPopup;
    [SerializeField] AlertPopup _alertPopup;

    [Header("Photon Chat Module")]
    [SerializeField] internal ChatSettings _chatAppSettings;

    [Header("Data")]
    [SerializeField] string[] _channels;

    string _userID;
    ChatClient _chatClient;
    string _currentChannel;

    static ChatManager _instance;

    #region Property
    public static ChatManager Instance
    {
        get
        {
            return _instance;
        }
    }

    // 기본 채널
    public string DefaultChannel
    {
        get
        {
            return _channels[0];
        }
    }

    public string UserID
    {
        get
        {
            return _userID;
        }
    }
    #endregion

    #region Interface
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            CommonDebug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            CommonDebug.LogWarning(message);
        }
        else
        {
            CommonDebug.Log(message);
        }
    }

    public void OnDisconnected()
    {
        CommonDebug.Log("OnDisconnected");
        Logout();
    }

    public void OnConnected()
    {
        CommonDebug.Log("OnConnected");
        
        if(_channels == null || _channels.Length == 0)
        {
            Debug.LogError("접속할 채널이 없습니다.");
            return;
        }

        _chatClient.Subscribe(_channels);        
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);

        InitComplete();

        _loadingPopup.ClosePopup();
    }

    public void OnChatStateChange(ChatState state)
    {
        CommonDebug.Log("OnChatStateChange >> state : " + state.ToString());
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        CommonDebug.Log($"OnGetMessages >> 채널 : {channelName}, 유저 아이디 : {senders[0]}, 메세지 : {messages[0]}");
        if(channelName.Equals(_currentChannel))
        {
            UpdateChatMessage(_currentChannel);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        CommonDebug.Log("OnPrivateMessage");
        if (channelName.Equals(_currentChannel))
        {
            UpdateChatMessage(_currentChannel);
        }
    }

    
    public void OnSubscribed(string[] channels, bool[] results)
    {
        CommonDebug.Log($"OnSubscribed >> 채널명 : {channels[0]}");

        // Array로 받지만 한 채널씩 들어옴.
        foreach (string channel in channels)
        {
            // Default 채널을 시작 채널로 설정함
            if (channel.Equals(DefaultChannel))
            {
                SetChannel(channel);
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        CommonDebug.Log("OnUnsubscribed");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        CommonDebug.Log($"OnStatusUpdate >> UserID : {user}");

        if(status == ChatUserStatus.Online)
        {
            _chatUI.AddLoginFriend(user);
        }
        else if(status == ChatUserStatus.Offline)
        {
            _chatUI.RemoveLoginFriend(user);
        }
        else
        {
            Debug.LogWarning("온라인, 오프라인 이외의 상태입니다.");
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        CommonDebug.Log($"OnUserSubscribed >> 채널 : {channel}, 유저 아이디 : {user}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        CommonDebug.Log($"OnUserUnsubscribed >> 채널 : {channel}, 유저 아이디 : {user}");
    }
    #endregion

    #region Unity Method
    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _loginUI.OpenUI();
        _chatUI.CloseUI();

        _friendPopup.ClosePopup();
        _alertPopup.ClosePopup();
        _loadingPopup.ClosePopup();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void Update()
    {
        if(_chatClient != null)
        {
            _chatClient.Service();
        }
    }
    #endregion

    #region Public Method
    public void Login(string userID)
    {
        _loadingPopup.OpenPopup();
        _userID = userID;
        _loginUI.CloseUI();
        _chatUI.OpenUI();

        InitChannelSelector();

        OfflineDataManager.Instance.Init();

        Connect();
    }

    public void Logout()
    {
        RemoveAllFriends();
        Disconnect();
        _loginUI.OpenUI();
        _chatUI.CloseUI();
    }

    public void SetChannel(string channelName)
    {
        SelectChannel(channelName);
        UpdateChatMessage(channelName);
    }

    // 포톤 서버에 송신하는 메세지
    public void SendChatMessage(string msg)
    {
        _chatClient.PublishMessage(_currentChannel, msg);
    }

    public void AddFriend(string friendID)
    {
        string[] addFriend = new string[1]
        {
            friendID
        };

        _chatClient.AddFriends(addFriend);
    }

    public void RemoveFriend(string friendID)
    {
        string[] removeFriend = new string[1]
        {
            friendID
        };

        _chatClient.RemoveFriends(removeFriend);

        _chatUI.RemoveLoginFriend(friendID);
    }

    public void RemoveAllFriends()
    {
        var removeFriends = OfflineDataManager.Instance.GetFriendsData().ToArray();

        _chatClient.RemoveFriends(removeFriends);

        for (int i = 0; i < removeFriends.Length; i++)
        {
            _chatUI.RemoveLoginFriend(removeFriends[i]);
        }
    }

    public void OpenFriendPopup()
    {
        _friendPopup.OpenPopup();
    }

    public void OpenAlertPopup(string msg)
    {
        _alertPopup.Open(msg);
    }
    #endregion

    #region Private Method

    void Connect()
    {
        CommonDebug.Log($"ID : {_userID}, 로그인 하였습니다.");

        _chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        _chatClient.UseBackgroundWorkerForSending = true;
#endif

        _chatClient.Connect(_chatAppSettings.AppId, "1.0", new Photon.Chat.AuthenticationValues(_userID));
    }

    void Disconnect()
    {
        CommonDebug.Log($"ID : {_userID}, 로그아웃 하였습니다.");

        _userID = string.Empty;
        if (_chatClient != null)
        {
            _chatClient.Disconnect();
        }
        _chatClient = null;
    }

    void InitChannelSelector()
    {
        _chatUI.SetCurrentChannel(DefaultChannel);
        // 드롭다운 초기화.
        for (int i = 0; i < _channels.Length; i++)
        {
            _chatUI.AddChannelSelector(_channels[i]);
        }
    }

    // 서버 연결이 완료된 후
    void InitComplete()
    {
        RegistFriends();
    }

    void RegistFriends()
    {
        var friendList = OfflineDataManager.Instance.GetFriendsData();
        if (friendList.Count > 0)
            _chatClient.AddFriends(friendList.ToArray());
    }

    void SelectChannel(string channelName)
    {
        _currentChannel = channelName;
    }

    void UpdateChatMessage(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
            return;

        ChatChannel channel = null;
        bool isValid = _chatClient.TryGetChannel(channelName, out channel);
        if (!isValid)
        {
            CommonDebug.LogError("채널이 존재하지 않습니다. ChannelName : " + channelName);
            return;
        }
        
        _chatUI.TxtChatMessage.text = channel.ToStringMessages();
    }
    #endregion
}
