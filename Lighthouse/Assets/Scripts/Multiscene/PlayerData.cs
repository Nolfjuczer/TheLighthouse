using UnityEngine;
using System.Collections;

[System.Serializable]
public sealed class PlayerData
{
	#region Variables

	public const string playerDataKey = "PlayerData";

	#endregion Variables

	#region Methods

	private PlayerData(bool first = false)
	{
		if(first)
		{

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
