using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tree_ : MonoBehaviour
{
    public Vector2 _treeSize = new Vector2(30, 30);

    // [notice -"if zero then no limit"]
    public int _maxLeafs = 0; // if zero then no limit

    public int _maxRootLeafSide = 20; // Max size of One Tale;
    public int _minRootLeafSide = 4;
    public int _leafMaxOffser = 8;
    public int _leafMinOffser = 0;
    public int _stickWidth = 1;

    Leaf root; // root Tale

    public List<Leaf> _treeRootL = new List<Leaf>(); // the array of Root leafs (all the Root rooms)
    public List<Leaf> _treeL = new List<Leaf>(); // the array of Root leafs (all the rooms)

    public List<Stick> _treeS = new List<Stick>();  // the array of sticks (all the holes)

    public int[,] _treeInt;

    [System.Serializable]
    public class Stick
    {
        public Rect stickR;  // Stick rectangle       
        public Stick(Rect ch0, Rect ch1, int stickWidth)
        {
            Vector2 ch0C = GetCenter(ch0); // me center
            Vector2 ch1C = GetCenter(ch1); // brother center
            stickR = new Rect(
                  ch0C.x - (int)(1 * stickWidth / 2),
                  ch0C.y - (int)(1 * stickWidth / 2),
                (int)(ch1C.x - ch0C.x + stickWidth),
                (int)(ch1C.y - ch0C.y + stickWidth));
        }
        // return center of rectangle
        Vector2 GetCenter(Rect r)
        {
            return new Vector2(r.x + (int)r.width / 2, r.y + (int)r.height / 2);
        }
    }
    [System.Serializable]
    public class Leaf
    {
        public Rect leafR;  // Leaf rectangle
        public Leaf child0; // first child after split
        public Leaf child1; // Second child after split
        public Leaf(Rect r) { leafR = r; }
    }
    List<Leaf> Split(Leaf l)
    {
        bool splitH;    // type to split (true for -; and false for | )
        float bigSide;  // size of biger side of tale

        // we will find biger side and cut tale on that side
        if (l.leafR.width > l.leafR.height)
        {
            bigSide = l.leafR.width;
            splitH = false;
        }
        else if (l.leafR.width < l.leafR.height)
        {
            bigSide = l.leafR.height;
            splitH = true;
        }
        else // if width=height then random direction
        {
            bigSide = l.leafR.height;
            splitH = Random.Range(0f, 1f) > 0.5f;
        }
        // we will cut if biger side is bigger than maximum allowed side
        if (bigSide > _maxRootLeafSide) // if too big room then cut
        {
            float split = (int)Random.Range(_minRootLeafSide, bigSide - _minRootLeafSide); // determine where we're going to split

            if (splitH)
            {
                l.child0 = new Leaf(new Rect(l.leafR.x, l.leafR.y, l.leafR.width, split));
                l.child1 = new Leaf(new Rect(l.leafR.x, l.leafR.y + split, l.leafR.width, l.leafR.height - split));
            }
            else
            {
                l.child0 = new Leaf(new Rect(l.leafR.x, l.leafR.y, split, l.leafR.height));
                l.child1 = new Leaf(new Rect(l.leafR.x + split, l.leafR.y, l.leafR.width - split, l.leafR.height));
            }

            GenerateStick(new List<Leaf>() { l.child0, l.child1 });
            return new List<Leaf> { l.child0, l.child1 };
        }
        else
        {
            return null; // room wasn cut 
        }
    }
    public void GenerateTree()
    {
        _treeRootL.Clear();
        _treeS.Clear();
        _treeL.Clear();
        _treeInt = new int[(int)_treeSize.x, (int)_treeSize.y];

        GenerateRootLeafs();
        ClampSticksForMapSize();
        GenerateLeafs();
        GenerateIntArray();
    }
    void GenerateRootLeafs()
    {
        List<Leaf> toCut = new List<Leaf>();

        root = new Leaf(new Rect(0, 0, _treeSize.x, _treeSize.y));

        toCut.Add(root);
        do
        {
            for (int i = toCut.Count - 1; i >= 0; i--)
            {
                List<Leaf> listLeaf = Split(toCut[i]);

                if (listLeaf == null)
                {
                    // tale will be created, if each side biger than minimum allowed side
                    // if (toCut[i].leafR.width >= _minRootLeafSide && toCut[i].leafR.height >= _minRootLeafSide)
                    // {
                    _treeRootL.Add(toCut[i]);
                    // }
                }
                else
                {
                    toCut.AddRange(listLeaf);
                }
                toCut.RemoveAt(i);
            }
            // print("map.Count = " + _treeL.Count);
            //  print("toCut.Count = " + toCut.Count);
        } while (toCut.Count > 0 && AdditionalLimits());
        //print("map.Count = " + _mapL.Count);
    }
    void GenerateLeafs()
    {
        foreach (Leaf leaf in _treeRootL)
        {
            _treeL.Add(FixedRoom(leaf));
        }

    }
    Leaf FixedRoom(Leaf l)
    {
        Rect rect = new Rect();

        int a = Random.Range(_leafMinOffser, _leafMaxOffser);
        int b = Random.Range(_leafMinOffser, _leafMaxOffser);

        rect.x = l.leafR.x + a;
        rect.width = l.leafR.width - a - b; // to compensate x

        a = Random.Range(_leafMinOffser, _leafMaxOffser);
        b = Random.Range(_leafMinOffser, _leafMaxOffser);

        rect.y = l.leafR.y + a;
        rect.height = l.leafR.height - a - b; // to compensate y
        
        return new Leaf(rect);
    }

    void ClampSticksForMapSize()
    { // clamp stick size according to map limit
        foreach (Stick stick in _treeS)
        {
            stick.stickR.x = Mathf.Clamp(stick.stickR.x, 0, _treeSize.x);
            stick.stickR.y = Mathf.Clamp(stick.stickR.y, 0, _treeSize.y);
            if (stick.stickR.width > 0)
            {
                if (stick.stickR.x + stick.stickR.width > _treeSize.x)
                    stick.stickR.width -= stick.stickR.x + stick.stickR.width - _treeSize.x;
            }
            else
            {
                if (stick.stickR.x + stick.stickR.width < 0)
                    stick.stickR.width += stick.stickR.x + stick.stickR.width;
            }
            if (stick.stickR.y + stick.stickR.height > _treeSize.y)
                stick.stickR.height -= stick.stickR.y + stick.stickR.height - _treeSize.y;
        }
    }
    void GenerateStick(List<Leaf> l)
    {
        _treeS.Add(new Stick(l[0].leafR, l[1].leafR, _stickWidth));
    }
    // additionl limits can be added
    bool AdditionalLimits()
    {
        if (_maxLeafs == 0 || _maxLeafs > _treeRootL.Count) return true;
        else return false;
    }
    void Start()
    {
        GenerateTree();
    }

    void GenerateIntArray()
    {
        // fill the array with sticks(-1) and liafs(cnt++) 
        foreach (Stick stick in _treeS)
        {
            for (int i = (int)stick.stickR.x; i < stick.stickR.x + stick.stickR.width; i++)
            {
                for (int j = (int)stick.stickR.y; j < stick.stickR.y + stick.stickR.height; j++)
                {
                    _treeInt[i, j] = -1;
                }
            }
        }
        foreach (Leaf leaf in _treeL)
        {
            for (int i = (int)leaf.leafR.x; i < leaf.leafR.x + leaf.leafR.width; i++)
            {
                for (int j = (int)leaf.leafR.y; j < leaf.leafR.y + leaf.leafR.height; j++)
                {
                    _treeInt[i, j] = _treeL.IndexOf(leaf) + 1;
                }
            }
        }
    }
    public void PrintArray()
    {
        Debug.Log("StartArray");
        string stringOut;
        for (int i = 0; i < (int)_treeSize.x; i++)
        {
            stringOut = "";
            for (int j = 0; j < (int)_treeSize.y; j++)
            {
                stringOut += _treeInt[i, j] + ";";
            }
            Debug.Log(stringOut);
        }
        Debug.Log("End Array");
    }
}

