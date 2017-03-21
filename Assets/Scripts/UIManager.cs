using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public GameManager GM;
    public Canvas PauseMenuCanvas;


    // Use this for initialization
    void Start()
    {
        //--------------------------------------------------------------------------
        // Game Settings Related Code


        //--------------------------------------------------------------------------
        // Music Settings Related Code

    }

    // Update is called once per frame
    void Update()
    {
        ScanForKeyStroke();
    }

    void ScanForKeyStroke()
    {
        if (Input.GetKeyDown("escape")) GM.TogglePauseMenu();
    }

    //-----------------------------------------------------------
    // Game Options Function Definitions
    /*public void OptionSliderUpdate(float val) { ... }
    void SetCustomSettings(bool val) { ... }*/
    /*void WriteSettingsToInputText(GameSettings settings) { ... }*/

    //-----------------------------------------------------------
    // Music Settings Function Definitions
   
}