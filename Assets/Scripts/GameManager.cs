using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	public MapManager mapManager;
	[HideInInspector] public bool playerCanMove = true;
	private List<Enemie> enemies;
	private bool enemiesMoving;


	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemie>();

		mapManager = GetComponent<MapManager>();
		
		StartGame();
	}

	void StartGame()
	{
		mapManager.Setup();
	}

	void Update()
	{
		if (playerCanMove || enemiesMoving) {
			return;
		}

		StartCoroutine(MoveEnemies());
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;

		yield return new WaitForSeconds(.1f);

		if (enemies.Count == 0) {
			yield return new WaitForSeconds(.1f);
		}

		foreach (Enemie t in enemies)
		{
			t.Move();

			yield return new WaitForSeconds(0.1f);
		}
			
		enemiesMoving = false;

		playerCanMove = true;
	}

	public void AddEnemieToList(Enemie enemie)
	{
		enemies.Add(enemie);
	}
	
}
