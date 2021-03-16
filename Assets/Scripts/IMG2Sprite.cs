using UnityEngine;
using System.Collections;
using System.IO;
 
public static class IMG2Sprite
{
    public static Sprite LoadNewSprite(string FilePath)
    {
        return Resources.Load<Sprite>(FilePath);
    }
}
