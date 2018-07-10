using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance { get; private set; }
    [SerializeField] Transform hpBar;
    [SerializeField] GameObject lifeSprite;
    int lives = 3;
    List<GameObject> livesUI = new List<GameObject>();

    [SerializeField] Transform meatBar;
    [SerializeField] GameObject meatSprite;
    List<GameObject> meatsUI = new List<GameObject>();
    [SerializeField] int meats;
    public int Meats { get { return meats; } }

    private void Awake()
    {
        Instance = this;
        for(int i = 0; i < lives; i++)
        {
            var lifeOBJ = Instantiate(lifeSprite);
            livesUI.Add(lifeOBJ);
            lifeOBJ.transform.SetParent(hpBar);
        }

        for (int i = 0; i < meats; i++)
        {
            var meatOBJ = Instantiate(meatSprite);
            meatsUI.Add(meatOBJ);
            meatOBJ.transform.SetParent(meatBar);
        }
    }

    private void Start()
    {
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void LoseHP()
    {
        if (lives > 0)
        {
            Destroy(livesUI[lives - 1]);
            livesUI.RemoveAt(--lives);
        }

        if(lives <= 0)
        {
            GameOver();
        }
    }

    public void LoseMeat()
    {
        if (meats > 0)
        {
            Destroy(meatsUI[meats - 1]);
            meatsUI.RemoveAt(--meats);
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("gameOver");
    }
}
