using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameComponent 
{ 
    public class LOCATIONS 
    {
        public List<Vector3> leftShore_spaces = new List<Vector3>();
        public List<Vector3> rightShore_spaces = new List<Vector3>();
        public Vector3 leftShore_loc;
        public Vector3 rightShore_loc;
        public Vector3 boat_dst_r;
        public Vector3 boat_dst_l;
        public Vector3 boat_space1_l;
        public Vector3 boat_space2_l;
        public Vector3 boat_space1_r;
        public Vector3 boat_space2_r;
        public LOCATIONS()
        {
            leftShore_loc = new Vector3(-15, 0.4f, 0);
            rightShore_loc = new Vector3(15, 0.4f, 0);
            boat_dst_r = new Vector3(8.5f, 0, 0);
            boat_dst_l = new Vector3(-8.5f, 0, 0);
            boat_space1_l = new Vector3(-9.5f,1,0);
            boat_space2_l = new Vector3(-7.5f, 1, 0);
            boat_space1_r = new Vector3(9.5f, 1, 0);
            boat_space2_r = new Vector3(7.5f, 1, 0);

            for (int i = 0; i < 6; i++)
            {
                rightShore_spaces.Add(new Vector3(12 + i * 1.2f, 2.3f, 0));
                leftShore_spaces.Add(new Vector3(-12 - i * 1.2f, 2.3f, 0));
            }
        }
    }

    public class GameJudge : IGameCondition
    {
        public  int lShoreNumDevil, rShoreNumDevil, rShoreNumPriest, lShoreNumPriest, boatPriest, boatDevil;
        public GameJudge()
        {

        }
        public void status_PriestOn(bool isLeftshore)
        {
            if (isLeftshore)
            {
                lShoreNumPriest--;
            }

            else
            {
                rShoreNumPriest--;
            }
            boatPriest++;
        }

        public void status_PriestOff(bool isLeftshore)
        {
            if (isLeftshore)
                lShoreNumPriest++;
            else
            {
                rShoreNumPriest++;
            }
            boatPriest--;
        }
        public void status_DevilOn(bool isLeftshore)
        {
            if (isLeftshore)
                lShoreNumDevil--;
            else
            {
                rShoreNumDevil--;
            }
            boatDevil++;
        }

        public void status_DevilOff(bool isLeftshore)
        {
            if (isLeftshore)
                lShoreNumDevil++;
            else
            {
                rShoreNumDevil++;
            }
            boatDevil--;
        }

        public bool isOver()
        {
            if ((lShoreNumDevil + boatDevil > lShoreNumPriest + boatPriest && lShoreNumPriest + boatPriest > 0) || (lShoreNumDevil > lShoreNumPriest && lShoreNumPriest > 0))
            {
                showGameText("Mission Failed!");
                return true;
            }
            else if ((rShoreNumDevil + boatDevil > rShoreNumPriest + boatPriest && rShoreNumPriest + boatPriest > 0) || (rShoreNumDevil > rShoreNumPriest && rShoreNumPriest > 0))
            {
                showGameText("Mission Failed!");
                return true;
            }
            else if (lShoreNumDevil == 0 && lShoreNumPriest == 0)
            {
                showGameText("Mission Successed!");
                return true;
            }
            else
            {
                return false;
            }
        }



        public void showGameText(string s)
        {
            GameObject Canvas = Camera.Instantiate(Resources.Load("prefab/Canvas")) as GameObject;
            GameObject GameText = Camera.Instantiate(Resources.Load("prefab/Text"), Canvas.transform) as GameObject;
            GameText.transform.position = Canvas.transform.position;
            GameText.GetComponent<Text>().text = s;
        }
    }
    public interface IPlayerAction
    {
        void boatMove();
        void priestOn();
        void devilOn();
        void priestOff();
        void devilOff();
    }

    public interface IGameCondition
    {
        void status_PriestOn(bool isLeftshore);

        void status_PriestOff(bool isLeftshore);
        void status_DevilOn(bool isLeftshore);

        void status_DevilOff(bool isLeftshore);

        bool isOver();
    }

    public class MainSceneController: System.Object,IPlayerAction
    {
        private static MainSceneController instance;
        public GenGameObj gameObj;
        public GameJudge gamejudge=new GameJudge();

        private int  lShoreNumDevil, rShoreNumDevil,rShoreNumPriest,lShoreNumPriest,boatPriest,boatDevil;  

        public static MainSceneController getInstance()
        {
            if (instance == null)
                instance = new MainSceneController();
            return instance;
        }

        internal void setMainSceneController(GenGameObj _gameObj)
        {
            gameObj = _gameObj;
            lShoreNumDevil = 3;
            rShoreNumDevil = 0;
            lShoreNumPriest = 3;
            rShoreNumPriest = 0;
            boatDevil = 0;
            boatPriest = 0;
            updateGameJudge();
        }
        internal void updateGameJudge()
        {
            gamejudge.lShoreNumDevil=lShoreNumDevil;
            gamejudge.rShoreNumDevil=rShoreNumDevil;
            gamejudge.lShoreNumPriest=lShoreNumPriest;
            gamejudge.rShoreNumPriest=rShoreNumPriest;
            gamejudge.boatDevil=boatDevil;
            gamejudge.boatPriest=boatPriest;
        }

        //IUserAction interfaces
        public void boatMove()
        {
            gameObj.boatMove();
        }
        public void priestOn()
        {
            gameObj.priestGetOn();
        }
        public void devilOn()
        {
            gameObj.devilGetOn();
        }
        public void priestOff()
        {
            gameObj.priestGetOff();
        }
        public void devilOff()
        {
            gameObj.devilGetOff();
        }
    }
}


