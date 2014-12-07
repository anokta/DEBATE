using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LonerController : MonoBehaviour
{
    public float speed, acceleration;

    const int MIN_SHOUT = 3;
    const int MAX_SHOUT = 7;

    AudioSource sample;
    int shoutLength, shoutCurrent;

    void Awake()
    {
        sample = GetComponent<AudioSource>();
        shoutLength = shoutCurrent = 0;
    }

    void Update()
    {
        if (shoutCurrent < shoutLength && !sample.isPlaying)
        {
            sample.pitch = MusicalProperties.GetRandomPitch();
            sample.Play();

            shoutCurrent++;
        }
    }

    public void Move(int x, int y)
    {
        if (Mathf.Abs(rigidbody.velocity.x) < speed)
            rigidbody.AddForce(acceleration * Vector3.right * x);
    }

    public void Interact()
    {
        shoutCurrent = 0;
        shoutLength = Random.Range(MIN_SHOUT, MAX_SHOUT);
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 position = transform.position;

        if (stream.isWriting)
        {
            stream.Serialize(ref position);

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);
        }
        else
        {
            stream.Serialize(ref position);
            transform.position = position;

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);
        }
    }

    static class MusicalProperties
    {
        enum MusicalScale
        {
            MAJOR = 0, HARMONIC_MINOR = 1, NATURAL_MINOR = 2
        }

        static float[] majorScale = { 0, 2, 4, 5, 7, 9, 11 };
        static float[] harmonicMinorScale = { 0, 2, 3, 5, 7, 8, 11 };
        static float[] naturalMinorScale = { 0, 2, 3, 5, 7, 8, 10 };
        static Dictionary<MusicalScale, float[]> Scales = new Dictionary<MusicalScale, float[]>()
        {
            { MusicalScale.MAJOR, majorScale },
            { MusicalScale.HARMONIC_MINOR, harmonicMinorScale },
            { MusicalScale.NATURAL_MINOR, naturalMinorScale }
        };

        public static float[] CurrentScale = Scales[MusicalScale.MAJOR];

        public static float GetRandomPitch()
        {
            int index = Random.Range(0, CurrentScale.Length);

            return Mathf.Pow(1.05f, CurrentScale[index]);
        }
    }
}