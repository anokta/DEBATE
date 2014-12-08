using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    public static float currentAlphaOver;
    float currentAlphaMenu, targetAlphaMenu;
    float targetAlphaOver, currentAlphaRule;

    float elapsed;

    public GUIText welcome, title, over, space, rule;

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
        currentAlphaRule = 0.0f;

        elapsed = 0.0f;
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

            float targetAlphaRule = (GameEventManager.CurrentState == GameEventManager.GameState.Running && NetworkManager.PlayerCount == 1 && Time.time - elapsed > 7.5f) ? 1.0f : 0.0f;
            if(currentAlphaRule != targetAlphaRule)
            {
                currentAlphaRule = Mathf.Lerp(currentAlphaRule, targetAlphaRule, 4.0f * Time.deltaTime);
            }

            Color c;

            c = rule.color;
            c.a = currentAlphaRule;
            rule.color = c;

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

        elapsed = Time.time;
    }

    void GameOver()
    {
        targetAlphaMenu = 0.0f;
        targetAlphaOver = 1.0f;
    }
}
