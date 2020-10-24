using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public bool AutoConect = false;

    #region Private Fields
    string GameVersion = "1";
    bool isConnecting;

    private LineRendererSettings lrs;

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";
    #endregion

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        lrs = GameObject.Find("VRCanvas").GetComponent<LineRendererSettings>();
    }

    void Start()
    {
        if (AutoConect)
        {
            Debug.Log("Start() : ");
            new WaitForSeconds(2.0f);
            Connect();
        }
    }

    #region Public Method
    public void Connect()
    {
        // Player Infomation Settings
        SetPlayerInfo();
        SceneManager.sceneLoaded += OnLoadedScene;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region MonoBehaviorPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Debug.Log("Launcher: OnConnectedToMaster()");
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Launcher: OnDisconnected() with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Launcher: OnJoinRandomFailed()");
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRooms });

        RoomOptions options = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 10,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Launcher: OnJoinedRoom()");
        //PhotonNetwork.LoadLevel("Room");

        //SceneManager.sceneLoaded += OnLoadedScene;
        SceneManager.LoadScene("Room");
    }
    #endregion

    private void OnLoadedScene(Scene next, LoadSceneMode mode)
    {
        Debug.Log("OnLoadedScene : " + next.name);

        // シーン切り替え後のスクリプトを取得
        var GameManagerRoom = GameObject.Find("GameManager").GetComponent<GameManagerRoom>();

        // データを渡す処理
        GameManagerRoom.CharactarName = lrs.Select;
        
        // イベントから削除
        SceneManager.sceneLoaded -= OnLoadedScene;
    }

    #region Custom
    private void SetPlayerInfo()
    {
        Debug.Log("Launcher: SetPlayerInfo()");
        // 適当にニックネームを設定
        string PlayerName = "guest" + UnityEngine.Random.Range(1000, 9999);
        PhotonNetwork.NickName = PlayerName;
        // Unity自身にペアとなったエントリーのリストを保存しておく
        PlayerPrefs.SetString(playerNamePrefKey, PlayerName);
    }
    #endregion
}