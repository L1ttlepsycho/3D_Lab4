using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameComponent;
using Action;
using System;
using UnityEngine.UI;


public class GenGameObj : MonoBehaviour
{
    public  List<GameObject> priests = new List<GameObject>();
    public List<GameObject> devils = new List<GameObject>();
    public GameObject priest,devil,rightShore, leftShore,boat;

    protected LOCATIONS locs = new LOCATIONS();

    private BoatBehavior boatBehavior;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            GameObject p = Instantiate(priest);
            p.name = "Priest " + (i + 1);
            p.tag = "Priest";
            p.transform.position = locs.leftShore_spaces[i];
            p.AddComponent<CreatureBehavior>();
            priests.Add(p);
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject d = Instantiate(devil);
            d.name = "Devil " + (i + 1);
            d.tag = "Devil";
            d.transform.position = locs.leftShore_spaces[i+3];
            d.AddComponent<CreatureBehavior>();
            devils.Add(d);
        }
        rightShore = Instantiate(rightShore);
        rightShore.name = "RightShore";
        rightShore.transform.position = locs.rightShore_loc;

        leftShore = Instantiate(leftShore);
        leftShore.name = "LeftShore";
        leftShore.transform.position = locs.leftShore_loc;

        boat= Instantiate(boat);
        boat.name = "Boat";
        boat.AddComponent<BoatBehavior>();
        boatBehavior = boat.GetComponent<BoatBehavior>();
        boat.transform.position = locs.boat_dst_l;

        MainSceneController.getInstance().setMainSceneController(this);
    }

    public void priestGetOn()
    {
        if (boatBehavior.isMoving) return;
        if (boatBehavior.isBoatAtLeftSide())
        {
            for (int i = 0; i < priests.Count; i++)
            {
                CreatureBehavior creatureBehavior = priests[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onLeftShore&&!creatureBehavior.onBoat)
                {
                    if (boatBehavior.l_empty)
                    {
                        creatureBehavior.getOnBoat(true,locs.boat_space1_l,true);
                        priests[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(true);
                    }
                    else if (boatBehavior.r_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space2_l, false);
                        priests[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(false);
                    }
                    break;
                }
            }
        }
        else
        { 
            for (int i = 0; i < priests.Count; i++)
            {
                CreatureBehavior creatureBehavior = priests[i].GetComponent<CreatureBehavior>();
                if (!creatureBehavior.onLeftShore && !creatureBehavior.onBoat)
                { 
                    if (boatBehavior.l_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space2_r, true);
                        priests[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(true);
                    }
                    else if (boatBehavior.r_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space1_r, false);
                        priests[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(false);
                    }
                    break;
                }
            }
        }
    }

    public void devilGetOn()
    {
        if (boatBehavior.isMoving) return;
        if (boatBehavior.isBoatAtLeftSide())
        {
            for (int i = 0; i < devils.Count; i++)
            {
                CreatureBehavior creatureBehavior = devils[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onLeftShore && !creatureBehavior.onBoat)
                {
                    if (boatBehavior.l_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space1_l,true);
                        devils[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(true);
                    }
                    else if (boatBehavior.r_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space2_l,false);
                        devils[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(false);
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < devils.Count; i++)
            {
                CreatureBehavior creatureBehavior = devils[i].GetComponent<CreatureBehavior>();
                if (!creatureBehavior.onLeftShore && !creatureBehavior.onBoat)
                {
                    if (boatBehavior.l_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space2_r,true);
                        devils[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(true);
                    }
                    else if (boatBehavior.r_empty)
                    {
                        creatureBehavior.getOnBoat(true, locs.boat_space1_r,false);
                        devils[i].transform.parent = boat.transform;
                        boatBehavior.sitOnPos(false);
                    }
                    break;
                }
            }
        }
    }

    public void priestGetOff()
    {
        if (boatBehavior.isMoving) return;
        if (boatBehavior.onLeftShore)
        {
            for (int i = 0; i < priests.Count; i++)
            {
                CreatureBehavior creatureBehavior = priests[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onBoat)
                {
                    priests[i].transform.parent = boat.transform.parent;
                    Vector3 nextpos = creatureBehavior.getOriginPos();
                    if (!creatureBehavior.onLeftShore)
                    {
                        creatureBehavior.onLeftShore ^= true;
                        nextpos.x = -nextpos.x;
                    }
                    bool pos=creatureBehavior.getOffBoat(true, nextpos);
                    boatBehavior.getOffPos(pos);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < priests.Count; i++)
            {
                CreatureBehavior creatureBehavior = priests[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onBoat)
                {
                    priests[i].transform.parent = boat.transform.parent;
                    Vector3 nextpos = creatureBehavior.getOriginPos();
                    if (creatureBehavior.onLeftShore)
                    {
                        creatureBehavior.onLeftShore ^= true;
                        nextpos.x = -nextpos.x;
                    }
                    bool pos = creatureBehavior.getOffBoat(true, nextpos);
                    boatBehavior.getOffPos(pos);
                    break;
                }
            }
        }

    }

    public void devilGetOff()
    {
        if (boatBehavior.isMoving) return;
        if (boatBehavior.onLeftShore)
        {
            for (int i = 0; i < devils.Count; i++)
            {
                CreatureBehavior creatureBehavior = devils[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onBoat)
                {
                    devils[i].transform.parent = boat.transform.parent;
                    Vector3 nextpos = creatureBehavior.getOriginPos();
                    if (!creatureBehavior.onLeftShore)
                    {
                        creatureBehavior.onLeftShore ^= true;
                        nextpos.x = -nextpos.x;
                    }
            
                    bool pos = creatureBehavior.getOffBoat(true, nextpos);
                    boatBehavior.getOffPos(pos);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < devils.Count; i++)
            {
                CreatureBehavior creatureBehavior = devils[i].GetComponent<CreatureBehavior>();
                if (creatureBehavior.onBoat)
                {
                    devils[i].transform.parent = boat.transform.parent;
                    Vector3 nextpos = creatureBehavior.getOriginPos();
                    if (creatureBehavior.onLeftShore)
                    {
                        creatureBehavior.onLeftShore ^= true;
                        nextpos.x = -nextpos.x;
                    }
                    bool pos = creatureBehavior.getOffBoat(true, nextpos);
                    boatBehavior.getOffPos(pos);
                    break;
                }
            }
        }

    }

    public void boatMove()
    {   
        boatBehavior.boatMove();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
