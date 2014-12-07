using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    float currentAlpha, targetAlpha;

    public GUIText welcome, title;

    void Awake()
    {
        GameEventManager.GameMenu += GameMenu;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
    }

    void OnDestroy()
    {
        GameEventManager.GameMenu -= GameMenu;
        GameEventManager.GameStart -= GameStart;
        GameEventManager.GameOver -= GameOver;
    }

	// Use this for initialization
	void Start () {
        currentAlpha = targetAlpha = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	    if(currentAlpha != targetAlpha)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, 2 * Time.deltaTime);
        }

        Color c = welcome.color;
        c.a = currentAlpha;

        welcome.color = c;
        title.color = c;
	}

    void GameMenu()
    {
        targetAlpha = 1.0f;
    }

    void GameStart()
    {

    }

    void GameOver()
    {

    }
}
