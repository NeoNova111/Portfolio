using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool Contains(this LayerMask layermask, int layer)
    {
        if (layer == -1) return false;

        return layermask == (layermask | (1 << layer));
    }

    public static Transform ClosestTransformToTarget(this Transform[] transforms, Vector3 target)
    {
        Transform closest = transforms[0];
        for(int i = 1; i < transforms.Length; i++)
        {
            if (Vector3.Distance(closest.position, target) > Vector3.Distance(transforms[i].position, target)) closest = transforms[i];
        }
        return closest;
    }

    public static Vector3 FarthestPointOnBounds(this Collider col, Vector3 position)
    {
        Vector3 positionToCollider = col.transform.position - position;
        Vector3 otherSide = col.transform.position + positionToCollider;
        return col.ClosestPointOnBounds(otherSide);
    }

    public static Vector3 ToCardinalDirection(this Vector3 v)
    {
        int largestIndex = 0;
        for(int i = 0; i < 3; i++)
        {
            largestIndex = Mathf.Abs(v[i]) > Mathf.Abs(v[largestIndex]) ? i : largestIndex;
        }

        float newLargest = v[largestIndex] > 0 ? 1 : -1;
        v = Vector3.zero;
        v[largestIndex] = newLargest;

        return v;
    }

    //grid napping
    public static Vector3 SnapToGrid(this Vector3 pos, float gridScale)
    {
        return pos = new Vector3(       Mathf.Round(pos.x / gridScale) * gridScale,
                                        Mathf.Round(pos.y / gridScale) * gridScale,
                                        Mathf.Round(pos.z / gridScale) * gridScale);
    }

    public static Vector2 SnapToGrid(this Vector2 pos, float gridScale)
    {
        return pos = new Vector2(       Mathf.Round(pos.x / gridScale) * gridScale,
                                        Mathf.Round(pos.y / gridScale) * gridScale);
    }

    public static float SnapToGrid(this float f, float gridScale)
    {
        return Mathf.Round(f / gridScale) * gridScale;
    }

    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }
}
