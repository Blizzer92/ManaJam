using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum enumGameStates
{
    Menu,
    Input,    
    PlayerMoving,    
	EnemiesMove,
    EnemiesMoving,
    GameEnd
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public GameObject MenuScreen;
	public GameObject TextScreen;
	public GameObject LifeUI;
	public Camera UICamara;
	public Button startGameBT;
	public Text intro;
	public Bar healthBar;
	
	public MapManager mapManager;
	[HideInInspector] public bool playerCanMove = true;	
	private List<Enemy> enemies;
	public int level = -1;

	private Camera mainCamera;
	
	public TextAsset[] levels;
	public enumGameStates gameState = enumGameStates.Menu;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();

		mapManager = GetComponent<MapManager>();
		
	}

	public void StartGameClick()
	{
		TextScreen.SetActive(true);
		Intro();
		MenuScreen.SetActive(false);

	}

	public void AfterIntro()
	{
		TextScreen.SetActive(false);
		UICamara.gameObject.SetActive(false);
		gameState = enumGameStates.Input;
		StartGame();
	}
	
	public void StartGame()
	{
		level++;
		AudioManager.instance.StopMusic();
		LifeUI.SetActive(true);
		enemies.Clear();
		mapManager.Setup(level);
		AudioManager.instance.PlayMusic("Game");
		AudioManager.instance.SetMusicVolume(0.5f);
	}

	public void Intro()
	{
		Sequence introSequence = DOTween.Sequence();
		introSequence.Append(intro.DOText("Es war einmal ...", 1f));
		introSequence.AppendInterval(1f);
		introSequence.Append(intro.DOText("die kranke kleine Schwester von Kadse wünschte sich noch einmal, ein Märchen zu hören.", 1f));
		introSequence.AppendInterval(1f);
		introSequence.Append(intro.DOText("Alle Märchen wurden von den Katzen geklaut.", 1f));
		introSequence.AppendInterval(1f);
		introSequence.Append(intro.DOText("Um noch einmal ein Märchen hören zu können, muss Kadse losziehen ins Katzen-*Ort*, wo *das Märchen* in 5 Teile aufgeteilt und versteckt wurden.", 1f));
		introSequence.AppendInterval(1f).OnComplete(() => { AfterIntro(); });
	}
	
	private void Start()
	{
		startGameBT.onClick.AddListener(StartGameClick);
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        if (go != null)
            mainCamera = go.GetComponent<Camera>();
        else
            mainCamera = null;
        AudioManager.instance.PlayMusic("StartMenu");		
	}

	void Update()
	{        
        UpdateStateMachine();
	}

	void UpdateStateMachine()
	{
		switch(gameState)
		{
			case enumGameStates.EnemiesMove:				
				gameState = enumGameStates.EnemiesMoving;    
				Debug.Log("MoveEnemies");
                StartCoroutine(MoveEnemies());                
				break;
			case enumGameStates.GameEnd:
				//TODO: GAMEEND anzeigen Sounf abspielen usw
			break;
		}
	}
	
	IEnumerator MoveEnemies()
	{						
		foreach (Enemy t in enemies)
		{
			t.Move();
			while(t.isMoving)
			{
				yield return null;
			}			
		}
					
		gameState = enumGameStates.Input;
	}

	public void AddEnemieToList(Enemy enemy)
	{
		enemies.Add(enemy);
	}

	public void RemoveEnemyFromList(Enemy enemy)
	{
		enemies.Remove(enemy);
	}

}
