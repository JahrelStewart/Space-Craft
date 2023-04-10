using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{    
    [SerializeField] public Text scores;   

    // Update is called once per frame
    void Update()
    {
        /*if(PlayerPrefs.GetInt("tokensObtainedByPlayer") == PlayerPrefs.GetInt("tokensObtainedBySeeker"))
        {
            winner.text = "ITS A TIE :/".ToString();
        }
        else if(PlayerPrefs.GetInt("tokensObtainedByPlayer") > PlayerPrefs.GetInt("tokensObtainedBySeeker"))
        {
            winner.text = "YOU!!!".ToString();
        }
        else if (PlayerPrefs.GetInt("tokensObtainedByPlayer") < PlayerPrefs.GetInt("tokensObtainedBySeeker"))
        {
            winner.text = "THE SEEKER :(".ToString();
        }*/
    }
}
