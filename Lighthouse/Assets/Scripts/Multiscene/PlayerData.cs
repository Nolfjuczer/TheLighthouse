using UnityEngine;
using System.Collections;

[System.Serializable]
public sealed class PlayerData
{
	#region Variables

	public struct LevelInfo
	{
		public int levelIndex;
		public int maxCash;
	}

	public bool audio = true;
	public bool music = true;

	public const string playerDataKey = "PlayerData";

	#endregion Variables

	#region Methods

	private PlayerData(bool first = false)
	{
		if(first)
		{
			audio = true;
			music = true;
		}
		else
		{
			audio = true;
			music = true;
		}
	}

	public static void Save(PlayerData playerData)
	{
		string data = JsonUtility.ToJson(playerData);

		if (!string.IsNullOrEmpty(data))
		{
			PlayerPrefs.SetString(playerDataKey, data);
			PlayerPrefs.Save();
		}
	}

	public static PlayerData Load()
	{
		PlayerData resultPlayerData = null;

		if(PlayerPrefs.HasKey(playerDataKey))
        {
			string data = PlayerPrefs.GetString(playerDataKey);

			if(!string.IsNullOrEmpty(data))
			{
				resultPlayerData = JsonUtility.FromJson<PlayerData>(data);
				resultPlayerData.ValidatePlayerData();
			}
        }
		if(resultPlayerData == null)
		{
			resultPlayerData = new PlayerData(true);
			resultPlayerData.ValidatePlayerData();
		}

		return resultPlayerData;
	}

	public void ValidatePlayerData()
	{

	}

	#endregion Methods
}
