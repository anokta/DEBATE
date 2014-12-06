using UnityEngine;
using System.Collections;

public class LonerController : MonoBehaviour {

    public float speed, acceleration;

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
        // Move
        int x = (int)Input.GetAxisRaw("Horizontal");
        int y = (int)Input.GetAxisRaw("Vertical");

        if(x != 0)
        {
            if(Mathf.Abs(rigidbody.velocity.x) < speed)
                rigidbody.AddForce(acceleration * Vector3.right * x);
        }


        // Interact
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