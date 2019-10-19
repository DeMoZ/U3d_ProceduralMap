using UnityEngine;
using System.Collections;

public  class StaticTreeTextures
{
    public static Texture WallTexture()
    {
        Texture2D t = Resources.Load("_Textures/crate1D") as Texture2D;
        if (!t) { Debug.Log("Unable to Load WallTexture texture..."); }
        return t;
    }
    public static Texture FloorTexture()
    {
        Texture2D t = Resources.Load("_Textures/ground1D") as Texture2D;
        if (!t) { Debug.Log("Unable to Load FloorTexture texture..."); }
        return t;
    }

}
