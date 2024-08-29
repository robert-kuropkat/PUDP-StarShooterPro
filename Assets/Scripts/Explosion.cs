using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start() 
    {
        AudioSource myAudio =  GetComponent<AudioSource>();
        if (myAudio == null) { Debug.LogError("Explosion Audio Source is NULL"); }
                        else { myAudio.Play(); }
        Destroy(this, 2.4f); 
    }

}
