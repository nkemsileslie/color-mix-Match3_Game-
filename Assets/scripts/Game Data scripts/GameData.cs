using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData{
    public bool[] isActive;
    public int[] highScore;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
   public static GameData gameData;
   public  SaveData  saveData;
  
    //Start is called before the first frame update
    void Awake()
    {
      if(gameData == null){
         DontDestroyOnLoad(this.gameObject);
           gameData = this;
       }else{
          Destroy(this.gameObject);
       } 
      Load();
    }

    private void Start()
    {
        
    }

    public void Save()
    {
        //Create a binary formatter which can read binary files
       BinaryFormatter formatter = new BinaryFormatter();

        //Create a route from the program to the file
       FileStream file = File.Open(Application.persistentDataPath + "/Play.dat", FileMode.Create);

        //Create a copy of saveData in the file
       SaveData data = new SaveData();
       data = saveData;

        //Actually save the in the file
       formatter.Serialize(file, data);

        //Close the data stream
        file.Close();

       Debug.Log("Saved...");
    }

   public  void Load()
    {
        //Check if the save game file exists
        if(File.Exists(Application.persistentDataPath + "/Play.dat" ))
        {
            //Create a binary formatter 
          BinaryFormatter formatter = new BinaryFormatter();
          FileStream file = File.Open(Application.persistentDataPath + "/Play.dat", FileMode.Create);
          saveData = formatter.Deserialize(file) as SaveData;
          file.Close();
          Debug.Log("Loaded...");
        }
    }

    private void OnDisable()
    {
      Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
