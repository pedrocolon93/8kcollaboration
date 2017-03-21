using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using ChunkedEnumerator;
public class Slice : MonoBehaviour {

    public bool boolhit = false;

    void OnMouseDown(){
        //if (type.CompareTo("right")==0)
            //Debug.Log ("Her");
        boolhit = true;
    }
    public string type = "Original";



    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            
            // if left mouse clicked, try slice this object
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            TrySlice(ray);


        }
    }
    private class Line3d{
        private Vector3 start, end;
        public float detectionThreshold = 1;
        public Line3d(Vector3 start, Vector3 end){
            this.start = start;
            this.end = end;
        }
        public bool checkMembership(Vector3 testpoint){
            
            Vector3 thisvector = end - start;
            Vector3 testvector = testpoint - start;
        
            thisvector.Normalize ();
            testvector.Normalize ();
//			bool tst = Vector3.up == Vector3.up;
//			bool res =  thisvector == testvector;
            Vector3 diffvector = thisvector-testvector;
            diffvector.x = Math.Abs (diffvector.x);
            diffvector.y = Math.Abs (diffvector.y);
            diffvector.z = Math.Abs (diffvector.z);
            float tolerance = 0.2f;
            if (diffvector.x < tolerance && diffvector.y < tolerance && diffvector.z < tolerance) {
                return true;
            }
            return false;
        }
    }
    private bool VerticalSlice(Vector3[] edges, RaycastHit hitpoint){
        Vector3 intersectionpoint = hitpoint.point;
        float maxX = 0, minX = 0, maxY = 0, minY = 0, maxZ = 0, minZ = 0;
        maxX = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.x;
        minX = -maxX;
        maxY = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.y;
        minY = -maxY;
        maxZ = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.z;
        minZ = -maxZ;

        Vector3 minVector = new Vector3();
        minVector.x = minX;
        minVector.y = minY;
        minVector.z = minZ;
        Vector3 maxVector = new Vector3();
        maxVector.x = maxX;
        maxVector.y = maxY;
        maxVector.z = maxZ;
        minVector += hitpoint.collider.bounds.center;
        maxVector += hitpoint.collider.bounds.center;

        Line3d fronttopside = new Line3d (new Vector3 (minVector.x, maxVector.y, minVector.z), new Vector3 (maxVector.x, maxVector.y, minVector.z));
        Line3d frontbottomside = new Line3d (new Vector3 (minVector.x, minVector.y, minVector.z), new Vector3 (minVector.x, minVector.y, minVector.z));
        Line3d backtopside = new Line3d (new Vector3 (minVector.x, maxVector.y, maxVector.z), new Vector3 (maxVector.x, maxVector.y, maxVector.z));
        Line3d backbottomside = new Line3d (new Vector3 (minVector.x, minVector.y, maxVector.z), new Vector3 (minVector.x, minVector.y, maxVector.z));


        if(fronttopside.checkMembership(hitpoint.point) || frontbottomside.checkMembership(hitpoint.point)
            ||backtopside.checkMembership(hitpoint.point)||backbottomside.checkMembership(hitpoint.point)){
            List<Vector3> leftcube = new List<Vector3>();
            //Top left points
            leftcube.Add(new Vector3(minVector.x, maxVector.y, maxVector.z));//p1
            leftcube.Add(new Vector3(minVector.x, maxVector.y, minVector.z));//p0
            //Bottom left points
            leftcube.Add(new Vector3(minVector.x, minVector.y, maxVector.z));//p2
            leftcube.Add(new Vector3(minVector.x, minVector.y, minVector.z));//p3
            //Top right points
            leftcube.Add(new Vector3(intersectionpoint.x, maxVector.y, maxVector.z));//p5
            leftcube.Add(new Vector3(intersectionpoint.x, maxVector.y, minVector.z));//p4
            //Bottom right points
            leftcube.Add(new Vector3(intersectionpoint.x, minVector.y, maxVector.z));//p6
            leftcube.Add(new Vector3(intersectionpoint.x, minVector.y, minVector.z));//p7

            //Create edges for right cube
            List<Vector3> rightcube = new List<Vector3>();
            //Top left points
            rightcube.Add(new Vector3(intersectionpoint.x, maxVector.y, maxVector.z));//p1
            rightcube.Add(new Vector3(intersectionpoint.x, maxVector.y, minVector.z));//p0
            //Bottom left points
            rightcube.Add(new Vector3(intersectionpoint.x, minVector.y, maxVector.z));//p2
            rightcube.Add(new Vector3(intersectionpoint.x, minVector.y, minVector.z));//p3
            //Top right points
            rightcube.Add(new Vector3(maxVector.x, maxVector.y, maxVector.z));//p5
            rightcube.Add(new Vector3(maxVector.x, maxVector.y, minVector.z));//p4
            //Bottom right points
            rightcube.Add(new Vector3(maxVector.x, minVector.y, maxVector.z));//p6
            rightcube.Add(new Vector3(maxVector.x, minVector.y, minVector.z));//p7
            StartCoroutine(Createleftcube (leftcube));
            StartCoroutine(Createrightcube(rightcube));
            StartCoroutine(Cleanup());
            return true;
        }

        return false;
    }
    private bool HorizontalSlice(Vector3[] edges, RaycastHit hitpoint){
        Vector3 intersectionpoint = hitpoint.point;
        float maxX = 0, minX = 0, maxY = 0, minY = 0, maxZ = 0, minZ = 0;
        maxX = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.x;
        minX = -maxX;
        maxY = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.y;
        minY = -maxY;
        maxZ = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.z;
        minZ = -maxZ;

        Vector3 minVector = new Vector3();
        minVector.x = minX;
        minVector.y = minY;
        minVector.z = minZ;
        Vector3 maxVector = new Vector3();
        maxVector.x = maxX;
        maxVector.y = maxY;
        maxVector.z = maxZ;
        minVector += hitpoint.collider.bounds.center;
        maxVector += hitpoint.collider.bounds.center;

        Line3d frontleftside = new Line3d (new Vector3 (minVector.x, minVector.y, minVector.z), new Vector3 (minVector.x, maxVector.y, minVector.z));
        Line3d frontrightside = new Line3d (new Vector3 (maxVector.x, minVector.y, minVector.z), new Vector3 (maxVector.x, maxVector.y, minVector.z));
        Line3d backleftside = new Line3d (new Vector3 (minVector.x, minVector.y, maxVector.z), new Vector3 (minVector.x, maxVector.y, maxVector.z));
        Line3d backrightside = new Line3d (new Vector3 (maxVector.x, minVector.y, maxVector.z), new Vector3 (maxVector.x, maxVector.y, maxVector.z));


        if(frontleftside.checkMembership(hitpoint.point) || frontrightside.checkMembership(hitpoint.point)
            ||backleftside.checkMembership(hitpoint.point)||backrightside.checkMembership(hitpoint.point)){
            List<Vector3> leftcube = new List<Vector3>();
            //Top left points
            leftcube.Add(new Vector3(minVector.x, maxVector.y, maxVector.z));//p1
            leftcube.Add(new Vector3(minVector.x, maxVector.y, minVector.z));//p0
            //Bottom left points
            leftcube.Add(new Vector3(minVector.x, intersectionpoint.y, maxVector.z));//p2
            leftcube.Add(new Vector3(minVector.x, intersectionpoint.y, minVector.z));//p3
            //Top right points
            leftcube.Add(new Vector3(maxVector.x, maxVector.y, maxVector.z));//p1
            leftcube.Add(new Vector3(maxVector.x, maxVector.y, minVector.z));//p0
            //Bottom right points
            leftcube.Add(new Vector3(maxVector.x, intersectionpoint.y, maxVector.z));//p6
            leftcube.Add(new Vector3(maxVector.x, intersectionpoint.y, minVector.z));//p7

            //Create edges for right cube
            List<Vector3> rightcube = new List<Vector3>();
            //Top left points
            rightcube.Add(new Vector3(minVector.x, intersectionpoint.y, maxVector.z));//p1
            rightcube.Add(new Vector3(minVector.x, intersectionpoint.y, minVector.z));//p0
            //Bottom left points
            rightcube.Add(new Vector3(minVector.x, minVector.y, maxVector.z));//p2
            rightcube.Add(new Vector3(minVector.x, minVector.y, minVector.z));//p3
            //Top right points
            rightcube.Add(new Vector3(maxVector.x, intersectionpoint.y, maxVector.z));//p1
            rightcube.Add(new Vector3(maxVector.x, intersectionpoint.y, minVector.z));//p0
            //Bottom right points
            rightcube.Add(new Vector3(maxVector.x, minVector.y, maxVector.z));//p6
            rightcube.Add(new Vector3(maxVector.x, minVector.y, minVector.z));//p7
            StartCoroutine(Createleftcube (leftcube));
            StartCoroutine(Createrightcube(rightcube));
            StartCoroutine(Cleanup());
            return true;
        }

        return false;
    }
    private bool ZSlice(Vector3[] edges, RaycastHit hitpoint){
        Vector3 intersectionpoint = hitpoint.point;
        float maxX = 0, minX = 0, maxY = 0, minY = 0, maxZ = 0, minZ = 0;
        maxX = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.x;
        minX = -maxX;
        maxY = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.y;
        minY = -maxY;
        maxZ = hitpoint.collider.GetComponent<MeshCollider>().bounds.extents.z;
        minZ = -maxZ;

        Vector3 minVector = new Vector3();
        minVector.x = minX;
        minVector.y = minY;
        minVector.z = minZ;
        Vector3 maxVector = new Vector3();
        maxVector.x = maxX;
        maxVector.y = maxY;
        maxVector.z = maxZ;
        minVector += hitpoint.collider.bounds.center;
        maxVector += hitpoint.collider.bounds.center;

        Line3d righttopside = new Line3d (new Vector3 (maxVector.x, maxVector.y, minVector.z), new Vector3 (maxVector.x, maxVector.y, maxVector.z));
        Line3d rightbottomside = new Line3d (new Vector3 (maxVector.x, minVector.y, minVector.z), new Vector3 (maxVector.x, minVector.y, maxVector.z));
        Line3d lefttopside = new Line3d (new Vector3 (minVector.x, maxVector.y, minVector.z), new Vector3 (minVector.x, maxVector.y, maxVector.z));
        Line3d leftbottomside = new Line3d (new Vector3 (minVector.x, minVector.y, minVector.z), new Vector3 (minVector.x, minVector.y, maxVector.z));


        if(righttopside.checkMembership(hitpoint.point) || rightbottomside.checkMembership(hitpoint.point)
            ||lefttopside.checkMembership(hitpoint.point)||leftbottomside.checkMembership(hitpoint.point)){
            List<Vector3> leftcube = new List<Vector3>();
            //Top left points
            leftcube.Add(new Vector3(minVector.x, maxVector.y, hitpoint.point.z));//p1
            leftcube.Add(new Vector3(minVector.x, maxVector.y, minVector.z));//p0
            //Bottom left points
            leftcube.Add(new Vector3(minVector.x, minVector.y, hitpoint.point.z));//p2
            leftcube.Add(new Vector3(minVector.x, minVector.y, minVector.z));//p3
            //Top right points
            leftcube.Add(new Vector3(maxVector.x, maxVector.y, hitpoint.point.z));//p5
            leftcube.Add(new Vector3(maxVector.x, maxVector.y, minVector.z));//p4
            //Bottom right points
            leftcube.Add(new Vector3(maxVector.x, minVector.y, hitpoint.point.z));//p6
            leftcube.Add(new Vector3(maxVector.x, minVector.y, minVector.z));//p7

            //Create edges for right cube
            List<Vector3> rightcube = new List<Vector3>();
            //Top left points
            rightcube.Add(new Vector3(minVector.x, maxVector.y, maxVector.z));//p1
            rightcube.Add(new Vector3(minVector.x, maxVector.y, hitpoint.point.z));//p0
            //Bottom left points
            rightcube.Add(new Vector3(minVector.x, minVector.y, maxVector.z));//p2
            rightcube.Add(new Vector3(minVector.x, minVector.y, hitpoint.point.z));//p3
            //Top right points
            rightcube.Add(new Vector3(maxVector.x, maxVector.y, maxVector.z));//p5
            rightcube.Add(new Vector3(maxVector.x, maxVector.y, hitpoint.point.z));//p4
            //Bottom right points
            rightcube.Add(new Vector3(maxVector.x, minVector.y, maxVector.z));//p6
            rightcube.Add(new Vector3(maxVector.x, minVector.y, hitpoint.point.z));//p7
            StartCoroutine(Createleftcube(leftcube));
            StartCoroutine(Createrightcube(rightcube));
            StartCoroutine(Cleanup());
            return true;
        }

        return false;
    }
    public bool leftcubedone = false, rightcubedone = false;
    IEnumerator Cleanup()
    {
        while (!(leftcubedone && rightcubedone))
            yield return null;
//        boolhit = true;
        yield return null;
        gameObject.GetComponent<PointCloudManager>().DestroyPointCloud();
        yield return null;
        Camera.main.GetComponent<KinectIntegrationManager>().RemoveDragable(gameObject);
        yield return null;
        Destroy(gameObject);
    }

    public bool TrySlice(Ray ray)
    {
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
            return false;
        if (boolhit)
            return false;
        //Debug.Log ("Proceeding to slice");
        //Debug.Log (type);
        Vector3 intersectionpoint = hit.point;
        MeshFilter mf = hit.collider.GetComponentInParent<MeshFilter>();
        if (!mf.Equals(gameObject.GetComponent<MeshFilter>()))
            return false;
        
        Mesh intersectedcubemesh = mf.mesh;
        Vector3[] edges = intersectedcubemesh.vertices;
        //Identify where the intersection occurred
        float maxX = 0, minX = 0, maxY = 0, minY = 0, maxZ = 0, minZ = 0;
        maxX = hit.collider.GetComponent<MeshCollider>().bounds.extents.x;
        minX = -maxX;
        maxY = hit.collider.GetComponent<MeshCollider>().bounds.extents.y;
        minY = -maxY;
        maxZ = hit.collider.GetComponent<MeshCollider>().bounds.extents.z;
        minZ = -maxZ;

        Vector3 minVector = new Vector3();
        minVector.x = minX;
        minVector.y = minY;
        minVector.z = minZ;
        Vector3 maxVector = new Vector3();
        maxVector.x = maxX;
        maxVector.y = maxY;
        maxVector.z = maxZ;
        minVector += hit.collider.bounds.center;
        maxVector += hit.collider.bounds.center;


        boolhit = true;
        if (HorizontalSlice(edges, hit))
        {
          
            return true;
        }
        else if (VerticalSlice(edges, hit))
        {
           
            return true;
        }
        else if (ZSlice(edges, hit))
        {
            
            return true;
        }
        else
        {
            boolhit = false;
            return false;
        }
    }
    IEnumerator Createleftcube(List<Vector3> leftcube){
        GameObject leftcubego = CreateCube.CreateDataManipulationCube(leftcube, GetComponent<Transform>().parent);
//        leftcubego.GetComponent<Slice>().boolhit = true;
        leftcubego.GetComponent<Slice>().type = "left";
        leftcubego.transform.parent = gameObject.transform.parent;
        Vector3 newPosition = new Vector3();
        leftcubego.transform.position = newPosition;
        leftcubego.GetComponent<PointCloudMesh>().containedParticles = new List<Vector4>();
        Bounds leftCubeBounds = leftcubego.GetComponent<MeshCollider>().bounds;
        List<Vector4> ContainedPoints = new List<Vector4>();
        List<CheckMembershipJob> membershipJobs = new List<CheckMembershipJob>();
        double chunksize = (gameObject.GetComponent<PointCloudMesh>().containedParticles.Count/Environment.ProcessorCount*1.0);
        int chunks = Convert.ToInt32(Math.Ceiling(chunksize));
        List < IEnumerable < Vector4 >> actualchunks = null;
        actualchunks = gameObject.GetComponent<PointCloudMesh>().containedParticles.Chunk(chunks).ToList();
        foreach (var chunk in actualchunks )
        {
            CheckMembershipJob j = new CheckMembershipJob();
            j.InDataVector4Array = chunk.ToArray();
            j.CubeBounds = leftCubeBounds;
            j.OutDataList = new List<Vector4>();
            j.Start();
            membershipJobs.Add(j);
            yield return null;
        }
        foreach (var checkMembershipJob in membershipJobs)
        {
            while (!checkMembershipJob.Update())
            {
                yield return null;
            }
            ContainedPoints.AddRange(checkMembershipJob.OutDataList);
        }
        membershipJobs.Clear();
        leftcubego.GetComponent<PointCloudMesh>().originalContainedParticles =
            ContainedPoints.ToArray().ToList();
        leftcubego.GetComponent<PointCloudMesh>().containedParticles=
            ContainedPoints.ToArray().ToList();
        leftcubego.GetComponent<PointCloudMesh>().UpdateContainedPointDisplay();
        yield return null;
        leftcubedone = true;
    }
    IEnumerator Createrightcube(List<Vector3> leftcube)
    {
        GameObject leftcubego = CreateCube.CreateDataManipulationCube(leftcube, GetComponent<Transform>().parent);
//        leftcubego.GetComponent<Slice>().boolhit = true;
        leftcubego.GetComponent<Slice>().type = "left";
        leftcubego.transform.parent = gameObject.transform.parent;
        Vector3 newPosition = new Vector3();
        leftcubego.transform.position = newPosition;
        leftcubego.GetComponent<PointCloudMesh>().containedParticles = new List<Vector4>();
        Bounds leftCubeBounds = leftcubego.GetComponent<MeshCollider>().bounds;
        List<Vector4> ContainedPoints = new List<Vector4>();
        List<CheckMembershipJob> membershipJobs = new List<CheckMembershipJob>();
        double chunksize = (gameObject.GetComponent<PointCloudMesh>().containedParticles.Count / Environment.ProcessorCount * 1.0);
        int chunks = Convert.ToInt32(Math.Ceiling(chunksize));
        List<IEnumerable<Vector4>> actualchunks = null;
        actualchunks = gameObject.GetComponent<PointCloudMesh>().containedParticles.Chunk(chunks).ToList();
        foreach (var chunk in actualchunks)
        {
            CheckMembershipJob j = new CheckMembershipJob();
            j.InDataVector4Array = chunk.ToArray();
            j.CubeBounds = leftCubeBounds;
            j.OutDataList = new List<Vector4>();
            j.Start();
            membershipJobs.Add(j);
            yield return null;
        }
        foreach (var checkMembershipJob in membershipJobs)
        {
            while (!checkMembershipJob.Update())
            {
                yield return null;
            }
            ContainedPoints.AddRange(checkMembershipJob.OutDataList);
        }
        membershipJobs.Clear();
        leftcubego.GetComponent<PointCloudMesh>().originalContainedParticles =
            ContainedPoints.ToArray().ToList();
        leftcubego.GetComponent<PointCloudMesh>().containedParticles =
            ContainedPoints.ToArray().ToList();
        yield return null;
        leftcubego.GetComponent<PointCloudMesh>().UpdateContainedPointDisplay();
        rightcubedone = true;
    }
}
