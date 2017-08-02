using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour {

    public void ToggleAudio()
    {
        AudioManager.Instance.ToggleAudio();
    }
}
