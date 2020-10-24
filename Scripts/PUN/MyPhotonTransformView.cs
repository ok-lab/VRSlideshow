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
        private Vector3 m_StoredPosition; // �ʒu��ۑ�����

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;

        bool m_firstTake = false;

        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = myGameObject.transform.position; // �A�^�b�`���ꂽ�I�u�W�F�N�g�̍��W���
            m_NetworkPosition = Vector3.zero; // Shorthand for writing Vector3(0, 0, 0).

            m_NetworkRotation = Quaternion.identity; // ����]���Ӗ����܂��B������Ԃ����]�����Ȃ���ԂȂ̂ŁA��]�����Z�b�g����ۂɎg���܂��B
        }

        void OnEnable()// ���̊֐��̓I�u�W�F�N�g���L��/�A�N�e�B�u�ɂȂ����Ƃ��ɌĂяo����܂�
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (!this.m_PhotonView.IsMine) // �������쐬�����I�u�W�F�N�g����Ȃ��Ƃ�
            {
                myGameObject.transform.position = Vector3.MoveTowards(myGameObject.transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                myGameObject.transform.rotation = Quaternion.RotateTowards(myGameObject.transform.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                // ���W�Ɖ�]�𓮂����Ă���
                //�@Vector3.MoveTowards
                // �T�v:
                //     Calculate a position between the points specified by current and target, moving
                //     no farther than the distance specified by maxDistanceDelta.
                //
                //     maxDistanceDelta�Ŏw�肳�ꂽ�������������Ɉړ����Ȃ��悤�ɁA���ݓ_�ƖڕW�_�Ŏw�肳�ꂽ�_�̊Ԃ̈ʒu���v�Z���܂��B
                //
                // �p�����[�^�[:
                //   current:
                //     The position to move from.
                //
                //   target:
                //     The position to move towards.
                //
                //   maxDistanceDelta:
                //     Distance to move current per call.
                //
                // �߂�l:
                //     The new position.

                //�@Quaternion.RotateTowards
                // �T�v:
                //     Rotates a rotation from towards to. �Ɍ������ĉ�]���܂��B
                //
                // �p�����[�^�[:
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
                    this.m_Direction = myGameObject.transform.position - this.m_StoredPosition; // ���̍��W�ƒ��߂Ă������W�̍���������������߂�
                    this.m_StoredPosition = myGameObject.transform.position; // �ړ���̌��݂̍��W��ۑ�

                    stream.SendNext(myGameObject.transform.position); // �P���݂̍��W��stream�ɑ���  
                    stream.SendNext(this.m_Direction); // �Q�����𑗂�
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(myGameObject.transform.rotation); // �R���݂̉�]��stream�ɑ���
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(myGameObject.transform.localScale); // �S�傫���̏��
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
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext(); // ���肩��󂯎�����f�[�^���L���@�P�D����̍��W�Ȃ̂Ńl�b�g���[�N���W
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        myGameObject.transform.position = this.m_NetworkPosition; // �ŏ��A�����̍��W�́A�l�b�g���[�N���W�ɂȂ� �������O�ɂȂ�
                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag; // ���O�ƈړ����������ƂɃl�b�g���[�N���W��ς���
                        this.m_Distance = Vector3.Distance(myGameObject.transform.position, this.m_NetworkPosition); // ���݂̍��W�ƃl�b�g���[�N���W�̋�����ۑ�����
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