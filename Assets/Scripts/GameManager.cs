using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public Camera UICamara;

	public MapManager mapManager;
	[HideInInspector] public bool playerCanMove = true;
	private List<Enemy> enemies;
	public int level = -1;
	
	
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

	void StartGame()
	{
		
		enemies.Clear();
		mapManager.Setup(level);
	}

	void Update()
	{		
		UpdateStateMachine();
	}

	void UpdateStateMachine()
	{
		switch(gameState)
		{
			case enumGameStates.Menu:
				if (Input.GetMouseButton(0))
				{
					MenuScreen.SetActive(false);
					UICamara.enabled = false;
					gameState = enumGameStates.Input;
					StartGame();
				}
				break;
			case enumGameStates.EnemiesMove:				
				gameState = enumGameStates.EnemiesMoving;                  
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

	private void OnEnable()
	{
		SceneManager.sceneLoaded += Finish;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= Finish;
	}

	void Finish(Scene scene, LoadSceneMode mode)
	{
		level++;
		if (level > 0)
		{
			StartGame();
		}
	}

}
