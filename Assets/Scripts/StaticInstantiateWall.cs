using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticInstantiateWall : MonoBehaviour
{
    public static GameObject _prefab;
    static GameObject _goReturn;
    static List<GameObject> _prefabList = new List<GameObject> {
        (GameObject)(Resources.Load("Prefabs/Barrel")),
        (GameObject)(Resources.Load("Prefabs/Cratesmall")),
        (GameObject)(Resources.Load("Prefabs/Cratetall"))
};
    public static GameObject InstantiateWall(string prefabName) 
    {
        //_bodyPrefab = (GameObject)Instantiate(Resources.Load("Models/ManUnderwear0"));
        //_bodyPrefab = (GameObject)(Resources.Load("Prefabs/ManUnderwear"));
        //2% probability to plug Torch
        if (Random.Range(1, 101) <= 2) _prefab = (GameObject)(Resources.Load("Prefabs/Torch1"));
        else _prefab = _prefabList[Random.Range(0, _prefabList.Count)];

        if (_prefab)
        {
            _goReturn = Instantiate(_prefab);
        }
        else
        {
            _goReturn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer r = _goReturn.GetComponent<Renderer>();
            r.material.SetTexture("_MainTex", StaticTreeTextures.WallTexture());
            UnityEngine.AI.NavMeshObstacle nmo = _goReturn.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.carving = true;
        }

        _goReturn.transform.position = new Vector3(0.5f, 0.5f, 0.5f);
        _goReturn.name = prefabName;



        return _goReturn;
    }  
}
