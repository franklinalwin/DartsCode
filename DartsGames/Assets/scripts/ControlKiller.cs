using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlKiller : MonoBehaviour
{
    // Start is called before the first frame update
    public Text lblPlayer;
    public Text lblHits;
    public Text lblTeam;
    public Text lblInfoHits;
    public ControlGame controlGame;
    void Start()
    {

    }
    void Awake()
    {
        

    }

    void Update()
    {
        if (ControlGame.instance.newPlayer )
        {
            ////////////No Quitar/////////
            ControlGame.instance.newPlayer = false; 
            ////////////////////////

            lblPlayer.text = "Player: "+ ControlGame.instance.currentPlayer.name +" of  "+ ControlGame.instance.players.Count;
            lblTeam.text = "Team: " + ControlGame.instance.currentPlayer.team + " of  " + ControlGame.instance.dictionaryTeams.Count;

            lblHits.text = "";
            lblInfoHits.text = "";
        }
        if (ControlGame.instance.newThrow )
        {
            ////////////No Quitar/////////
            ControlGame.instance.newThrow = false;
            ////////////////////////

            lblHits.text = "Points: " + ControlGame.instance.hitsString;

            if(ControlGame.instance.hitsString.Contains("NONE"))
            {
                lblInfoHits.text = "";  
            }
            else
            {
                int valuePoint = ControlGame.instance.pointsHit;
                string colorPoint = ControlGame.instance.colorHit;
                FactorHit factor = ControlGame.instance.factorHit;
                lblInfoHits.text = "Info point: " + valuePoint.ToString() + "  " + colorPoint + " " + factor.ToString();

            }
          
            
        }


    }
}
