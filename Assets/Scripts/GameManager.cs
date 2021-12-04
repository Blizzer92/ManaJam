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
    EnemiesMoving
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	public MapManager mapManager;
	[HideInInspector] public bool playerCanMove = true;
	private List<Enemy> enemies;


	public enumGameStates gameState = enumGameStates.Input;

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
		mapManager.Setup();
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
                StartCoroutine(MoveEnemies());                
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
		StartGame();
	}

}
