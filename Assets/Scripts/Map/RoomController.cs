using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public bool _border;
    public List<GameObject> goList = new List<GameObject>();
    GameObject _treeGO;
    Tree_ _tree;
    Vector3 _caveFloorPos = new Vector3();

    // [SerializeField]
    // GameObject _playerPrefab;
    GameObject _player;

    public bool instantiateAbstraction; // to instantiate non Array metod preview

    void Start()
    {
        _treeGO = new GameObject();
        _treeGO.name = "Tree";
        Generate();
    }
    void Update()
    {
        // if (Input.anyKeyDown) { Generate(); }
    }
    public void MakeCave()
    {
        Generate();
    }

    void Generate()
    {

        ProjectIOSingletone.Get().ClearBots(); // destroy all bots
        foreach (GameObject g in goList) { Destroy(g); } // destroy go in lits, such as floor stuf
        goList.Clear();

        _tree = GetComponent<Tree_>();
        _tree.GenerateTree();
        //**       _tree.PrintArray();
        if (instantiateAbstraction)
        {
            InstantiatorRootLeaf(_tree._treeRootL);
            InstantiatorStick(_tree._treeS);
            InstantiatorLeaf(_tree._treeL);
        }

        InstantiatorCave(_tree);//._treeInt);

        ProjectIOSingletone.Get().map = _tree;

        Vector3 top, look;
        top = (new Vector3(_caveFloorPos.x, _caveFloorPos.y + 50, _caveFloorPos.z));
        //look = new Vector3(_caveFloorPos.x, 0, _caveFloorPos.y);
        look = new Vector3(_caveFloorPos.x + _tree._treeSize.x / 2, _caveFloorPos.y, _caveFloorPos.z + _tree._treeSize.y / 2);

        StaticEvents.eventCameraMoveLook.Invoke(top, look);
    }
    void InstantiatorRootLeaf(List<Tree_.Leaf> map)
    {
        foreach (Tree_.Leaf tale in map)
        {

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0);
            GameObject goCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goCube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
            goCube.transform.SetParent(go.transform);

            go.transform.localScale = new Vector3(tale.leafR.width, 1, tale.leafR.height);
            go.transform.position = new Vector3(tale.leafR.x, 0, tale.leafR.y);

            Renderer r = goCube.GetComponent<Renderer>();
            Color c = GenerateColor();
            r.material.color = c;
            go.name = "Root_" + map.IndexOf(tale);
            goList.Add(go);
            go.transform.SetParent(_treeGO.transform);

        }
    }
    void InstantiatorStick(List<Tree_.Stick> map)
    {
        foreach (Tree_.Stick tale in map)
        {

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0);
            GameObject goCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goCube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
            goCube.transform.SetParent(go.transform);

            go.transform.localScale = new Vector3(tale.stickR.width, 1.5f, tale.stickR.height);
            go.transform.position = new Vector3(tale.stickR.x, 0, tale.stickR.y);

            Renderer r = goCube.GetComponent<Renderer>();
            Color c = Color.black; //GenerateColor();
            r.material.color = c;
            go.name = "Stick_" + map.IndexOf(tale);
            goList.Add(go);
            go.transform.SetParent(_treeGO.transform);
            //  print("tale.stickR " + tale.stickR);
        }
    }
    void InstantiatorLeaf(List<Tree_.Leaf> map)
    {
        foreach (Tree_.Leaf tale in map)
        {

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0);
            GameObject goCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goCube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
            goCube.transform.SetParent(go.transform);

            go.transform.localScale = new Vector3(tale.leafR.width, 2f, tale.leafR.height);
            go.transform.position = new Vector3(tale.leafR.x, 0, tale.leafR.y);

            Renderer r = goCube.GetComponent<Renderer>();
            Color c = Color.white; //GenerateColor();
            r.material.color = c;
            go.name = "Leaf_" + map.IndexOf(tale);
            goList.Add(go);
            go.transform.SetParent(_treeGO.transform);
            //  print("tale.stickR " + tale.stickR);
        }
    }
    //void Instantiator(List<Tree_> map)
    //{
    //    foreach (Tree_ tale in map)
    //    {

    //        GameObject go = new GameObject();
    //        go.transform.position = new Vector3(0, 0);
    //        GameObject goCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //        goCube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
    //        goCube.transform.SetParent(go.transform);

    //        go.transform.localScale = new Vector3(tale.taleR.width, 1, tale.taleR.height);
    //        go.transform.position = new Vector3(tale.taleR.x, 0, tale.taleR.y);

    //        Renderer r = goCube.GetComponent<Renderer>();
    //        Color c = GenerateColor();
    //        r.material.color = c;

    //        goList.Add(go);
    //    }
    //}
    void InstantiatorCave(Tree_ tree)//int[,] array)
    {

        //GameObject goFloor = FloorPlane(tree);
        GameObject goFloor = FloorCube(tree);
        goFloor.name = "Floor";
        goList.Add(goFloor);

        GameObject go = null;

        if (_border)
        {
            int i, j;
            i = -1;// i < tree._treeSize.x; i++)
            for (j = -1; j < tree._treeSize.y; j++)
            {
                go = BorderCube(i, j);
                go.name = "Border_" + i + ";" + j;
                go.transform.parent = goFloor.transform;

                go = BorderCube(j + 1, i);
                go.name = "Border_" + j + ";" + i;
                go.transform.parent = goFloor.transform;
            }
            i = (int)tree._treeSize.x;// i < tree._treeSize.x; i++)
            for (j = -1; j <= tree._treeSize.y; j++)
            {
                go = BorderCube(i, j);
                go.name = "Border_" + i + ";" + j;
                go.transform.parent = goFloor.transform;

                go = BorderCube(j, i);
                go.name = "Border_" + j + ";" + i;
                go.transform.parent = goFloor.transform;
            }


        }

        for (int i = 0; i < tree._treeSize.x; i++)
        {
            for (int j = 0; j < tree._treeSize.y; j++)
            {
                if (tree._treeInt[i, j] == 0)
                {
                    go = WallCube(i, j);
                    go.name = "Wall_" + i + ";" + j;
                }
                if (go)
                {
                    go.transform.parent = goFloor.transform;
                }
            }
        }

        //*****
        _caveFloorPos = MapPosition(tree);
        goFloor.transform.position = _caveFloorPos;// new Vector3(tree._treeSize.x, 0, tree._treeSize.y);

    }
    //void InstantiatePlayer(Tree_ tree)
    //{
    //    // make List of all free pos on map
    //    List<Vector2> freePos = new List<Vector2>();
    //    Vector3 playerPos = new Vector3();
    //    for (int i = 0; i < tree._treeSize.x; i++)
    //    {
    //        for (int j = 0; j < tree._treeSize.y; j++)
    //        {
    //            if (tree._treeInt[i, j] != 0)//if(tree._treeInt[i,j]==1|| tree._treeInt[i, j] == -1)
    //            {
    //                freePos.Add(new Vector2(i, j));
    //            }
    //        }
    //    }
    //    // choose random free pos on map
    //    if (freePos.Count < 1) { Debug.LogWarning("No empty points on the cave"); }
    //    else
    //    {
    //        int pos = Random.Range(0, freePos.Count);
    //        playerPos = new Vector3(freePos[pos].x, 1, freePos[pos].y);
    //    }

    //    _player = StaticInstantiateBody.InstantiateBody((playerPos + _caveFloorPos), "Player");

    //    goList.Add(_player);
    //    ProjectIOSingletone.Get().ThePlayerT = _player.transform;
    //}

    //public void MakePlayer()
    //{
    //    ProjectIOSingletone.DestroyObject(_player);
    //    //InstantiatePlayer(_tree);
    //    InstantiatePlayer(ProjectIOSingletone.Get().map);
    //}

    //public void MakeBot(string type)
    //{
    //    InstantiateBot(ProjectIOSingletone.Get().map, type);
    //}
    //public void InstantiateBot(Tree_ tree, string type)
    //{
    //    // make List of all free pos on map
    //    List<Vector2> freePos = new List<Vector2>();
    //    Vector3 botPos = new Vector3();
    //    for (int i = 0; i < tree._treeSize.x; i++)
    //    {
    //        for (int j = 0; j < tree._treeSize.y; j++)
    //        {
    //            if (tree._treeInt[i, j] != 0)//if(tree._treeInt[i,j]==1|| tree._treeInt[i, j] == -1)
    //            {
    //                freePos.Add(new Vector2(i, j));
    //            }
    //        }
    //    }
    //    // choose random free pos on map
    //    if (freePos.Count < 1) { Debug.LogWarning("No empty points on the cave"); }
    //    else
    //    {
    //        int pos = Random.Range(0, freePos.Count);
    //        botPos = new Vector3(freePos[pos].x, 1, freePos[pos].y);
    //    }

    //    //  _player = StaticInstantiateBody.InstantiateBody((playerPos + _caveFloorPos), "Player");
    //    ProjectIOSingletone.Get().AddToBots(StaticInstantiateBody.InstantiateBody((botPos + _caveFloorPos), "bot_" + type).transform);
    //    // goList.Add(_player);
    //    //  ProjectIOStatic.ThePlayerT = _player.transform;
    //}

    Color GenerateColor()
    {
        Color c = Color.white;
        switch (Random.Range(0, 7)) // i reserved 8 and 9 colors
        {
            case 0:
                c = Color.yellow;
                break;
            case 1:
                c = Color.blue;
                break;
            case 2:
                c = Color.cyan;
                break;
            case 3:
                c = Color.gray;
                break;
            case 4:
                c = Color.green;
                break;
            case 5:
                c = Color.magenta;
                break;
            case 6:
                c = Color.red;
                break;
            case 7:
                c = Color.black;
                break;
            case 8:
                c = Color.white;
                break;
        }
        return c;
    }
    Vector3 MapPosition(Tree_ tree)
    {
        return new Vector3(tree._treeSize.x, 0, tree._treeSize.y);
    }
    GameObject FloorPlane(Tree_ tree)
    {
        GameObject go = new GameObject();
        //go.transform.position = new Vector3(0+(int)(tree._treeSize.x/2),0, (int)(0 +tree._treeSize.y/2));

        GameObject goFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        goFloor.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        goFloor.transform.SetParent(go.transform);

        //go.transform.localScale = new Vector3(array.GetLength(0), 1, array.GetLength(0));
        go.transform.localScale = new Vector3(tree._treeSize.x / 10, 1f, tree._treeSize.y / 10);
        go.transform.position = new Vector3(0 + (tree._treeSize.x / 2), 0, (0 + tree._treeSize.y / 2));

        Renderer r = goFloor.GetComponent<Renderer>();
        r.material.SetTexture("_MainTex", StaticTreeTextures.FloorTexture());

        return go;
    }
    GameObject FloorCube(Tree_ tree)
    {
        GameObject go = new GameObject();
        go.transform.position = new Vector3(0, 0);

        GameObject goFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);

        goFloor.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        goFloor.transform.SetParent(go.transform);


        //Rigidbody goR = goFloor.AddComponent<Rigidbody>();
        //goR.isKinematic = true;
        //goR.useGravity = false;

        //go.transform.localScale = new Vector3(array.GetLength(0), 1, array.GetLength(0));
        go.transform.localScale = new Vector3(tree._treeSize.x, 0.1f, tree._treeSize.y);

        Renderer r = goFloor.GetComponent<Renderer>();
        r.material.SetTexture("_MainTex", StaticTreeTextures.FloorTexture());


        return go;
    }
    GameObject WallCube(int i, int j)
    {
        GameObject go = new GameObject();
        go.transform.position = new Vector3(0, 0);

        // GameObject goWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject goWall = StaticInstantiateWall.InstantiateWall("Wall");
         //goWall.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        goWall.transform.SetParent(go.transform);

       // go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.position = new Vector3(i, 0, j);

       // Renderer r = goWall.GetComponent<Renderer>();
        //r.material.SetTexture("_MainTex", StaticTreeTextures.WallTexture());

        //NavMeshObstacle nmo = goWall.AddComponent<NavMeshObstacle>();
        //nmo.carving = true;

        return go;
    }

    GameObject BorderCube(int i, int j)
    {
        GameObject go = new GameObject();
        go.transform.position = new Vector3(0, 0);

        GameObject goBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);

        goBorder.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        goBorder.transform.SetParent(go.transform);

        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.position = new Vector3(i, 0, j);

        Renderer r = goBorder.GetComponent<Renderer>();
        Color c = Color.black;
        r.material.color = c;
        return go;
    }
}
