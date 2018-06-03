using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class DataEditor : MonoBehaviour {
	private string gameDataProjectFilePath = "/data/config.json";
	GameSettings gameSettings;

	private void LoadGameData() {
			string filePath = Application.dataPath + gameDataProjectFilePath;

			if (File.Exists (filePath)) {
					string dataAsJson = File.ReadAllText (filePath);
					gameSettings = JsonUtility.FromJson<GameSettings> (dataAsJson);
			} else {
				gameSettings = new GameSettings();
			}
	}

	private void SaveGameData() {
			string dataAsJson = JsonUtility.ToJson (gameSettings);

			string filePath = Application.dataPath + gameDataProjectFilePath;
			File.WriteAllText (filePath, dataAsJson);

	}
}
