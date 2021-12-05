using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum enumGameStates
{
	None,
    Menu,
    Input,    
    PlayerMoving,    
	EnemiesMove,
    EnemiesMoving,
	WaitRound,
    GameEnd
}

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public GameObject MenuScreen;
	public GameObject TextScreen;
	public GameObject LifeUI;
	public GameObject OutroScreen;
	public Camera UICamara;
	public Button startGameBT;
	public Text intro;
	public Text outro;
	public Bar healthBar;
	public float textSpeed = 1f;
	
	public MapManager mapManager;
	[HideInInspector] public bool playerCanMove = true;	
	private List<Enemy> enemies;
	public int level = -1;

	private Camera mainCamera;
	
	public float waitRoundTimeMax = 0.5f;
	private float waitRoundTime;
	
	public TextAsset[] levels;
	[SerializeField] private enumGameStates gameState = enumGameStates.Menu;
	[SerializeField] private enumGameStates prevGameState = enumGameStates.None;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		//DontDestroyOnLoad(gameObject);

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
	
	public void AfterOutro()
	{
		SceneManager.LoadScene(0);

	}
	
	public void StartGame()
	{
		level++;
		AudioManager.instance.StopMusic();
		LifeUI.SetActive(true);
		enemies.Clear();
		mapManager.Setup(level);
		ChangeState(enumGameStates.Input);
		AudioManager.instance.PlayMusic("Game");
		AudioManager.instance.SetMusicVolume(0.5f);
	}

	public void Intro()
	{
		Debug.Log("Intro Start");
		Sequence introSequence = DOTween.Sequence();
		introSequence.Append(intro.DOText("Es war einmal eine Welt, in der die Katzen alle Märchen stahlen und zerstörten.", textSpeed));
		introSequence.AppendInterval(textSpeed);
		introSequence.Append(intro.DOText("Ein letztes war verschont geblieben.", textSpeed));
		introSequence.AppendInterval(textSpeed);
		introSequence.Append(intro.DOText("Jedoch hielten sie dieses versteckt, in 5 Teilen an 5 Orten.", textSpeed));
		introSequence.AppendInterval(textSpeed);
		introSequence.Append(intro.DOText("Zieh' los und finde alle 5 Teile! Aber gib Acht. Katzen sind gefährlich.", textSpeed));
		introSequence.AppendInterval(textSpeed).OnComplete(() => { AfterIntro(); });
		Debug.Log("Intro Ende");
	}
	
	public void Outro()
	{
		Sequence introSequence = DOTween.Sequence();
		introSequence.Append(outro.DOText("Geschwind kehrte die kleine Ratte nach Haus' zurück und erzählt der todkranken Schwester das letzte Märchen.", textSpeed));
		introSequence.AppendInterval(textSpeed);
		introSequence.Append(outro.DOText("Es war ein schöner Moment.", textSpeed));
		introSequence.AppendInterval(textSpeed);
		introSequence.Append(outro.DOText("Und wenn die kleine Ratte nicht gestorben ist, dann schwelgt sie noch heute in dieser wärmenden Erinnerung.", textSpeed));
		introSequence.AppendInterval(textSpeed).OnComplete(() => { AfterOutro(); });
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
				
		waitRoundTime = waitRoundTimeMax;			
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
				ChangeState(enumGameStates.EnemiesMoving);				
                StartCoroutine(MoveEnemies());                
				break;
			case enumGameStates.GameEnd:
				break;
			case enumGameStates.WaitRound:				
				EventManager.TriggerEvent("PlayerWaitIcon", new Dictionary<string, object> {{"enable", true}});
				waitRoundTime -= Time.deltaTime;
				if (waitRoundTime <= 0f)
				{
					ChangeState(enumGameStates.Input);
					waitRoundTime = waitRoundTimeMax;
                    EventManager.TriggerEvent("PlayerWaitIcon", new Dictionary<string, object> { { "enable", false } });
				}
				break;
		}
	}

	public void ChangeState(enumGameStates newState)  
	{
		prevGameState = gameState;
		gameState = newState;	
		//Debug.Log("Change state: " + prevGameState + " -> " + gameState);
	}

	public enumGameStates GetState()
	{
		return gameState;
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
					
		ChangeState(enumGameStates.WaitRound);
	}

	public void AddEnemieToList(Enemy enemy)
	{
		enemies.Add(enemy);
	}

	public void RemoveEnemyFromList(Enemy enemy)
	{
		enemies.Remove(enemy);
	}

	public void Finsish()
	{
		Destroy(GameObject.Find("Map"));
		LifeUI.SetActive(false);
		UICamara.gameObject.SetActive(true);
		OutroScreen.SetActive(true);
		Outro();
	}
	

}
