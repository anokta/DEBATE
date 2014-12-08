using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    public static float currentAlphaOver;
    float currentAlphaMenu, targetAlphaMenu;
    float targetAlphaOver;

    public GUIText welcome, title, over, space;

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
    void Start()
    {
        currentAlphaMenu = targetAlphaMenu = 0.0f;
        currentAlphaOver = targetAlphaOver = 0.0f;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!Network.isServer)
        {
            if (currentAlphaMenu != targetAlphaMenu)
            {
                currentAlphaMenu = Mathf.Lerp(currentAlphaMenu, targetAlphaMenu, 1.5f * Time.deltaTime);
            }
            if (currentAlphaOver != targetAlphaOver)
            {
                currentAlphaOver = Mathf.Lerp(currentAlphaOver, targetAlphaOver, 3.0f * Time.deltaTime);
            }

            Color c;
            
            c = welcome.color;
            c.a = currentAlphaMenu;
            welcome.color = c;

            c.a = currentAlphaOver;
            over.color = c;

            c = title.color;

            c.a = currentAlphaMenu;
            title.color = c;

            c.a = currentAlphaOver;
            space.color = c;
        }
    }

    void GameMenu()
    {
        targetAlphaMenu = 1.0f;
        targetAlphaOver = 0.0f;
    }

    void GameStart()
    {
        targetAlphaMenu = 0.0f;
        targetAlphaOver = 0.0f;
    }

    void GameOver()
    {
        targetAlphaMenu = 0.0f;
        targetAlphaOver = 1.0f;
    }
}
