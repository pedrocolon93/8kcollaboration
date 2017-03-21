using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MoveParticlesJob : ThreadedJob
{
    public Vector3 InDataVector3;
    public Vector4 InDataVector4;
    public Vector3[] InDataVector3Array;
    public Vector4[] InDataVector4Array;  // arbitary job data
    public Vector3[] OutDataVector3Array; // arbitary job data
    public Vector4[] OutDataVector4Array;
    public List<Vector4> OutDataList;
    public Bounds CubeBounds;
    public Vector3 movementDelta;
    protected override void ThreadFunction()
    {
        for (int i = 0; i < OutDataList.Count; i++)
        {
            OutDataList[i] += new Vector4(movementDelta.x, movementDelta.y, movementDelta.z, 0);
            
        }
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        
    }
}