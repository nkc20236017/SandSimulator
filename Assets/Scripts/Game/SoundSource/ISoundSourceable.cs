using UnityEngine;

public interface ISoundSourceable
{
	/// <summary>
	/// 音の発生源の生成時間を設定
	/// </summary>
	/// <param name="id">音の発生源のID</param>
	/// <param name="time">生成間隔の時間</param>
	public void SetInstantiation(string id, float time = 1f);
	
	/// <summary>
	/// 音の発生源を生成
	/// </summary>
	/// <param name="id">音の発生源のID</param>
	/// <param name="position">発生源の場所</param>
	/// <param name="despawnTime">消滅時間</param>
	public void InstantiateSound(string id, Vector3 position, float despawnTime = 3f);
}
