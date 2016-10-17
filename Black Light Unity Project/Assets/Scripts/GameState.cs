using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class GameState {


    //Save file name
    private const string SAVE_FILE_NAME = "/save.s";

    //url
    private string save_url = Application.persistentDataPath + SAVE_FILE_NAME;


    /// <summary>
    /// Turns the actual game state into an arraylist following a certain format.
    /// </summary>
    /// <returns>ArrayList containing the game state following a certain format</returns>
    private ArrayList GameStateIntoArrayList()
    {
        return null;
    }

    /// <summary>
    /// Extracts information from the ArrayList issued as a parameter and saves it as the game state.
    /// </summary>
    /// <param name="arrayList">ArrayList containing the information to be stored as the game state.
    ///                         It must follow the format defined in the documentation for GameStateIntoArrayList()</param>
    private void ArrayListIntoGameState(ArrayList arrayList)
    {
        
    }

    /// <summary>
    /// Saves the actual game state into a file in the path save_url with the name SAVE_FILE_NAME
    /// </summary>
    private void Save()
    {
        FileStream serializationStream = File.Create(save_url);
        BinaryFormatter bf = new BinaryFormatter();

        ArrayList gameState_arr = GameStateIntoArrayList();
        bf.Serialize(serializationStream, gameState_arr);
        serializationStream.Close();
    }

    /// <summary>
    /// Loads the game state from the file in the path save_url and the name SAVE_FILE_NAME
    /// </summary>
    private void Load()
    {
        if (File.Exists(save_url))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream deserializationStream = File.Open(save_url, FileMode.Open);
            ArrayList gameState_arr = (ArrayList)bf.Deserialize(deserializationStream);
            ArrayListIntoGameState(gameState_arr);
        }
    }
}
