using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManagerRoom : MonoBehaviourPunCallbacks
{
    public static GameManagerRoom Instance;

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    public List<GameObject> AvatarPrefabs;

    public string CharactarName;

    public void Start()
    {
        Instance = this;

        // assign CharacterName to playerPrefab if you receive CharacterName from LineRendererSettings
        PlayerPrefabSettings(CharactarName);


        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 1.4f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
        
        Debug.Log(CharactarName);
    }

    private void PlayerPrefabSettings(string Name)
    {
        if (Name == "unity_chan")
        {
            this.playerPrefab = AvatarPrefabs[0];
        }
        else if (Name == "robot")
        {
            this.playerPrefab = AvatarPrefabs[1];
        }
        else if (Name == "Paladin")
        {
            this.playerPrefab = AvatarPrefabs[2];
        }
        else if (Name == "Rin")
        {
            this.playerPrefab = AvatarPrefabs[3];

        }
        else if (Name == "Misaki")
        {
            this.playerPrefab = AvatarPrefabs[4];
        }
        else if (Name == "FA_unitychan_btn")
        {
            this.playerPrefab = AvatarPrefabs[5];
        }
        else
        {
            this.playerPrefab = AvatarPrefabs[0];
        }
        
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    public void QuitBunOnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
#endif
    }
}
