using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


public class CreateCube : MonoBehaviour {
	public string dataareatag = "DataContainer";
	// Use this for initialization
	void Start () {
        //Initial cube creation
        //Find all the created data points, and get the bounds
        foreach (var iList in GameObject.FindGameObjectWithTag(dataareatag).GetComponent<LoadVolume>().iListList)
        {
            float maxX = 0f, maxY = 0f, maxZ = 0f;
            float minX = 0f, minY = 0f, minZ = 0f;


            foreach (Vector4 particleEntry in iList)
            {
                //Find the bounds
                if (particleEntry.x > maxX)
                {
                    maxX = particleEntry.x;
                }
                if (particleEntry.y > maxY)
                {
                    maxY = particleEntry.y;
                }
                if (particleEntry.z > maxZ)
                {
                    maxZ = particleEntry.z;
                }
                if (particleEntry.x < minX)
                {
                    minX = particleEntry.x;
                }
                if (particleEntry.y < minY)
                {
                    minY = particleEntry.y;
                }
                if (particleEntry.z < minZ)
                {
                    minZ = particleEntry.z;
                }
            }

            List<Vector3> rightcube = new List<Vector3>();
            //Top left points
            rightcube.Add(new Vector3(minX, maxY, maxZ));//p1
            rightcube.Add(new Vector3(minX, maxY, minZ));//p0
                                                         //Bottom left points
            rightcube.Add(new Vector3(minX, minY, maxZ));//p2
            rightcube.Add(new Vector3(minX, minY, minZ));//p3
                                                         //Top right points
            rightcube.Add(new Vector3(maxX, maxY, maxZ));//p5
            rightcube.Add(new Vector3(maxX, maxY, minZ));//p4
                                                         //Bottom right points
            rightcube.Add(new Vector3(maxX, minY, maxZ));//p6
            rightcube.Add(new Vector3(maxX, minY, minZ));//p7

            GameObject initialCube = CreateDataManipulationCube(rightcube, gameObject.transform);

            //Give it its particles;
            initialCube.GetComponent<PointCloudMesh>().containedParticles =
                iList;
            initialCube.GetComponent<PointCloudMesh>().originalContainedParticles = iList.ToArray().ToList();
            initialCube.GetComponent<PointCloudMesh>().UpdateContainedPointDisplay();
          //  initialCube.transform.Rotate(Vector3.up, 90, Space.World);
           // initialCube.transform.Rotate(Vector3.forward, -90, Space.World);
        }
       
    }

    public static GameObject CreateDataGameObject(List<Vector4> containedPoints, Transform parentTransform)
    {
        float maxX = 0f, maxY = 0f, maxZ = 0f;
        float minX = 0f, minY = 0f, minZ = 0f;


        foreach (Vector4 particleEntry in containedPoints)
        {
            //Find the bounds
            if (Math.Abs(particleEntry.x) > maxX)
            {
                maxX = Math.Abs(particleEntry.x);
            }
            if (Math.Abs(particleEntry.y) > maxY)
            {
                maxY = Math.Abs(particleEntry.y);
            }
            if (Math.Abs(particleEntry.z) > maxZ)
            {
                maxZ = Math.Abs(particleEntry.z);
            }
            if (particleEntry.x < minX)
            {
                minX = particleEntry.x;
            }
            if (particleEntry.y < minY)
            {
                minY = particleEntry.y;
            }
            if (particleEntry.z < minZ)
            {
                minZ = particleEntry.z;
            }
        }

        List<Vector3> rightcube = new List<Vector3>();
        //Top left points
        rightcube.Add(new Vector3(minX, maxY, maxZ));//p1
        rightcube.Add(new Vector3(minX, maxY, minZ));//p0
                                                     //Bottom left points
        rightcube.Add(new Vector3(minX, minY, maxZ));//p2
        rightcube.Add(new Vector3(minX, minY, minZ));//p3
                                                     //Top right points
        rightcube.Add(new Vector3(maxX, maxY, maxZ));//p5
        rightcube.Add(new Vector3(maxX, maxY, minZ));//p4
                                                     //Bottom right points
        rightcube.Add(new Vector3(maxX, minY, maxZ));//p6
        rightcube.Add(new Vector3(maxX, minY, minZ));//p7

        GameObject initialCube = CreateDataManipulationCube(rightcube, parentTransform);

        //Give it its particles;
        initialCube.GetComponent<PointCloudMesh>().containedParticles =
            containedPoints;
        initialCube.GetComponent<PointCloudMesh>().originalContainedParticles = containedPoints.ToArray().ToList();
        initialCube.GetComponent<PointCloudMesh>().UpdateContainedPointDisplay();
        //initialCube.transform.Rotate(Vector3.up, 90, Space.World);
        //initialCube.transform.Rotate(Vector3.forward, -90, Space.World);
        return initialCube;
    }

