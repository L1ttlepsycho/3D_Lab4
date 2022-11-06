using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameComponent;

namespace Action
{
    public enum SSActionEventType : int { Started, Terminated };

    public interface ISSActionCallBack
    {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Terminated,
            int intParam = 0, string strParam = null, Object objectParam = null);
    }

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

    public class CCActionSequence : SSAction, ISSActionCallBack
    {
        public List<SSAction> sequence;    //动作的列表
        public int repeat = -1;            //-1就是无限循环做组合中的动作
        public int start = 0;              //当前做的动作的索引

        public static CCActionSequence GetSSAction(int repeat, int start, List<SSAction> sequence)
        {
            CCActionSequence action = ScriptableObject.CreateInstance<CCActionSequence>();
            action.repeat = repeat;
            action.sequence = sequence;
            action.start = start;
            return action;
        }

        public override void Update()
        {
            if (sequence.Count == 0) return;
            if (start < sequence.Count)
            {
                sequence[start].Update();
            }
        }
        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Terminated,
            int intParam = 0, string strParam = null, Object objectParam = null)
        {
            source.destroyed = false;
            this.start++;
            if (repeat > 0)
            {
                repeat--;
            }
            if (repeat == 0)
            {
                this.destroyed = true;
                this.callback.SSActionEvent(this);
            }
        }

        public override void Start()
        {
            foreach (SSAction action in sequence)
            {
                action.gameobject = this.gameobject;
                action.transform = this.transform;
                action.callback = this;
                action.Start();
            }
        }

        void onDestroy()
        {

        }
    }

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
}
