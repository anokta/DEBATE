using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LonerController : MonoBehaviour
{
    TextMesh label;
    int playerID;
    public int PlayerID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    public float speed, acceleration;
    public int voice;

    const int MIN_SHOUT = 3;
    const int MAX_SHOUT = 7;

    const float MAX_PUSH = 1000.0f;
    const float MIN_ANGER = 30.0f;
    const float MAX_ANGER = 60.0f;

    const float UNIT_ANGER = 0.05f;

    Vector3 targetPosition;

    AudioSource sample;
    AudioChorusFilter chorus;
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
        set { anger = Mathf.Max(0.0f, value); targetColor = new Color(anger * 1.0f + skinColor.r, -anger * 0.6f + skinColor.g, -anger * 0.6f + skinColor.b); }
    }

    GameManager gameManager;

    bool isDead;

    void Awake()
    {
        sample = GetComponent<AudioSource>();
        shoutLength = shoutCurrent = 0;

        anger = 0.0f;
        skinColor = currentColor = targetColor = Color.white;
        voice = 0;

        targetPosition = transform.position;

        chorus = GetComponent<AudioChorusFilter>();

        label = GetComponentInChildren<TextMesh>();

        gameManager = FindObjectOfType<GameManager>();

        isDead = false;
    }

    void Update()
    {
        if (shoutCurrent < shoutLength && !sample.isPlaying)
        {
            sample.pitch = MusicalProperties.GetRandomPitch(voice);
            sample.Play();

            shoutCurrent++;

            rigidbody.AddForce(new Vector3(Random.Range(-0.15f, 0.15f), 1.0f, 0.0f) * (41 + Mathf.Min(0.5f, anger) * 10));
        }
        else
        {
            if (Network.isServer)
            {
                Anger -= 2.0f * UNIT_ANGER * Time.deltaTime;

                if (transform.position.y < 0.0f)
                {
                    voice = Mathf.FloorToInt(7.0f * transform.position.y);

                    if (!isDead && transform.position.y < -10.0f)
                    {
                        isDead = true;
                        gameManager.networkView.RPC("AddLog", RPCMode.Others, "Candidate #" + playerID + "rather died than to argue");
                        networkView.RPC("Die", RPCMode.Others, playerID);
                    }
                }
            }
        } 
        
        if (Network.isClient)
        {
            int myID = int.Parse(Network.player.ToString());

            label.text = myID == playerID ? "Myself" : "Candidate #" + playerID;

            if (currentColor != targetColor)
            {
                if (myID == playerID)
                {
                    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, (anger > 0.0f || shoutCurrent < shoutLength) ? 12.0f - 10.0f * anger : 15.0f, 1.75f * Time.deltaTime);
                }

                currentColor = Color.Lerp(currentColor, targetColor, 2.5f * Time.deltaTime);
                renderer.material.color = currentColor;

                chorus.depth = anger;

                currentColor.a = 0.75f;
                label.color = currentColor;
            }

            if (myID == playerID) // && Camera.main.transform.position != transform.position - Vector3.forward * 5.0f)
            {
                Vector3 target = new Vector3(0.0f, 1.0f, -5.0f);

                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, anger > 0.0f ? target + transform.position : target, Time.deltaTime);
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
        // Restart shout
        shoutCurrent = 0;
        shoutLength = Random.Range(MIN_SHOUT, MAX_SHOUT);

        // Check encounters
        RaycastHit[] rights = Physics.RaycastAll(transform.position, Vector3.right, Mathf.Max(2.0f, 4.0f * anger));
        RaycastHit[] lefts = Physics.RaycastAll(transform.position, Vector3.left, Mathf.Max(2.0f, 4.0f * anger));

        if (rights.Length + lefts.Length == 0)
        {
            Anger += UNIT_ANGER;

            if(Anger > 1.0f)
            {
                gameManager.networkView.RPC("AddLog", RPCMode.Others, "Candidate #" + playerID + " could not hanlde the stress today");
                networkView.RPC("Die", RPCMode.Others, playerID);
            }
        }

        foreach (RaycastHit hit in rights)
        {
            if (hit.transform.tag.Equals("Player"))
            {
                Anger -= 1.25f * UNIT_ANGER;

                LonerController victim = hit.transform.gameObject.GetComponent<LonerController>();
                victim.Anger += 2.5f * UNIT_ANGER;

                float force = Mathf.Min(MAX_PUSH, Mathf.Max(0.0f, speed - victim.rigidbody.velocity.x) * 1.0f / hit.distance * Mathf.Max(MIN_ANGER, anger * MAX_ANGER)); 
                victim.rigidbody.AddForce(force * Vector3.right);

                if (victim.Anger > 1.0f)
                {
                    gameManager.networkView.RPC("AddLog", RPCMode.Others, "Candidate #" + playerID + " debated his way into Candidate #" + victim.PlayerID);
                    networkView.RPC("Die", RPCMode.Others, victim.PlayerID);
                }
            }
        }
        foreach (RaycastHit hit in lefts)
        {
            if (hit.transform.tag.Equals("Player"))
            {
                Anger -= 1.25f * UNIT_ANGER;
                LonerController victim = hit.transform.gameObject.GetComponent<LonerController>();
                victim.Anger += 2.5f * UNIT_ANGER;

                float force = Mathf.Min(MAX_PUSH, Mathf.Max(0.0f, victim.rigidbody.velocity.x + speed) * 1.0f / hit.distance * Mathf.Max(MIN_ANGER, anger * MAX_ANGER)); 
                victim.rigidbody.AddForce(force * Vector3.left);

                if(victim.Anger > 1.0f)
                {
                    gameManager.networkView.RPC("AddLog", RPCMode.Others, "Candidate #" + playerID + " deducted his way into Candidate #" + victim.PlayerID);
                    networkView.RPC("Die", RPCMode.Others, victim.PlayerID);
                }
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 position = transform.position;

        if (stream.isWriting)
        {
            stream.Serialize(ref playerID);

            stream.Serialize(ref position);

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);

            stream.Serialize(ref voice);

            float r = targetColor.r, g = targetColor.g, b = targetColor.b;
            stream.Serialize(ref r); stream.Serialize(ref g); stream.Serialize(ref b);

            stream.Serialize(ref anger);
        }
        else
        {
            stream.Serialize(ref playerID);

            stream.Serialize(ref position);
            targetPosition = position;

            stream.Serialize(ref shoutCurrent);
            stream.Serialize(ref shoutLength);

            stream.Serialize(ref voice);

            float r = targetColor.r, g = targetColor.g, b = targetColor.b;
            stream.Serialize(ref r); stream.Serialize(ref g); stream.Serialize(ref b);
            targetColor = new Color(r, g, b);

            stream.Serialize(ref anger);
        }
    }

    static class MusicalProperties
    {
        public static float[] CurrentScale = { 0, 2, 4, 5, 7, 9, 11 };

        const int OCTAVE = 12;

        public static float GetRandomPitch(int voice)
        {
            int index = voice + Random.Range(0, CurrentScale.Length);

            int octaveOffset = Mathf.FloorToInt((float)index / CurrentScale.Length);
            int scaleOffset = index - octaveOffset * CurrentScale.Length;

            return Mathf.Pow(1.05f, CurrentScale[scaleOffset] + octaveOffset * OCTAVE);
        }
    }

    [RPC]
    void Die(int id)
    {
        if (int.Parse(Network.player.ToString()) == id)
        {
            Network.Disconnect();

            GameEventManager.TriggerGameOver();
        }
    }
}