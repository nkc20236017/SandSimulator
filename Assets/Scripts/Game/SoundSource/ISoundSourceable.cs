using UnityEngine;

public interface ISoundSourceable
{
	/// <summary>
	/// 音の発生源を生成
	/// </summary>
	/// <param name="position">発生源の場所</param>
	/// <param name="despawnTime">消滅時間</param>
	public void InstantiateSound(Vector3 position, float despawnTime = 3f);
}
