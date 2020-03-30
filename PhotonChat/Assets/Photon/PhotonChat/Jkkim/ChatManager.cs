using ExitGames.Client.Photon;
using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    const string CHAT_CHANNEL_JOIN_MESSAGE_FORMAT = "{0}에 입장하였습니다.";
    const string CHAT_JOIN_FRIEND = "접속한 친구 : ";
    const string COMMA = ",";

    [Header("Login UI")]
    [SerializeField] LoginUI _loginUI;

    [Header("Chat UI")]
    [SerializeField] ChatUI _chatUI;

    [Header("Popup")]
    [SerializeField] FriendPopup _friendPopup;

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

    public string UserName
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
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("OnDisconnected");
        Logout();
    }

    public void OnConnected()
    {
        Debug.Log("OnConnected");

        if(_channels == null || _channels.Length == 0)
        {
            Debug.LogError("접속할 채널이 없습니다.");
            return;
        }

        _chatClient.Subscribe(_channels);        
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);

        InitComplete();
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange >> state : " + state.ToString());
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log($"OnGetMessages >> 채널 : {channelName}, 유저 아이디 : {senders[0]}, 메세지 : {messages[0]}");
        if(channelName.Equals(_currentChannel))
        {
            UpdateChatMessage(_currentChannel);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage");
        if (channelName.Equals(_currentChannel))
        {
            UpdateChatMessage(_currentChannel);
        }
    }

    
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed");

        // Array로 받지만 한 채널씩 들어옴.
        foreach (string channel in channels)
        {
            // 드롭다운 초기화.
            _chatUI.AddChannelSelector(channel);

            // Default 채널을 시작 채널로 설정함
            if (channel.Equals(DefaultChannel))
            {
                SetChannel(channel);
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("OnUnsubscribed");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("OnStatusUpdate");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"OnUserSubscribed >> 채널 : {channel}, 유저 아이디 : {user}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"OnUserUnsubscribed >> 채널 : {channel}, 유저 아이디 : {user}");
    }
    #endregion

    #region Unity Method
    void Awake()
    {
        _instance = this;
        OfflineDataManager.Instance.Init();
    }

    void Start()
    {
        _loginUI.OpenUI();
        _chatUI.CloseUI();
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
        _userID = userID;

        Connect();
        _loginUI.CloseUI();
        _chatUI.OpenUI();
    }

    public void Logout()
    {
        Disconnect();
        _loginUI.OpenUI();
        _chatUI.CloseUI();
    }

    public void SetChannel(string channelName)
    {
        _chatUI.SetCurrentChannelCaption(channelName);

        SelectChannel(channelName);
        UpdateChatMessage(channelName);
    }

    // 포톤 서버에 송신하는 메세지
    public void SendChatMessage(string msg)
    {
        _chatClient.PublishMessage(_currentChannel, msg);
    }

    public void AddFriend(string friendName)
    {
        string[] addFriend = new string[1]
        {
            friendName
        };

        _chatClient.AddFriends(addFriend);
    }

    public void RemoveFriend(string friendName)
    {
        string[] removeFriend = new string[1]
        {
            friendName
        };

        _chatClient.RemoveFriends(removeFriend);
    }

    public void ResetFriendText()
    {
        var friendList = OfflineDataManager.Instance.GetFriendsData();

        StringBuilder sb = new StringBuilder();

        sb.Append(CHAT_JOIN_FRIEND);

        if (friendList != null)
        {
            for (int i = 0; i < friendList.Count; i++)
            {
                sb.Append(friendList[i]);

                if (i < friendList.Count - 1)
                    sb.Append(COMMA);
            }
        }

        _chatUI.ResetFriendText(sb.ToString());
    }

    public void OpenFriendPopup()
    {
        _friendPopup.OpenPopup();
    }
    #endregion

    #region Private Method

    void Connect()
    {
        //this.UserIdFormPanel.gameObject.SetActive(false);

        _chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        _chatClient.UseBackgroundWorkerForSending = true;
#endif

        _chatClient.Connect(_chatAppSettings.AppId, "1.0", new Photon.Chat.AuthenticationValues(_userID));
        
        Debug.Log($"ID : {_userID}, 로그인 하였습니다.");
    }

    void Disconnect()
    {
        Debug.Log($"ID : {_userID}, 로그아웃 하였습니다.");

        _userID = string.Empty;
        if (_chatClient != null)
        {
            _chatClient.Disconnect();
        }
    }

    // 서버 연결이 완료된 후
    void InitComplete()
    {
        RegistFriends();
        ResetFriendText();
    }

    void RegistFriends()
    {
        var friendList = OfflineDataManager.Instance.GetFriendsData();
        if (friendList.Count > 0)
            _chatClient.AddFriends(friendList.ToArray());
    }

    // 귓속말 송신
    void SendPrivateMessage(string userID, string msg)
    {
        _chatClient.SendPrivateMessage(userID, msg);
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
            Debug.LogError("채널이 존재하지 않습니다. ChannelName : " + channelName);
            return;
        }
        
        _chatUI.TxtChatMessage.text = channel.ToStringMessages();
    }
    #endregion
}
