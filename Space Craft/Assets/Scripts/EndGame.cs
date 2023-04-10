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
        scores.text = PlayerPrefs.GetInt("points").ToString();
    }
}
