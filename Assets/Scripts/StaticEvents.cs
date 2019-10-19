using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class StaticEvents
{
    public class PlayerOnMap : UnityEvent<Transform> { }
    //   public static PlayerOnMap eventPlayerOnMap = new PlayerOnMap();
    //static void EventPlayerOnMap() { }

    public static UnityEvent eventMapCreated = new UnityEvent();
    //static void EventMapCreated() { }
    /*
    public class MovePoint : UnityEvent<Vector3> { }
    public static MovePoint eventMovePoint = new MovePoint();
    public class LookPoint : UnityEvent<Vector3> { }
    public static LookPoint eventLookPoint = new LookPoint();
*/
    //    public class CameraFollow : UnityEvent<Transform> { }
    public class NewGOinList : UnityEvent<GameObject> { }
    public class RemoveGOfromList : UnityEvent<GameObject> { }
    public class ClearBots : UnityEvent { }
    public static NewGOinList eventNewGOinList = new NewGOinList();
    public static RemoveGOfromList eventRemoveGOfromList = new RemoveGOfromList();
    public static ClearBots eventClearBots = new ClearBots();

    public class PlayerGameObject : UnityEvent<GameObject> { }
    public static PlayerGameObject eventNewPlayerGameObject = new PlayerGameObject();

    //public class CameraFollow : UnityEvent<GameObject> {}
    // public static CameraFollow eventCameraFollow = new CameraFollow();
    //
    public class CameraMoveLook : UnityEvent<Vector3, Vector3> { }
    public static CameraMoveLook eventCameraMoveLook = new CameraMoveLook();

    public class CameraFloat : UnityEvent<Vector2> { }
    public static CameraFloat eventCameraFloat = new CameraFloat();

    public class ResetInventory : UnityEvent<List<MyItemInventory>> { }
    public static ResetInventory eventResetInventory = new ResetInventory();

    public class Hit : UnityEvent<object[]> { }         // float damage, Body Hunter, Body victim
    public static Hit eventHit = new Hit();             //body hit body

    public class Kill : UnityEvent<object[]> { }        //float damage, Body Hunter, Body victim
    public static Kill eventKill = new Kill();          //body killed body


}
