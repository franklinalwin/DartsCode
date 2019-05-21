using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class Player
{
    public int id;
    public string name;
    public int team;
    private int points = 0;
    public int goldMedal = 0;
    public int silverMedal = 0;
    public int bronzeMedal = 0;


}
public enum StateGame : byte { IDLE, PLAYERS, MEDAL_TABLE, DEMOLITION, KILLER, SHANGAI, QUACK_SHOT, ERROR };
public enum FactorHit: byte { SINGLE, DOUBLE, TRIPLE, EYE};

public class ControlGame : MonoBehaviour
{
    public static ControlGame instance = null;
    public List<Player> players = new List<Player>();
    public Dictionary<int, List<Player>> dictionaryTeams = new Dictionary<int , List<Player>>();

    public StateGame stateGame;
    public Player currentPlayer = null; 
    private int indexCurrentPlayer = 0;

    public bool newPlayer = false;
    public bool newThrow = false;

    private bool conectedToDartsDectection = false;

    public string messageError;
    public string hitsString;
    private string urlPlayers = "http://localhost:5000/darts/players?numPlayers=4&numTeams=2";

    private Uri uriDartsDetection = new Uri("ws://localhost:9000");
    private ClientWebSocket cws = null;
    private ArraySegment<byte> buf = new ArraySegment<byte>(new byte[1024]);
    private bool isGameInit = false;
    public string colorHit;
    public int pointsHit; 
    public FactorHit factorHit;
    public int positionHit; 

    void Awake()
    {

        Debug.Log("Control");

        stateGame = StateGame.IDLE;

        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);


    }
    
    public bool isPlayersLoaded()
    {
        return players.Count > 0;

    }
    async void connectDartDectetion()
    {
        cws = new ClientWebSocket();
        try
        {
            await cws.ConnectAsync(uriDartsDetection, CancellationToken.None);
            if (cws.State == WebSocketState.Open)
            {
                conectedToDartsDectection = true;
                Debug.Log("connected");
                GetInfoHits();
            }

        }
        catch (Exception e)
        {

            messageError = "Can't connect to the darts detection system";
            Debug.Log(messageError + "->" + e.Message);
            SceneManager.LoadScene("Error");

        }
    }

    async void GetInfoHits()
    {
        WebSocketReceiveResult r = await cws.ReceiveAsync(buf, CancellationToken.None);
        hitsString = Encoding.UTF8.GetString(buf.Array, 0, r.Count);
        Debug.Log("Hit: " + hitsString);
        if(!hitsString.Contains("REMOVE_DARTS") && !hitsString.Contains("READY"))
        {
            if(!hitsString.Contains("NONE"))
            { 
                mapStringHit();
            }
            newThrow = true;
        }
        else if(hitsString.Contains("READY"))
        {
            if(isGameInit)
            {
                isGameInit = false; 
            }
            else
            {
                indexCurrentPlayer++;
                if(indexCurrentPlayer>= players.Count)
                {
                    indexCurrentPlayer = 0; 
                }
              
                currentPlayer = players[indexCurrentPlayer];
                newPlayer = true;               

            }

        }

        GetInfoHits();
    }
    private void mapStringHit()
    {
        
        string[]  arrayHit = hitsString.Split('-');
        colorHit = arrayHit[1]; 
        if (hitsString.Contains("EYE"))
        {
            factorHit = FactorHit.EYE;
            pointsHit = -1; 
        }
        else
        {
            pointsHit = Int32.Parse(arrayHit[0]);

            positionHit = Int32.Parse(arrayHit[2]);

            
            if (colorHit.Contains("R") || colorHit.Contains("G"))
            {
                if (positionHit == 1)
                {
                    factorHit = FactorHit.DOUBLE;
                }
                else if (positionHit == 2)
                {
                    factorHit = FactorHit.TRIPLE;
                }

            }
            else
            {
                factorHit = FactorHit.SINGLE; 
            }
        }

    }
    public int getNumberOfTeams()
    {
        return dictionaryTeams.Count;
    }

    IEnumerator loadPlayers()
    {
        UnityWebRequest www = UnityWebRequest.Get(urlPlayers);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string[] sections1 = www.downloadHandler.text.Split(';');

            foreach (string playerText in sections1)
            {
                if (playerText.Length > 1)
                {
                    Player player = new Player();
                    string[] sections2 = playerText.Split(',');

                    player.id = Convert.ToInt32(sections2[0]);
                    player.name = sections2[1];
                    player.team = Convert.ToInt32(sections2[2]);

                    players.Add(player);

                    if(!dictionaryTeams.ContainsKey(player.team))
                    {
                        dictionaryTeams.Add(player.team, new List<Player>());
                    }
                    dictionaryTeams[player.team].Add(player);

                }
            }



            Debug.Log("Players Size:  " + players.Count);
        }


        if (players.Count <= 0)
        {
            SceneManager.LoadScene("Error");
            stateGame = StateGame.ERROR;
            messageError = "The players were not loaded";
            SceneManager.LoadScene("Error");
        }
    }



    void Start()
    {

    }
    void changeScene()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            stateGame = StateGame.IDLE;
            SceneManager.LoadScene("Idle");

        }
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            stateGame = StateGame.PLAYERS;
            SceneManager.LoadScene("Players");
            if (players.Count <= 0)
            {
                StartCoroutine(loadPlayers());
            }
        }
        if (players.Count > 0)
        {


            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                stateGame = StateGame.MEDAL_TABLE;
                SceneManager.LoadScene("MedalTable");
            }
            else
            {

                bool newScene = false;
                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
                {
                    stateGame = StateGame.DEMOLITION;
                    SceneManager.LoadScene("Demolition");
                    newScene = true;

                }
                else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
                {
                    stateGame = StateGame.KILLER;
                    SceneManager.LoadScene("Killer");
                    newScene = true;

                }
                else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
                {
                    stateGame = StateGame.QUACK_SHOT;
                    SceneManager.LoadScene("QuackShot");
                    newScene = true;

                }
                else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
                {
                    stateGame = StateGame.SHANGAI;
                    SceneManager.LoadScene("Shangai");
                    newScene = true;
                }
                if (newScene)
                {
                    isGameInit = true; 
                    currentPlayer = players[0];
                    indexCurrentPlayer = 0;
                    newPlayer = true; 
                    if (!conectedToDartsDectection)
                    {
                        connectDartDectetion();
                    }
                }
            }
        }


    }


    // Update is called once per frame
    void Update()
    {
        changeScene();


    }
}
