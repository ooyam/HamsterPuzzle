using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
	[System.Serializable]
	public class SaveData
	{
		public int puzzelModeStageNum;
	}

	SaveData saveDate = new SaveData();
	public static SaveDataManager instance = null;

	[System.NonSerialized]
	public int puzzelModeStageNum = 0; //パズルモードクリアステージ番号

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	public void PuzzleModeSaveData(int ClearStageNum)
	{
		if(saveDate.puzzelModeStageNum < ClearStageNum)
		{
			saveDate.puzzelModeStageNum = ClearStageNum;

			StreamWriter writer;
			string jsonstr = JsonUtility.ToJson(saveDate);

			writer = new StreamWriter(Application.persistentDataPath + "/PuzzleModeSaveData.json", false);
			writer.Write(jsonstr);
			writer.Flush();
			writer.Close();
		}
	}

	public void PuzzleModeLoadData()
	{
		if (File.Exists(Application.persistentDataPath + "/PuzzleModeSaveData.json"))
		{
			string datastr = "";
			StreamReader reader;
			reader = new StreamReader(Application.persistentDataPath + "/PuzzleModeSaveData.json");
			datastr = reader.ReadToEnd();
			reader.Close();
			saveDate = JsonUtility.FromJson<SaveData>(datastr);

			puzzelModeStageNum = saveDate.puzzelModeStageNum;
		}
	}

	public void DataReset()
	{
		saveDate.puzzelModeStageNum = 0;

		StreamWriter writer;
		string json = JsonUtility.ToJson(saveDate);
		writer = new StreamWriter(Application.persistentDataPath + "/PuzzleModeSaveData.json", false);
		writer.Write(json);
		writer.Flush();
		writer.Close();
	}
}
