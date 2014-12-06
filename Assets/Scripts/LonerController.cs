using UnityEngine;
using System.Collections;

public class LonerController : MonoBehaviour {

    const int MIN_SHOUT = 3;
    const int MAX_SHOUT = 7;

    AudioSource sample;
    int shoutLength, shoutCurrent;

	void Awake () 
    {
        sample = GetComponent<AudioSource>();
        shoutLength = shoutCurrent = 0;
    }
	
	void Update () {
	    if(Input.GetButtonDown("Jump"))
        {
            Interact();
        }

        if(shoutCurrent < shoutLength && !sample.isPlaying)
        {
            sample.pitch = MusicalProperties.GetRandomPitch();
            sample.Play();

            shoutCurrent++;
        }
	}

    void Interact()
    {
        shoutCurrent = 0;
        shoutLength = Random.Range(MIN_SHOUT, MAX_SHOUT);
    }
}