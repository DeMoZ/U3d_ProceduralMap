using UnityEngine;
using System.Collections;
[System.Serializable]
public class MyItemFloor : MonoBehaviour
{
    public MyItemInventory _ii;

    //public Sprite _sprite;

    public MyItemFloor(Sprite sprite)
    {
        _ii._sprite = sprite;
    }

    // public Collider C;
    void OnTriggerEnter(Collider c)
    {
        //  C = c;
        Body body = c.gameObject.GetComponent<Body>();
        if (body)
        {
            //body._items.Add(this);
            // body._items.Add(this);
            body.AddItemToInv(_ii);// new MyItemInventory(_ii._id, _ii._sprite, _ii._prefabName));

            Destroy(gameObject);
        }
    }

    // формула для скорости анимации

    // x<=0.5
    // 
}
