using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderGroup
{
    public LineRenderer lineRenderer;
    public float thickness;
    public Color color;
    public GameObject lineObj;
    public List<GameObject> markers;
    public Vector3[] points;
    public bool closeEnd;
    public bool useMarkers;
    public bool allowOverride = true;

    public LineRenderGroup(ref Vector3[] _points, Color _color, Material material = null, string _name = "LineRenderGroup", bool _useMarkers = true, bool _closeEnd = false, float _thickness = 1f, Transform _parent = null)
    {
        points = _points;
        thickness = _thickness;
        closeEnd = _closeEnd;

        lineObj = new GameObject(_name + "_Line");
        lineObj.transform.SetParent(_parent);

        color = _color;

        useMarkers = _useMarkers;
        if (useMarkers)
        {
            markers = new List<GameObject>();
            for (int i = 0; i < points.Length; i++)
            {
                markers.Add(MakePrimitive(PrimitiveType.Sphere, _name + "Marker_" + i.ToString(), points[i], Quaternion.identity, Color.white, null, 3, lineObj.transform));
            }
        }

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        
        if(material != null) lineRenderer.material = material;
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        lineRenderer.widthMultiplier = thickness;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.positionCount = (points.Length + (closeEnd ? 1 : 0));

        lineRenderer.material.SetColor("_Color", color);
        lineRenderer.material.SetColor("_EmissionColor", color);

        Renderer renderer = lineRenderer.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
            renderer.material.SetColor("_Color", color);
            renderer.material.SetColor("_EmissionColor", color);
        }

        UpdateLineRenderers();
    }

    public void LineColorChange(Color newColor)
    {
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;

        Renderer renderer = lineRenderer.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.SetColor("_MainColor", newColor);
            renderer.material.color = newColor;
        }
        lineRenderer.enabled = false;
    }

    public void UpdateLineRenderers()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
            if (useMarkers)
            {
                if (markers != null && markers.Count > 0)
                {
                    markers[i].transform.position = points[i];
                }
            }
        }

        if (closeEnd)
        {
            // draw an "extra" line between the end point and start point, to visually close the shape
            lineRenderer.SetPosition(points.Length, points[0]);
        }
    }

    public void UpdateReferences(ref Vector3[] newVectorArray)
    {
        points = newVectorArray;
        UpdateLineRenderers();
    }

    public void UpdateReferences(ref List<GameObject> newObjectList)
    {
        markers = newObjectList;
        UpdateLineRenderers();
    }

    public void UpdateReferences(ref Vector3 newPointA, ref Vector3 newPointB)
    {
        UpdateLineRenderers();
    }

    public static GameObject MakePrimitive(PrimitiveType type, string name, Vector3 position, Quaternion rotation, Color color, Material material = null, float scaleFactor = 1f, Transform parent = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        MonoBehaviour.Destroy(go.GetComponent<Collider>());
        go.transform.localScale = Vector3.one * scaleFactor;
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.SetParent(parent, true);

        Renderer renderer = go.GetComponent<Renderer>();
        renderer.material.color = color;

        return go;
    }
}