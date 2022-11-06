using Action;
using GameComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehavior : MonoBehaviour
{   

    public bool isMoving;
    public bool onLeftShore;
    public bool l_empty,r_empty;

    private IGameCondition gameCon;
    public MyActionManager action_manager;
    private LOCATIONS loc=new LOCATIONS();
    private Vector3 direction=new Vector3(0.1f,0,0);
    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        onLeftShore = true;
        l_empty = true;
        r_empty = true;

        action_manager=gameObject.AddComponent<MyActionManager>() as MyActionManager;
        gameCon = MainSceneController.getInstance().gamejudge as IGameCondition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void boatMove()
    {
        if(onLeftShore)
        {
            action_manager.moveBoat(this.gameObject, loc.boat_dst_r, 15);
            gameCon.isOver();
            onLeftShore = false;
        }
            
        else
        {
            action_manager.moveBoat(this.gameObject, loc.boat_dst_l, 15);
            gameCon.isOver();
            onLeftShore = true;
        }
    }
    public bool isBoatAtLeftSide()
    {
        return onLeftShore;
    }


    public bool isLeftSeatEmpty()
    {
        return l_empty;
    }
    public bool isRightSeatEmpty()
    {
        return r_empty;
    }


    public void sitOnPos(bool isLeft)
    {
        if (isLeft)
            l_empty = false;
        else
            r_empty = false;
    }
    public void getOffPos(bool isLeft)
    {
        if (isLeft)
            l_empty = true;
        else
            r_empty = true;
    }

}
