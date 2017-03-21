using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CheckMembershipJob : ThreadedJob
{
    public Vector3 InDataVector3;
    public Vector4 InDataVector4;
    public Vector3[] InDataVector3Array;
    public Vector4[] InDataVector4Array;  // arbitary job data
    public Vector3[] OutDataVector3Array; // arbitary job data
    public Vector4[] OutDataVector4Array;
    public List<Vector4> OutDataList;
    public Bounds CubeBounds;
    protected override void ThreadFunction()
    {
        for (
            int i = 0; i < InDataVector4Array.Length;i++)
        {
            Vector4 particle = InDataVector4Array[i];
            if (CubeBounds.Contains(new Vector3(particle.x, particle.y, particle.z)))
            {
                OutDataList.Add(particle);
            }
        }
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        
    }
}