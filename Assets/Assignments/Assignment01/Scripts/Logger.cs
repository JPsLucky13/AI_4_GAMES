using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour {

    public static Logger instance = null;
    StreamWriter writer;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        
    }

    public void InitializeWriter(string fileName)
    {
        if(writer == null)
            writer = new StreamWriter(fileName, true);
    }


    public void WriteToFile(string data)
    {
        writer.WriteLine(data);
    }

    public void WriteToFileKinematic(string data)
    {
        string fileName = Application.dataPath + "/" + "KBoidData.txt";

        StreamWriter writer = new StreamWriter(fileName, true);

        writer.WriteLine(data + "\n");
        writer.Close();
    }

    public void WriteToFileDynamic(string data)
    {
        string fileName = Application.dataPath + "/" + "DBoidData.txt";

        StreamWriter writer = new StreamWriter(fileName, true);

        writer.WriteLine(data + "\n");
        writer.Close();
    }

    private void OnDisable()
    {
        if(writer != null)
            writer.Close();
    }
    
}
