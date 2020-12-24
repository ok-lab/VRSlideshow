using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

public class SpeakerSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var enumerator = Recorder.PhotonMicrophoneEnumerator;
        if (enumerator.IsSupported)
        {
            for (int i = 0; i < enumerator.Count; i++)
            {
                Debug.LogFormat("PhotonMicrophone Index={0} ID={1} Name={2}", i, enumerator.IDAtIndex(i),
                    enumerator.NameAtIndex(i));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
