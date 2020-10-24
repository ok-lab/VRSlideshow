using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPlayerController : MonoBehaviourPun
{
    public float Speed = 3f;
    public float RotateSpeed = 2f;

    private float h = 1f;
    private float v = 2f;

    private Animator anim;
    public float animSpeed = 2f;

    [SerializeField] private Camera m_Camera;



    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            // Animatorコンポーネントを取得する
            anim = GetComponent<Animator>();
        }
        else
        {
            m_Camera.enabled = false; // not my camera
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
        }
    }

    void Move()
    {
        //速度初期化
        float angleDir = transform.eulerAngles.y * (Mathf.PI / 180.0f);
        Vector3 dir1 = new Vector3(Mathf.Sin(angleDir), 0, Mathf.Cos(angleDir));
        Vector3 dir2 = new Vector3(-Mathf.Cos(angleDir), 0, Mathf.Sin(angleDir));

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += dir1 * Speed * Time.deltaTime;
            this.h = 1f;
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += -dir1 * Speed * Time.deltaTime;
            this.h = -1f;
        }

        else
        {
            this.h = 0;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -RotateSpeed, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, RotateSpeed, 0);
        }

        anim.SetFloat("Speed", this.h);
    }
}
