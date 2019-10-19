using UnityEngine;
using System.Collections;

public class StaticInstantiateBody : MonoBehaviour
{

    //[SerializeField]
    public static GameObject _bodyPrefab;
    static GameObject _body;
    public static GameObject InstantiateBody(Vector3 pos, string bodyName, string bodyTag)
    {

        //_bodyPrefab = (GameObject)Instantiate(Resources.Load("Models/ManUnderwear0"));
        //_bodyPrefab = (GameObject)(Resources.Load("Prefabs/ManUnderwear"));
        // _bodyPrefab = (GameObject)(Resources.Load("Prefabs/Overlord"));
        _bodyPrefab = (GameObject)(Resources.Load("Prefabs/skeleton_static"));
        if (_bodyPrefab)
        {
            _body = Instantiate(_bodyPrefab);
        }
        else
        {
            _body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }

        _body.transform.position = pos;
        _body.name = bodyName;
        //_body.tag = bodyTag;
        // AddComponentsToBody(_body);
        // StaticEvents.eventCameraFollow.Invoke(_body);
        return _body;
    }
}
