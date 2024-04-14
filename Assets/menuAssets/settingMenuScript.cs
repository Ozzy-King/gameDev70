using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class settingMenuScript : MonoBehaviour
{
    public GameObject mouseSensitivitySlider;
    public GameObject mouseSensitivityText;

    public void sliderUpdate()
    {
        string text = mouseSensitivitySlider.GetComponent<Slider>().value.ToString();
        TextMeshProUGUI test = mouseSensitivityText.GetComponent<TextMeshProUGUI>();
        test.text = text.Substring(0, Math.Min(3, text.Length));

        gameControllerScript gmShortcut = GameObject.Find("GameController").GetComponent<gameControllerScript>();
        gmShortcut.PlayerMouseSens = float.Parse(test.text);

        if (gmShortcut.gameState == gameControllerScript.gamestates.running)
        {
            GameObject.Find("player").GetComponent<playercameraLook>().mouseSensitivity = float.Parse(test.text);
        }
    }

    void OnEnable()
    {
        //set setting mouse sens value to mouse sens valu stroed in game controller
        setSensitivityValue(GameObject.Find("GameController").GetComponent<gameControllerScript>().PlayerMouseSens.ToString());

    }

    public void setSensitivityValue(string text) {
        
        TextMeshProUGUI test = mouseSensitivityText.GetComponent<TextMeshProUGUI>();
        test.text = text.Substring(0, Math.Min(3, text.Length));

        gameControllerScript gmShortcut = GameObject.Find("GameController").GetComponent<gameControllerScript>();
        gmShortcut.PlayerMouseSens = float.Parse(test.text);
        mouseSensitivitySlider.GetComponent<Slider>().value = float.Parse(test.text);
        if (gmShortcut.gameState == gameControllerScript.gamestates.running)
        {
            GameObject.Find("player").GetComponent<playercameraLook>().mouseSensitivity = float.Parse(test.text);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
