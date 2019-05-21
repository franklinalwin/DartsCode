using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlError : MonoBehaviour
{
    public Text lblError;
   
    void Start()
    {
        lblError.text = ControlGame.instance.messageError;
    }

    
    void Update()
    {
        
    }
}
