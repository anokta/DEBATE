using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LonerController : MonoBehaviour
{
    public float speed, acceleration;
    public int voice;

    const int MIN_SHOUT = 3;
    const int MAX_SHOUT = 7;

    const float MAX_PUSH = 1000.0f;
    const float MIN_ANGER = 125.0f;
    const float MAX_ANGER = 500.0f;

    const float UNIT_ANGER = 0.05f;

    Vector3 targetPosition;

    AudioSource sample;
    int shoutLength, shoutCurrent;

    private Color skinColor, currentColor, targetColor;
    public Color SkinColor
    {
        set { skinColor = currentColor = targetColor = value; }
    }

    private float anger;
    public float Anger
    {
        get { return anger; }
        set { anger = Mathf.Min(1.0f, Mathf.Max(0.0f, value)); targetColor = new Color(anger * 1.0f + skinColor.r, -anger * 0.6f + skinColor.g, -anger * 0.6f + skinColor.b); }
    }

    void Awake()
    {
        sample = GetComponent<AudioSource>();
        shoutLength = shoutCurrent = 0;

        anger = 0.0f;
        skinColor = currentColor = targetColor = Color.white;
        voice = 0;

        targetPosition = transform.position;
    }

    void Update()
    {
        if (Network.isServer)
        {
            if (shoutCurrent < shoutLength && !sample.isPlaying)
            {
                sample.pitch = MusicalProperties.GetRandomPitch(voice);
                sample.Play();

                shoutCurrent++;

                rigidbody.AddForce(new Vector3(Random.Range(-0.15f, 0.15f), 1.0f, 0.0f) * (45 + Mathf.Min(0.5f, anger) * 10));
            }
            else
            {
                Anger -= 2.0f * UNIT_ANGER * Time.deltaTime;
            }
        }
        else
        {
            if (currentColor != targetColor)
            {
                currentColor = Color.Lerp(currentColor, targetColor, 2.5f * Time.deltaTime);
                renderer.material.color = currentColor;
            }

            if (transform.position != targetPosition)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 6.0f * Time.deltaTime);
                }
            }
        }
    }

    public void Move(int x)
    {
        if (Mathf.Abs(rigidbody.velocity.x) < speed)
            rigidbody.AddForce(acceleration * Vector3.right * x);
    }

    public void Interact()
    {
        shoutCurrent = 0;
        shoutLength = Random.Range(MIN_SHOUT, MAX_SHOUT);

        RaycastHit[] rights = Physics.RaycastAll(transform.position, Vector3.right, Mathf.Max(1.0f, 2.0f * anger));
        RaycastHit[] lefts = Physics.RaycastAll(transform.position, Vector3.left, Mathf.Max(1.0f, 2.0f * anger));

        if (rights.Length + lefts.Length == 0)
        {
            Anger += 1.5f * UNIT_ANGER;
        }
        foreach (RaycastHit hit in rights)
        {
            if (hit.transform.tag.Equals("Player"))
            {
                Anger -= 1.25f * UNIT_ANGER;
                hit.transform.gameObject.GetComponent<LonerController>().Anger += 2.5f * UNIT_ANGER;
                hit.rigidbody.AddForce(Mathf.Min(MAX_PUSH, 1.0f / hit.distance * Mathf.Max(MIN_ANGER, anger * MAX_ANGER)) * Vector3.right);
            }
        }
        foreach (RaycastHit hit in lefts)
        {
            if (hit.transform.tag.Equals("Player"))
            {
                Anger -= 1.25f * UNIT_ANGER;
                hit.transform.gameObject.GetComponent<LonerController>().Anger += 2.5f * UNIT_ANGER;
                hit.rigidbody.AddForce(Mathf.Min(MAX_PUSH, 1.0f / hit.distance * Mathf.Max(MIN_ANGER, anger * MAX_ANGER)) * Vector3.left);
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 position = transform.position;

        if (stream.isWriting)
        {
            stream.Serialize(ref position);

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);

            stream.Serialize(ref voice);

            float r = targetColor.r, g = targetColor.g, b = targetColor.b;
            stream.Serialize(ref r); stream.Serialize(ref g); stream.Serialize(ref b);
        }
        else
        {
            stream.Serialize(ref position);
            targetPosition = position;

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);

            stream.Serialize(ref voice);

            float r = targetColor.r, g = targetColor.g, b = targetColor.b;
            stream.Serialize(ref r); stream.Serialize(ref g); stream.Serialize(ref b);
            targetColor = new Color(r, g, b);
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

        const int OCTAVE = 12;

        public static float GetRandomPitch(int voice)
        {
            int index = voice + Random.Range(0, CurrentScale.Length);

            int octaveOffset = Mathf.FloorToInt((float)index / CurrentScale.Length);
            int scaleOffset = index - octaveOffset * CurrentScale.Length;

            return Mathf.Pow(1.05f, CurrentScale[scaleOffset] + octaveOffset * OCTAVE);

        }
    }
}