using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class MyInventory : MonoBehaviour
{
    public Transform _invParent;
    GameObject slotPrefab;
    public GameObject SlotPrefab
    {
        get
        {
            if (slotPrefab) return slotPrefab;
            else
            {
                slotPrefab = (GameObject)(Resources.Load("Prefabs/ItemSlot"));
                return slotPrefab;
            }
        }
    }

    public List<MyInvSlot> slots = new List<MyInvSlot>();
    // public List<MyItemFloor> items = new List<MyItemFloor>();
    public List<MyItemInventory> itemsInventory = new List<MyItemInventory>();

    [System.Serializable]
    public class MyInvSlot
    {
        public Image image;
        public Transform _transform;
        public MyInvSlot(Transform go)
        {
            _transform = go;
            Transform goIm = go.GetChild(0);
            image = goIm.GetComponent<Image>();
            image.enabled = false;
        }
    }

    void OnEnable()
    {
        StaticEvents.eventResetInventory.AddListener(ResetInv);
    }

    void OnDisable()
    {
        StaticEvents.eventResetInventory.RemoveListener(ResetInv);
    }

    void Start()
    {
        // slotPrefab = (GameObject)(Resources.Load("Prefabs/ItemSlot"));
        //for (int i = 0; i < 10; i++)
        //{
        //    SlotAdd();
        //}
    }
    public void ResetInv(List<MyItemInventory> iis)
    {

        //itemsInventory = new List<MyItemInventory>();
        itemsInventory = iis;
        foreach (MyInvSlot sl in slots) Destroy(sl._transform.gameObject);
        slots = new List<MyInvSlot>();

        for (int i = 0; i < itemsInventory.Count; i++)
        {
            SlotAdd();
            slots[i].image.sprite = itemsInventory[i]._sprite;
            slots[i].image.enabled = true;
        }
    }

    public void ItemAdd(MyItemInventory item)
    {
        Debug.Log("received Add into inventory");
        for (int i = 0; i < itemsInventory.Count; i++)
        {
            if (itemsInventory[i] == null)
            {
                itemsInventory[i] = item;
                // add slots if not enough
                int j = itemsInventory.Count - slots.Count;
                itemsInventory.Add(item);
                //!!!
                //itemsInventory.Add(item)

                if (j > 0)
                    for (int k = j; k > 0; k++)
                        SlotAdd();

                slots[i].image.sprite = item._sprite;
                slots[i].image.enabled = true;
                return;
            }
        }
    }

    public void ItemRemove(MyItemInventory item)
    {
        for (int i = itemsInventory.Count - 1; i >= 0; i--)
        {
            if (itemsInventory[i] == item)
            {
                itemsInventory[i] = null;
                itemsInventory.RemoveAt(i);
                SlotRemove();

                return;
            }
        }
    }

    public void SlotAdd()
    {
        Transform go = Instantiate(SlotPrefab).transform;
        go.SetParent(_invParent);
        go.name = "ItemSlot" + slots.Count;
        MyInvSlot slot = new MyInvSlot(go);
        slots.Add(slot);
    }

    public void SlotRemove()
    {
        if (slots.Count > 0)
            slots.RemoveAt(slots.Count - 1);

    }
}
[System.Serializable]
public class MyItemInventory
{
    public int _id;
    public Sprite _sprite;
    public string _prefabName;
    public MyItemInventory(int id, Sprite sprite, string prefabName)
    {
        _id = id;
        _sprite = sprite;
        _prefabName = prefabName;
    }
}
