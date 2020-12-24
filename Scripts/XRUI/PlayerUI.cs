using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    public GameManagerRoom GameManager;

    public GameObject PlayerUIPanel;

    public GameObject LeftHand;

    private Button btn;

    public float distance = 100f; // rayの飛ばせる距離

    //Declare a LineRenderer to store the component attached to the GameObject. 
    [SerializeField] LineRenderer rend;

    //Settings for the LineRenderer are stored as a Vector3 array of points. Set up a V3 array to //initialize in Start. 
    Vector3[] points;

    public bool UIMode = false;

    public Text ID;
    public Text Name;
    public Text Version;

    private DisplayImages displayImages;

    private void Awake()
    {
        displayImages = GameObject.Find("SharingWhiteBoard").GetComponent<DisplayImages>();

        PlayerUIPanel.SetActive(false);
        if (photonView.IsMine)
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManagerRoom>();
        }
        else
        {
            PlayerUIPanel.SetActive(false);
        }
    }

    void Start()
    {
        InitializeLineRenderer();

        if (photonView.IsMine)
        {
            

            InitializePlayerInfo();
        }
    }

    private void InitializePlayerInfo()
    {
        if(photonView.IsMine)
        {
            this.ID.text = PhotonNetwork.LocalPlayer.UserId + "\n" + MasterClientString(PhotonNetwork.LocalPlayer);
            this.Name.text = PhotonNetwork.LocalPlayer.NickName;
            this.Version.text = PhotonNetwork.AppVersion;
        }
    }

    private string MasterClientString(Player aPlayer)
    {
        string IsMaster = "";
        if (aPlayer.IsMasterClient)
        {
            IsMaster = "(MasterClient)";
        }
        return IsMaster;
    }

    private void InitializeLineRenderer()
    {
        //get the LineRenderer attached to the gameobject. 
        rend = gameObject.GetComponent<LineRenderer>();

        //initialize the LineRenderer
        points = new Vector3[2];

        //set the start point of the linerenderer to the position of the gameObject. 
        //points[0] = Vector3.zero;
        points[0] = LeftHand.transform.position;

        //set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = LeftHand.transform.forward * 10;

        //finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);

        rend.enabled = false;
    }

    public LayerMask layerMask;

    public bool AlignLineRenderer(LineRenderer rend)
    {
        Ray ray = new Ray(LeftHand.transform.position, LeftHand.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * distance);
        bool hitBtn = false;

        points[0] = LeftHand.transform.position;

        // もしRayにオブジェクトが衝突したら　
        if (Physics.Raycast(ray, out hit, distance))
        {
            rend.enabled = true;
            //Debug.Log(hit.collider.gameObject.tag);//手もマスク
            if (hit.collider.gameObject.tag == "Button")
            {
                points[1] = hit.point;
                rend.startColor = Color.red;
                rend.endColor = Color.red;
                btn = hit.collider.gameObject.GetComponent<Button>();
                hitBtn = true;
            }
            else if (hit.collider.gameObject.tag == "BaseUI")
            {
                points[1] = hit.point;
                rend.startColor = Color.green;
                rend.endColor = Color.green;
                hitBtn = false;
            }
            else
            {
                points[1] = hit.point;
                rend.startColor = Color.blue;
                rend.endColor = Color.blue;
                hitBtn = false;
            }
        }
        else
        {
            rend.enabled = false;
            points[1] = LeftHand.transform.forward * 10;
            hitBtn = false;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitBtn;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // switch UI mode 
            if (Input.GetButtonDown("Oculus_CrossPlatform_Button2"))
            {
                if (UIMode)
                {
                    PlayerUIPanel.SetActive(false);
                    UIMode = false;
                    //rend.enabled = false;
                }
                else
                {
                    PlayerUIPanel.SetActive(true);
                    UIMode = true;
                    InitializePlayerInfo();
                    UpdateMemberList();
                }
            }

            if (AlignLineRenderer(rend) && Input.GetButtonDown("Oculus_CrossPlatform_Button3"))
            {
                btn.onClick.Invoke();
            }
        }
        else
        {
            /**
             * Laser Pointer 全員
             * Controll IsMine
             */
            AlignLineRenderer(rend);
        }
    }

    public void ButtonOnClick()
    {
        if (btn != null)
        {
            Debug.Log(btn.name);
            if (btn.name == "Leave_btn")
            {
                GameManager.LeaveRoom();
            }
            else if (btn.name == "Exit_btn")
            {
                GameManager.QuitBunOnClick();
            }
            else if (btn.name == "Give_btn")
            {
                RequestToGiveMeMasterToMasterClient();
            }
            else if (btn.name == "Next_btn")
            {
                displayImages.DisplayImageOnClick();
            }
            else if (btn.name == "Before_btn")
            {
                displayImages.BackImageOnClick();
            }
        }
    }

    public void RequestToGiveMeMasterToMasterClient()
    {
        //Player photonPlayer = Player;
        //PhotonNetwork.SetMasterClient(photonPlayer);
        photonView.RPC("GiveYouMasterClient", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void GiveYouMasterClient(string you)
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            if (you == player.NickName)
            {
                PhotonNetwork.SetMasterClient(player);
            }
        }
    }

    [SerializeField]
    Text joinedMembersText;

    public void OnPhotonPlayerConnected(Player player)
    {
        Debug.Log(player.NickName + " is joined.");
        UpdateMemberList();
    }

    public void UpdateMemberList()
    {
        joinedMembersText.text = "";
        foreach (var p in PhotonNetwork.PlayerList)
        {
            joinedMembersText.text += p.NickName + MasterClientString(p) + "\n";
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        InitializePlayerInfo();
        UpdateMemberList();
    }
}
