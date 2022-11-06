using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameComponent;
using Action;

public class CreatureBehavior : MonoBehaviour
{
    private Vector3 originPos;
    private IGameCondition gameCon;
    public MyActionManager action_manager;
    public bool onLeftShore, onBoat,posOnBoat;

    void Start()
    {
        originPos = this.transform.position;
        onBoat = false;
        onLeftShore = true;
        gameCon = MainSceneController.getInstance().gamejudge   as IGameCondition;
        action_manager = gameObject.AddComponent<MyActionManager>() as MyActionManager;
        posOnBoat = false;
    }

    void Update()
    {
        
    }

    public void getOnBoat(bool onable,Vector3 emptyPos,bool pos)
    {   
        if (onable)
        {
            action_manager.moveCreature(this.gameObject, emptyPos, 10);
            
            if (this.tag == "Priest")
            {
                gameCon.status_PriestOn(onLeftShore);
            }
            else
            {
                gameCon.status_DevilOn(onLeftShore);
            }
            onBoat = true;
            posOnBoat = pos;
        }
    }

    public bool getOffBoat(bool onable,Vector3 emptyPos)
    {
        if (onable)
        {
            action_manager.moveCreature(this.gameObject, emptyPos, 10);

            if (this.tag == "Priest")
            {
                gameCon.status_PriestOff(onLeftShore);
            }
            else
            {
                gameCon.status_DevilOff(onLeftShore);
            }
            onBoat = false;
            originPos = emptyPos;
        }
        return posOnBoat;
    }

    public Vector3 getOriginPos()
    {
        return originPos;
    }
}
