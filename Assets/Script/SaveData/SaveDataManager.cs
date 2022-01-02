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
		public int shootModeStageNum;
	}

	SaveData saveDate = new SaveData();
	public static SaveDataManager instance = null;

	[System.NonSerialized]
	public int puzzelModeStageNum = 0; //パズルモードクリアステージ番号
	[System.NonSerialized]
	public int shootModeStageNum = 0;  //シュートモードクリアステージ番号

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

	//========================================================================
	//パズルモードセーブデータ更新
	//========================================================================
	//ClearStageNum; クリアステージ番号
	//========================================================================
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

	//========================================================================
	//パズルモードセーブデータ読み込み
	//========================================================================
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

	//========================================================================
	//シュートモードセーブデータ更新
	//========================================================================
	//ClearStageNum; クリアステージ番号
	//========================================================================
	public void ShootModeSaveData(int ClearStageNum)
	{
		if(saveDate.puzzelModeStageNum < ClearStageNum)
		{
			saveDate.puzzelModeStageNum = ClearStageNum;

			StreamWriter writer;
			string jsonstr = JsonUtility.ToJson(saveDate);

			writer = new StreamWriter(Application.persistentDataPath + "/ShootModeSaveData.json", false);
			writer.Write(jsonstr);
			writer.Flush();
			writer.Close();
		}
	}

	//========================================================================
	//シュートモードセーブデータ読み込み
	//========================================================================
	public void ShootModeLoadData()
	{
		if (File.Exists(Application.persistentDataPath + "/ShootModeSaveData.json"))
		{
			string datastr = "";
			StreamReader reader;
			reader = new StreamReader(Application.persistentDataPath + "/ShootModeSaveData.json");
			datastr = reader.ReadToEnd();
			reader.Close();
			saveDate = JsonUtility.FromJson<SaveData>(datastr);

			puzzelModeStageNum = saveDate.puzzelModeStageNum;
		}
	}

	void DataReset()
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
