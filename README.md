# Unity实现flash小游戏 "Priests and Devils"(动作分离版)
## 基本实现思路
实现一个```SSAction```基类，并且实现游戏中核心动作——移动类```SSMoveToAction```，通过```SSActionManager```进行对动作的管理，基本与教材上一致。但是由于船和牧师与魔鬼移动时将牧师与魔鬼作为船的子对象，所以本项目不需要使用```CCActionSequence```。

本来想实现动作队列的（即前一个动作未完成时下一个动作入队但不执行），但由于已有代码的掣肘暂时无法实现:(

## 代码实现
### 实现基类```SSAction```
#### 实现回调函数
```
public enum SSActionEventType : int { Started, Terminated };

    public interface ISSActionCallBack
    {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Terminated,
            int intParam = 0, string strParam = null, Object objectParam = null);
    }
```

#### 基类实现
```
public class SSAction : ScriptableObject
    {
        public bool enabled = true;
        public bool destroyed = false;

        public GameObject gameobject;
        public Transform transform;
        public ISSActionCallBack callback;

        protected SSAction() { }
        // Start is called before the first frame update
        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        // Update is called once per frame
        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }
```
### 具体动作类```SSMoveToAction```
```
public class SSMoveToAction : SSAction
    {
        public Vector3 target;
        public float speed;

        private SSMoveToAction() { }
        public static SSMoveToAction GetSSAction(Vector3 target, float speed)
        {
            SSMoveToAction action = ScriptableObject.CreateInstance<SSMoveToAction>();
            action.target = target;
            action.speed = speed;
            return action;
        }

        public override void Update()
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
            if (this.transform.position == target)
            {
                this.destroyed = true;
                this.callback.SSActionEvent(this);
            }
        }

        public override void Start()
        {

        }
    }
```
### 实现```SSActionManager```及```MyActionManager```
```
public class SSActionManager : MonoBehaviour,ISSActionCallBack
    {
        private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    
        private List<SSAction> waitingAdd = new List<SSAction>();                       
        private List<int> waitingDelete = new List<int>();                                         

        protected void Update()
        {
            foreach (SSAction ac in waitingAdd)
            {
                actions[ac.GetInstanceID()] = ac;                                      
            }
            waitingAdd.Clear();

            foreach (KeyValuePair<int, SSAction> kv in actions)
            {
                SSAction ac = kv.Value;
                if (ac.destroyed)          
                {
                    waitingDelete.Add(ac.GetInstanceID());
                }
                else if (ac.enabled)
                {
                    ac.Update();
                }
            }

            foreach (int key in waitingDelete)
            {
                SSAction ac = actions[key];
                actions.Remove(key);
                DestroyObject(ac);
            }
            waitingDelete.Clear();
        }

        public void RunAction(GameObject gameobject, SSAction action, ISSActionCallBack manager)
        {
            action.gameobject = gameobject;
            action.transform = gameobject.transform;
            action.callback = manager;
            waitingAdd.Add(action);
            action.Start();
        }
        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Terminated,
            int intParam = 0, string strParam = null, Object objectParam = null)
        {
            
        }
    }
```
```
public class MyActionManager : SSActionManager
    {
        public SSMoveToAction boat_move;
        public SSMoveToAction creature_move;
        public MainSceneController controller;

        protected new void Start()
        {
            controller = MainSceneController.getInstance();    
        }
        public void moveBoat(GameObject boat, Vector3 target, float speed)
        {
            boat_move = SSMoveToAction.GetSSAction(target, speed);
            this.RunAction(boat, boat_move, this);
        }
        public void moveCreature(GameObject creature, Vector3 target, float speed)
        {
            creature_move = SSMoveToAction.GetSSAction(target, speed);
            this.RunAction(creature, creature_move, this);
        }
    }
```
### 实现一个裁判类向```MainSceneController```返回游戏是否结束
```
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
```

### 对于原有```BoatBehavior```与```CreatureBehavior```的修改
```
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
```
```
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
```
## 效果展示

