using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using laszip.net;
using UnityEditor;
public class LoadLidar : MonoBehaviour
{
    /// <summary>
    /// Represents a functional tuple that can be used to store
    /// two values of different types inside one object.
    /// </summary>
    /// <typeparam name="T1">The type of the first element</typeparam>
    /// <typeparam name="T2">The type of the second element</typeparam>
    public sealed class Tuple<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        /// <summary>
        /// Retyurns the first element of the tuple
        /// </summary>
        public T1 Item1
        {
            get { return item1; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T2 Item2
        {
            get { return item2; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("Tuple({0}, {1})", Item1, Item2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
            hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
            return hash;
        }

        public override bool Equals(object o)
        {
            if (!(o is Tuple<T1, T2>))
            {
                return false;
            }

            var other = (Tuple<T1, T2>)o;

            return this == other;
        }

        public bool Equals(Tuple<T1, T2> other)
        {
            return this == other;
        }

        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }
            if (a.item1 == null && b.item1 != null) return false;
            if (a.item2 == null && b.item2 != null) return false;
            return
                a.item1.Equals(b.item1) &&
                a.item2.Equals(b.item2);
        }

        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }

        public void Unpack(Action<T1, T2> unpackerDelegate)
        {
            unpackerDelegate(Item1, Item2);
        }
    }

    public string DatasetName = "autzen.laz";
    public string DatasetLocation = "Resources/LidarData";

    public double scaleFactor = 10000;

    private struct Point3D
    {
        public double X;
        public double Y;
        public double Z;
    }

    private Tuple<List<Vector3>, List<Color>> LoadLazData(string filename)
    {
        List<Vector3> pointList = new List<Vector3>();
        List<Color> colorList = new List<Color>();
        Tuple<List<Vector3>, List<Color>> result = new Tuple<List<Vector3>, List<Color>>(pointList,colorList);
        
        var lazReader = new laszip_dll();
		var compressed = true;
        string projectpath = Application.dataPath;
		lazReader.laszip_open_reader(projectpath+"/"+DatasetLocation+"/"+DatasetName, ref compressed);
		var numberOfPoints = lazReader.header.number_of_point_records;

		// Check some header values
		Console.WriteLine(lazReader.header.min_x);
        Console.WriteLine(lazReader.header.min_y);
        Console.WriteLine(lazReader.header.min_z);
        Console.WriteLine(lazReader.header.max_x);
        Console.WriteLine(lazReader.header.max_y);
        Console.WriteLine(lazReader.header.max_z);

		int classification = 0;
		var point = new Point3D();
		var coordArray = new double[3];

		// Loop through number of points indicated
		for (int pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
		{
			// Read the point
			lazReader.laszip_read_point();
				
			// Get precision coordinates
			lazReader.laszip_get_coordinates(coordArray);
			point.X = coordArray[0]/scaleFactor;
			point.Y = coordArray[1]/scaleFactor;
			point.Z = coordArray[2]/scaleFactor;
			
            // Get classification value
			classification = lazReader.point.intensity;
            pointList.Add(new Vector3((float)point.X, (float)point.Y, (float)point.Z));
		    try
		    {
                //		        colorList.Add(PointCloudMesh.IntToColor(classification, 256));
                colorList.Add(new Color(lazReader.point.rgb[0] / 255, lazReader.point.rgb[1] / 255, lazReader.point.rgb[2]/255,lazReader.point.rgb[3]));
            }
		    catch (Exception e)
		    {
		        Debug.Log(e);
                Debug.Log(classification);
                continue;
            }
		}

			// Close the reader
		lazReader.laszip_close_reader();
        return result;
    }

	// Use this for initialization
	void Start ()
	{
	    Tuple<List<Vector3>, List<Color>> points = LoadLazData("autzen.laz");
        GetComponent<PointCloudManager>().LoadDynamic(points.Item1.ToArray(),points.Item2.ToArray());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
