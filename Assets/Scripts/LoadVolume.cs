using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;  
using System;


public class LoadVolume : MonoBehaviour {
    public List<List<Vector4>> iListList = new List<List<Vector4>>();
	
    //public string datasetname = "Fdata.csv";
    public string datasetfolder = "Datasets";
    // Use this for initialization
    private IEnumerator NonBlockingWait(LoadDatasetsJob j)
    {
        yield return StartCoroutine(j.WaitFor());
    }
    void Start () {
        string datasetFolderPath = Application.dataPath + "/Resources/" + "/" + datasetfolder + "/";
        var info = new DirectoryInfo(datasetFolderPath);
        var fileInfo = info.GetFiles();
        List<LoadDatasetsJob> joblikst = new List<LoadDatasetsJob>();
        foreach (FileInfo file in fileInfo)
        {
            //Do the loading in a separate thread
            LoadDatasetsJob myJob = new LoadDatasetsJob();
            myJob.iListList = iListList;
            myJob.file = file;
            joblikst.Add(myJob);
            myJob.Start();
        }
        foreach (LoadDatasetsJob j in joblikst)
        {
            while (!j.Update()) ;
        }

    }

    // Update is called once per frame
    void Update () {
	
	}
}
