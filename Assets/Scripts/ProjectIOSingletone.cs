using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ProjectIOSingletone : MonoBehaviour
{
    public MyInventory myInventory;
    // singleton
    private static ProjectIOSingletone m_Instance = null;
    public static ProjectIOSingletone Get()
    {
        if (m_Instance == null)
            m_Instance = (ProjectIOSingletone)FindObjectOfType(typeof(ProjectIOSingletone));
        return m_Instance;
    }
    // class 

    [SerializeField]
    int manyBots = 25;// populate many bots at a time

    [SerializeField]
    Body thePlayerBody;
    public Tree_ map;

    public List<Body> goList = new List<Body>();

    public List<string> tagList = new List<string> {//all tags for groups of bots
        "group_0",
        "group_1",
        "group_2"
    };
    public string NewTag(string i)
    {
        if (!tagList.Contains(i))
            tagList.Add(i);
        return i;
    }
    public string GetTag(int i)
    {
        if (i < 0 || i > tagList.Count - 1)
            i = Random.Range(0, tagList.Count);
        return tagList[i];
    }


    [SerializeField]
    public Body ThePlayerB
    {
        set
        {
            thePlayerBody = value;
            StaticEvents.eventNewGOinList.Invoke(thePlayerBody.gameObject);
            StaticEvents.eventNewPlayerGameObject.Invoke(thePlayerBody.gameObject);
            MyBodyInventoryAdd(thePlayerBody);

        }
        get { return thePlayerBody; }
    }


    [SerializeField]
    public static Vector3 GetKeyWASD()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    #region Input
    [System.NonSerialized]
    public float InpMouseAxisH = 0;
    [System.NonSerialized]
    public float InpMouseAxisV = 0;
    [System.NonSerialized]
    public float InputMouseWhill = 0;

    [System.NonSerialized]
    public float InpKeybAxisH = 0;
    [System.NonSerialized]
    public float InpKeybAxisV = 0;
    #endregion




    public void DestroyObject(Body objT)
    {
        bool destroyPlayer = false;
        if (goList.Contains(objT))
        {
            if (objT == ThePlayerB)
                destroyPlayer = true;
            StaticEvents.eventRemoveGOfromList.Invoke(objT.gameObject);
            // DestroyImmediate(objT.gameObject, false);
            Destroy(objT.gameObject);
            goList.Remove(objT);

            if (destroyPlayer)
                thePlayerBody = null;
        }
        else
            Debug.LogWarning("this bot DOES NOT exist in bot list - player tryin to be deleted on start");
    }

    public void AddToBots(Body go)
    {
        if (goList.Contains(go))
            Debug.LogWarning("this bot exists in bot list");
        else
        {
            goList.Add(go);
            // Debug.Log(goList.Count);
            StaticEvents.eventNewGOinList.Invoke(go.gameObject);
        }
    }

    public void ClearBots()
    {
        //Debug.Log("Cleaning bots");

        //foreach (Transform go in goList)
        //            DestroyObject(go);
        for (int i = goList.Count - 1; i > -1; i--)
        {
            DestroyObject(goList[i]);
            // Debug.Log("Cleaning bots_ " + i);
        }
        goList.Clear();
        StaticEvents.eventClearBots.Invoke();
    }

    public void MakeBot(string groupTag)
    {
        InstantiateBot(ProjectIOSingletone.Get().map, NewTag(groupTag));
    }
    void InstantiateBot(Tree_ tree, string groupTag)
    {
        // make List of all free pos on map
        List<Vector2> freePos = new List<Vector2>();
        Vector3 botPos = new Vector3();
        for (int i = 0; i < tree._treeSize.x; i++)
        {
            for (int j = 0; j < tree._treeSize.y; j++)
            {
                if (tree._treeInt[i, j] != 0)//if(tree._treeInt[i,j]==1|| tree._treeInt[i, j] == -1)
                {
                    freePos.Add(new Vector2(i, j));
                }
            }
        }
        // choose random free pos on map
        if (freePos.Count < 1) { Debug.LogWarning("No empty points on the cave"); }
        else
        {
            int pos = Random.Range(0, freePos.Count);
            botPos = new Vector3(freePos[pos].x, 1, freePos[pos].y);
        }

        //  _player = StaticInstantiateBody.InstantiateBody((playerPos + _caveFloorPos), "Player");
        Body body = StaticInstantiateBody.InstantiateBody((botPos + MapPosition(tree)), "bot_" + goList.Count, groupTag).GetComponent<Body>();
        body._tag = groupTag;
        body._tagFriendList.Add(groupTag);
        ProjectIOSingletone.Get().AddToBots(body);
        // goList.Add(_player);
        //  ProjectIOStatic.ThePlayerT = _player.transform;
    }

    void InstantiatePlayer(Tree_ tree)
    {
        // make List of all free pos on map
        List<Vector2> freePos = new List<Vector2>();
        Vector3 playerPos = new Vector3();
        for (int i = 0; i < tree._treeSize.x; i++)
        {
            for (int j = 0; j < tree._treeSize.y; j++)
            {
                if (tree._treeInt[i, j] != 0)//if(tree._treeInt[i,j]==1|| tree._treeInt[i, j] == -1)
                {
                    freePos.Add(new Vector2(i, j));
                }
            }
        }
        // choose random free pos on map
        if (freePos.Count < 1) { Debug.LogWarning("No empty points on the cave"); }
        else
        {
            int pos = Random.Range(0, freePos.Count);
            playerPos = new Vector3(freePos[pos].x, 1, freePos[pos].y);
        }

        ProjectIOSingletone.Get().ThePlayerB = StaticInstantiateBody.InstantiateBody((playerPos + MapPosition(tree)), "Player", "Player").GetComponent<Body>();

        goList.Add(ThePlayerB);
        //ProjectIOSingletone.Get().ThePlayerT = _player.transform;
    }

    public void MakePlayer()
    {
        ProjectIOSingletone.Get().DestroyObject(ThePlayerB);
        InstantiatePlayer(ProjectIOSingletone.Get().map);
    }
    Vector3 MapPosition(Tree_ tree)
    {
        return new Vector3(tree._treeSize.x, 0, tree._treeSize.y);
    }

    public void PopulateBots()
    {
        // int a;
        for (int i = 0; i < manyBots; i++)
        {
            // a = Random.Range(0, 2);
            // MakeBot("" + a);
            MakeBot(GetTag(-1));// random tag from tagList
        }
    }

    void Start()
    {
        // StartCoroutine(FixedUpdate_());
    }

    void Update()
    {
        InpMouseAxisH = Input.GetAxis("Mouse X");
        InpMouseAxisV = Input.GetAxis("Mouse Y");
        InputMouseWhill = Input.GetAxis("Mouse ScrollWheel");

        InpKeybAxisH = Input.GetAxisRaw("Horizontal");
        InpKeybAxisV = Input.GetAxisRaw("Vertical");

        if (thePlayerBody)
        {
            ArmUnarmBTN();
            AttackBTN();
            JumpBTN();
        }
    }

    void ArmUnarmBTN()
    {
        //listen to buttons
        if (Input.GetKeyDown(KeyCode.E))//||Input.GetButtonDown("Fire3"))
            thePlayerBody.ActionStack(Body.ActionUnit.toArmStateSwSh);
        //thePlayerBody.Arm(Body.ArmState.armSwSh);

        if (Input.GetKeyDown(KeyCode.Q))
            thePlayerBody.ActionStack(Body.ActionUnit.toArmStateMage);
        //thePlayerBody.Arm(Body.ArmState.armMage);
    }
    void AttackBTN()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // thePlayerBody.Attacking();
            thePlayerBody.ActionStack(Body.ActionUnit.attacking);
        }
    }
    void JumpBTN()
    {
        if (Input.GetButton("Jump"))
        {
            thePlayerBody.Jumping();
            // thePlayerBody.ActionStack(Body.actionUnit.jumping);
        }
    }
    public void MyBodyInventoryAdd(Body body)
    {
        if (body != ThePlayerB)
        {
            //Debug.Log(body + " != " + ThePlayerB+"   INV will not be refreshed");
            return;
        }

        if (!myInventory)
        {
            Debug.Log("Inventory not set in " + this);
            return;
        }
        StaticEvents.eventResetInventory.Invoke(thePlayerBody.ItemsInventory);

    }
    
}