    public static GameObject CreateDataManipulationCube(List<Vector3> vertices, Transform parentTransform)
    {
        GameObject dataManipulationCube = Instantiate(Resources.Load("DataManipulationCube")) as GameObject;
        MeshFilter filter = dataManipulationCube.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();
        dataManipulationCube.GetComponent<MeshFilter>().mesh = CreateCubeMesh(vertices);
        dataManipulationCube.AddComponent<BoundBoxes_BoundBox>();
        dataManipulationCube.GetComponent<BoundBoxes_BoundBox>().permanent = true;
       /* dataManipulationCube.transform.position = GameObject.FindGameObjectWithTag("DataContainer").transform.TransformPoint(dataManipulationCube.GetComponent<MeshFilter>().mesh.bounds.center);*/
        dataManipulationCube.AddComponent<DragWithMouse>();
        dataManipulationCube.AddComponent<Slice>();
        if(parentTransform != null)
            dataManipulationCube.transform.parent = parentTransform;
        dataManipulationCube.tag = "DataSquare";
        dataManipulationCube.GetComponent<MeshRenderer>().enabled = false;
        dataManipulationCube.AddComponent<PointCloudMesh>();
        dataManipulationCube.AddComponent<ParticlesFollowMouseDrag>();
        dataManipulationCube.GetComponent<ParticlesFollowMouseDrag>().ParentSquare = dataManipulationCube;
        //dataManipulationCube.AddComponent<Rotate>();
        //dataManipulationCube.AddComponent<DatasetObjectController>();
        for (int i = 0; i < GameObject.FindGameObjectWithTag("CameraContainer").transform.childCount; i++)
        {
            if(GameObject.FindGameObjectWithTag("CameraContainer").transform.GetChild(i).gameObject.activeSelf)
                GameObject.FindGameObjectWithTag("CameraContainer").transform.GetChild(i).GetComponent<Camera>().GetComponent<KinectIntegrationManager>().AddDragable(dataManipulationCube);
        }
        return dataManipulationCube;
    }

    public static Mesh CreateCubeMesh(List<Vector3> inputvertices)
    {
        Mesh mesh = new Mesh();
        mesh.Clear();

        #region Vertices
        Vector3 p0 = inputvertices.ToArray()[1];
        Vector3 p1 = inputvertices.ToArray()[0];
        Vector3 p2 = inputvertices.ToArray()[2];
        Vector3 p3 = inputvertices.ToArray()[3];

        Vector3 p4 = inputvertices.ToArray()[5];
        Vector3 p5 = inputvertices.ToArray()[4];
        Vector3 p6 = inputvertices.ToArray()[6];
        Vector3 p7 = inputvertices.ToArray()[7];

//		Vector3 p0 = new Vector3( -length * .5f,	-width * .5f, height * .5f );
//		Vector3 p1 = new Vector3( length * .5f, 	-width * .5f, height * .5f );
//		Vector3 p2 = new Vector3( length * .5f, 	-width * .5f, -height * .5f );
//		Vector3 p3 = new Vector3( -length * .5f,	-width * .5f, -height * .5f );	
//
//		Vector3 p4 = new Vector3( -length * .5f,	width * .5f,  height * .5f );
//		Vector3 p5 = new Vector3( length * .5f, 	width * .5f,  height * .5f );
//		Vector3 p6 = new Vector3( length * .5f, 	width * .5f,  -height * .5f );
//		Vector3 p7 = new Vector3( -length * .5f,	width * .5f,  -height * .5f );

        Vector3[] vertices = new Vector3[]
        {
            // Bottom
            p0, p1, p2, p3,

            // Left
            p7, p4, p0, p3,

            // Front
            p4, p5, p1, p0,

            // Back
            p6, p7, p3, p2,

            // Right
            p5, p6, p2, p1,

            // Top
            p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up 	= Vector3.up;
        Vector3 down 	= Vector3.down;
        Vector3 front 	= Vector3.forward;
        Vector3 back 	= Vector3.back;
        Vector3 left 	= Vector3.left;
        Vector3 right 	= Vector3.right;

        Vector3[] normales = new Vector3[]
        {
            // Bottom
            down, down, down, down,

            // Left
            left, left, left, left,

            // Front
            front, front, front, front,

            // Back
            back, back, back, back,

            // Right
            right, right, right, right,

            // Top
            up, up, up, up
        };
        #endregion	

        #region UVs
        Vector2 _00 = new Vector2( 0f, 0f );
        Vector2 _10 = new Vector2( 1f, 0f );
        Vector2 _01 = new Vector2( 0f, 1f );
        Vector2 _11 = new Vector2( 1f, 1f );

        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            _11, _01, _00, _10,

            // Left
            _11, _01, _00, _10,

            // Front
            _11, _01, _00, _10,

            // Back
            _11, _01, _00, _10,

            // Right
            _11, _01, _00, _10,

            // Top
            _11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
            // Bottom
            3, 1, 0,
            3, 2, 1,			

            // Left
            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

            // Front
            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

            // Back
            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

            // Right
            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

            // Top
            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        #region Compile
        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        #endregion

        mesh.RecalculateBounds();
        return mesh;
    }
}
