using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    // It extends Vector3, so that it can return location in two dimensions. 
    public static Vector2 PositionToVector2(this Vector3 pos) => new Vector2(pos.x, pos.z);
}
