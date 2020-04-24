using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppUtils 
{
    
    public static Vector3 LatLonToPositionOnSphere(float lt, float ln, float _radius)
    {
        float ltR = lt * Mathf.Deg2Rad;
        float lnR = ln * Mathf.Deg2Rad;
        float xPos = (_radius) * Mathf.Cos(ltR) * Mathf.Cos(lnR);
        float zPos = (_radius) * Mathf.Cos(ltR) * Mathf.Sin(lnR);
        float yPos = (_radius) * Mathf.Sin(ltR);

        return new Vector3(xPos, yPos, zPos);
    }

    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    private const int CircleSegmentCount = 24;
    private const int CircleVertexCount = CircleSegmentCount + 2;
    private const int CircleIndexCount = CircleSegmentCount * 3;
    public static float angle;
    public static Mesh GenerateCircleMesh(int maxCount)
    {
        var circle = new Mesh();
        var vertices = new List<Vector3>(CircleVertexCount);
        var indices = new int[CircleIndexCount];
        var segmentWidth = Mathf.PI * 2f / CircleSegmentCount;
        angle = 0f;
        vertices.Add(Vector3.zero);
        // for (int i = 1; i < CircleVertexCount; ++i)
        for (int i = 1; i < maxCount + 2; ++i)
        {
            vertices.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
            angle -= segmentWidth;
            if (i > 1)
            {
                var j = (i - 2) * 3;
                indices[j + 0] = 0;
                indices[j + 1] = i - 1;
                indices[j + 2] = i;
            }
        }
        circle.SetVertices(vertices);
        circle.SetIndices(indices, MeshTopology.Triangles, 0);
        circle.RecalculateBounds();
        return circle;
    }

}
