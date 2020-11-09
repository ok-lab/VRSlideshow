using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MyVRPlayerController : MonoBehaviourPun
{

    public GameObject headCameraPosition;
    public GameObject right;
    public GameObject left;

    public float Speed = 3f;
    public float RotateSpeed = 2f;

    public float h = 1f;
    public float v = 2f;

    private Animator anim;
    public float animSpeed = 2f;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private AudioListener m_AudioListener;

    // 回転した事実を保持
    private bool flagRotate = false;

    [SerializeField] private string recenterInputName = "Fire2";


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            // 部屋を歩き回ることを想定したモード
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            InputTracking.Recenter();

            // Animatorコンポーネントを取得する
            anim = GetComponent<Animator>();
        }
        else
        {
            m_Camera.enabled = false; // not my camera
            m_AudioListener.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Recenter.
            if (OVRInput.GetDown(OVRInput.RawButton.LThumbstick) || Input.GetButtonDown("Oculus_CrossPlatform_SecondaryThumbstick") || Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick"))
            {
                InputTracking.Recenter();
            }
            //ポジションをセットする
            SettingPosition();
            //プレイヤーの操作を受け付ける
            PlayerControl();
        }
    }

    private void SettingPosition()
    {
        //this.head.transform.localPosition = InputTracking.GetLocalPosition(XRNode.CenterEye);

        // モーションコントローラーの位置・向きを取得する
        // 基本的な考え方は、positionは絶対座標軸上の座標値 localPositionは親オブジェクトとの相対座標値。
        this.left.transform.localPosition = InputTracking.GetLocalPosition(XRNode.LeftHand);
        this.left.transform.localRotation = InputTracking.GetLocalRotation(XRNode.LeftHand);
        this.right.transform.localPosition = InputTracking.GetLocalPosition(XRNode.RightHand);
        this.right.transform.localRotation = InputTracking.GetLocalRotation(XRNode.RightHand);

        //this.head.transform.localPosition += new Vector3(0, 0, -1);
        this.left.transform.localRotation *= Quaternion.Euler(0, 0, 90);
        this.right.transform.localRotation *= Quaternion.Euler(0, 0, -90);
    }

    private void PlayerControl()
    {
        // Rキーで位置トラッキングをリセットする（Stationaryでしか動作しない）
        if (Input.GetKeyDown(KeyCode.R))
        {
            InputTracking.Recenter();
        }

        //速度初期化
        float angleDir = headCameraPosition.transform.eulerAngles.y * (Mathf.PI / 180.0f);
        Vector3 dir1 = new Vector3(Mathf.Sin(angleDir), 0, Mathf.Cos(angleDir));
        Vector3 dir2 = new Vector3(-Mathf.Cos(angleDir), 0, Mathf.Sin(angleDir));

        //primary 左手 secondary 右手

        // 左手？ジョイスティックの入力 座標の移動
        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") != 0)
        {
            //Debug.Log("Horizontal:" + Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal"));
            transform.position += -dir2 * Speed * Time.deltaTime * Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal");
        }

        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") != 0)
        {
            //Debug.Log("Vertical:" + Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical"));
            transform.position += dir1 * Speed * Time.deltaTime * Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical");
        }

        // Rotate 右手のジョイスティックの入力　回転
        if (flagRotate) // 入力がなくなれば再び回転できる状態にする
        {
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") == 0f)
            {
                flagRotate = false;
            }
        }
        else
        {
            // 右手ジョイスティックの入力 カメラの移動
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > 0.5f)
            {
                //Debug.Log("Horizontal:" + Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal"));
                transform.Rotate(0.0f, 30.0f, 0.0f);
                flagRotate = true;
            }
            // 右手ジョイスティックの入力 カメラの移動
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < -0.5f)
            {
                //Debug.Log("Horizontal:" + Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal"));
                transform.Rotate(0.0f, -30.0f, 0.0f);
                flagRotate = true;
            }
        }
        this.h = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal") * 10;
        this.v = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") * 10;
        anim.SetFloat("Speed", this.v);
        anim.SetFloat("Direction", this.h);
        anim.speed = animSpeed;	
    }
}
