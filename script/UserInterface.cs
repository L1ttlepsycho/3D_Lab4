using GameComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserInterface : MonoBehaviour
{
    IPlayerAction myActions;
    float btnWidth = (float)Screen.width / 6.0f;
    float btnHeight = (float)Screen.height / 6.0f;

    void Start()
    {
        myActions = MainSceneController.getInstance() as IPlayerAction;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(5, 250, btnWidth, btnHeight), "Priest GetOn"))
        {
            myActions.priestOn();
        }
        if (GUI.Button(new Rect(155, 250, btnWidth, btnHeight), "Priest GetOff"))
        {
            myActions.priestOff();
        }
        if (GUI.Button(new Rect(325, 250, btnWidth, btnHeight), "Boat Go!"))
        {
            myActions.boatMove();
        }
        if (GUI.Button(new Rect(505, 250, btnWidth, btnHeight), "Devil GetOn"))
        {
            myActions.devilOn();
        }
        if (GUI.Button(new Rect(655, 250, btnWidth, btnHeight), "Devil GetOff"))
        {
            myActions.devilOff();
        }

    }
}
