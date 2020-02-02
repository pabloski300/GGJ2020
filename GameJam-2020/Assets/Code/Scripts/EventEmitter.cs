using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEmitter : MonoBehaviour
{
    public Sound sound;
    public void Start(){
        sound.Init();
    }
    // Start is called before the first frame update
    public void Play(Transform t)
    {
        sound.Play(t);
    }

    // Update is called once per frame
    public void Stop()
    {
        sound.Stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
