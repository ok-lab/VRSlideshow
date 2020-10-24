// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using Photon.Pun;
using UnityEngine;

namespace Photon.Pun
{
    using UnityEngine;


    [AddComponentMenu("Photon Networking/Photon Transform View")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    [RequireComponent(typeof(PhotonView))]
    public class MyPhotonTransformView : MonoBehaviour, IPunObservable
    {
        /* arrange */
        public GameObject myGameObject;

        private float m_Distance; // 
        private float m_Angle;

        private PhotonView m_PhotonView;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition; // 
        private Vector3 m_StoredPosition; // 位置を保存する

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;

        bool m_firstTake = false;

        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = myGameObject.transform.position; // アタッチされたオブジェクトの座標情報
            m_NetworkPosition = Vector3.zero; // Shorthand for writing Vector3(0, 0, 0).

            m_NetworkRotation = Quaternion.identity; // 無回転を意味します。初期状態から回転をしない状態なので、回転をリセットする際に使えます。
        }

        void OnEnable()// この関数はオブジェクトが有効/アクティブになったときに呼び出されます
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (!this.m_PhotonView.IsMine) // 自分が作成したオブジェクトじゃないとき
            {
                myGameObject.transform.position = Vector3.MoveTowards(myGameObject.transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                myGameObject.transform.rotation = Quaternion.RotateTowards(myGameObject.transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                // 座標と回転を動かしている
                //　Vector3.MoveTowards
                // 概要:
                //     Calculate a position between the points specified by current and target, moving
                //     no farther than the distance specified by maxDistanceDelta.
                //
                //     maxDistanceDeltaで指定された距離よりも遠くに移動しないように、現在点と目標点で指定された点の間の位置を計算します。
                //
                // パラメーター:
                //   current:
                //     The position to move from.
                //
                //   target:
                //     The position to move towards.
                //
                //   maxDistanceDelta:
                //     Distance to move current per call.
                //
                // 戻り値:
                //     The new position.

                //　Quaternion.RotateTowards
                // 概要:
                //     Rotates a rotation from towards to. に向かって回転します。
                //
                // パラメーター:
                //   from:
                //
                //   to:
                //
                //   maxDegreesDelta:
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // >If true, this client should add data to the stream to send it.
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_Direction = myGameObject.transform.position - this.m_StoredPosition; // 今の座標と貯めていた座標の差分から方向を求める
                    this.m_StoredPosition = myGameObject.transform.position; // 移動後の現在の座標を保存

                    stream.SendNext(myGameObject.transform.position); // １現在の座標をstreamに送る  
                    stream.SendNext(this.m_Direction); // ２方向を送る
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(myGameObject.transform.rotation); // ３現在の回転をstreamに送る
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(myGameObject.transform.localScale); // ４大きさの情報
                }
            }
            else
            {
                /**
                 * /// <summary>Read next piece of data from the stream when IsReading is true.</summary>
                    public object ReceiveNext()
                    {
                        if (this.IsWriting)
                        {
                            Debug.LogError("Error: you cannot read this stream that you are writing!");
                            return null;
                        }

                        object obj = this.readData[this.currentItem];
                        this.currentItem++;
                        return obj;
                    }
                 */


                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext(); // 相手から受け取ったデータを記憶　１．相手の座標なのでネットワーク座標
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        myGameObject.transform.position = this.m_NetworkPosition; // 最初、自分の座標は、ネットワーク座標になる 距離も０になる
                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag; // ラグと移動方向をもとにネットワーク座標を変える
                        this.m_Distance = Vector3.Distance(myGameObject.transform.position, this.m_NetworkPosition); // 現在の座標とネットワーク座標の距離を保存する
                    }
                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        this.m_Angle = 0f;
                        myGameObject.transform.rotation = this.m_NetworkRotation;
                    }
                    else
                    {
                        this.m_Angle = Quaternion.Angle(myGameObject.transform.rotation, this.m_NetworkRotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    myGameObject.transform.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
    }
}