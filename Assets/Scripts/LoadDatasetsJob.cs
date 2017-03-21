using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LoadDatasetsJob : ThreadedJob
{
    public Vector3 InDataVector3;
    public Vector4 InDataVector4;
    public Vector3[] InDataVector3Array;
    public Vector4[] InDataVector4Array;  // arbitary job data
    public Vector3[] OutDataVector3Array; // arbitary job data
    public Vector4[] OutDataVector4Array;
    public List<List<Vector4>> iListList = null;
    public FileInfo file = null;
    protected override void ThreadFunction()
    {
        try
        {

            if (!file.Extension.Equals(".csv"))
                return;
            List<Vector4> iList = new List<Vector4>();
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(file.FullName, Encoding.Default);
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        string[] entries = line.Split(',');
                        // Do whatever you need to do with the text line, it's a string now
                        // In this example, I split it into arguments based on comma
                        // deliniators, then send that array to DoStuff()
                        float x = Convert.ToSingle(entries[0]);
                        float y = Convert.ToSingle(entries[1]);
                        float z = Convert.ToSingle(entries[2]);
                        float w = 20;
                        try
                        {
                            w = Convert.ToSingle(entries[3]);
                        }
                        catch (Exception e)
                        {
                            w = 20;
                        }
                        
                        Vector4 datapoint = new Vector4(x, y, z, w);
                        iList.Add(datapoint);
                        if (file.FullName.Contains("Fdatafdatarotated"))
                        {
                            Vector4 datapoint2 = datapoint + new Vector4(.2f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(-.2f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, .2f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, -.2f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, -.2f, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, .2f, 0);
                            iList.Add(datapoint2);

                            datapoint2 = datapoint + new Vector4(.5f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(-.5f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, .5f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, -.5f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, -.5f, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, .5f, 0);
                            iList.Add(datapoint2);
                            /*
                            datapoint2 = datapoint + new Vector4(.5f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(-.5f, 0, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0.5f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, -.5f, 0, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, -.5f, 0);
                            iList.Add(datapoint2);
                            datapoint2 = datapoint + new Vector4(0, 0, .5f, 0);
                            iList.Add(datapoint2);
                        */
                        }
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
                iListList.Add(iList);
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            //GameObject.FindGameObjectWithTag("CalibrationText").GetComponent<GUIText>().text = "Failed" + e.Message;
            

        }
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        
    }
}