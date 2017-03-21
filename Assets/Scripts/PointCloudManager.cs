using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PointCloudManager : MonoBehaviour {

    // File
    public string dataPath;
    private string filename;
    public Material matVertex;

    // GUI
    private float progress = 0;
    private string guiText;
    private bool loaded = false;

    // PointCloud
    private GameObject pointCloud;

    public float scale = 1;
    public bool invertYZ = false;
    public bool forceReload = false;

    public int numPoints;
    public int numPointGroups;
    private int limitPoints = 65000;

    private Vector3[] points;
    private Color[] colors;
    private Vector3 minValue;

    public void UpdatePointCloudPosition(Vector3 positionDelta)
    {
        try
        {
            pointCloud.transform.position += positionDelta;
        }
        catch (Exception e)
        {
            return;
        }
    }

    public void DestroyPointCloud()
    {
        Destroy(pointCloud);
    }


    public void LoadDynamic(Vector3[] positions, Color[] colors)
    {
        points = positions;
        this.colors = colors;
        numPoints = points.Length;
        minValue = new Vector3();

        // Instantiate Point Groups
        numPointGroups = Mathf.CeilToInt(numPoints * 1.0f / limitPoints * 1.0f);

        pointCloud = new GameObject();


        for (int i = 0; i < numPointGroups - 1; i++)
        {
            StartCoroutine(InstantiateMeshNoSave(i, limitPoints));
//            if (i % 10 == 0)
//            {
//                guiText = i.ToString() + " out of " + numPointGroups.ToString() + " PointGroups loaded";
//            }
        }
        StartCoroutine(InstantiateMeshNoSave(numPointGroups - 1, numPoints - (numPointGroups - 1) * limitPoints));
        //Store PointCloud
        loaded = true;
    }
    
    IEnumerator InstantiateMeshNoSave(int meshInd, int nPoints)
    {
        // Create Mesh
        GameObject pointGroup = new GameObject(filename + meshInd);
        pointGroup.AddComponent<MeshFilter>();
        pointGroup.AddComponent<MeshRenderer>();
        pointGroup.GetComponent<Renderer>().material = matVertex;
        Mesh m = new Mesh();

        pointGroup.GetComponent<MeshFilter>().mesh = m;

//        CreateMesh(meshInd, nPoints, limitPoints, m);

        int id = meshInd;
        Mesh mesh = m;

        Vector3[] myPoints = new Vector3[nPoints];
        int[] indecies = new int[nPoints];
        Color[] myColors = new Color[nPoints];

        for (int i = 0; i < nPoints; ++i)
        {
            try
            {
                myPoints[i] = new Vector3(points[id * limitPoints + i].x - minValue.x, points[id * limitPoints + i].y - minValue.y, points[id * limitPoints + i].z - minValue.z);
                indecies[i] = i;
                myColors[i] = colors[id * limitPoints + i];
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                continue;
            }
            if(i%1000 == 0)
                yield return null;
        }


        mesh.vertices = myPoints;
        mesh.colors = myColors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
        mesh.uv = new Vector2[nPoints];
        mesh.normals = new Vector3[nPoints];


        pointGroup.transform.parent = pointCloud.transform;
        yield return pointGroup;
    }

    void calculateMin(Vector3 point){
        if (minValue.magnitude == 0)
            minValue = point;
        if (point.x < minValue.x)
            minValue.x = point.x;
        if (point.y < minValue.y)
            minValue.y = point.y;
        if (point.z < minValue.z)
            minValue.z = point.z;
    }
    /*
    void OnGUI(){


        if (!loaded){
            GUI.BeginGroup (new Rect(Screen.width/2-100, Screen.height/2, 400.0f, 20));
            GUI.Box (new Rect (0, 0, 200.0f, 20.0f), guiText);
            GUI.Box (new Rect (0, 0, progress*200.0f, 20), "");
            GUI.EndGroup ();
        }
    }
    */
}
