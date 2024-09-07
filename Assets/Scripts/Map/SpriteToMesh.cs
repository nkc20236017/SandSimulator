using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SpriteToMesh : MonoBehaviour
{
	public Sprite spriteToConvert;
	public float depthZ = 0.1f;

	private void Start()
	{
		if (spriteToConvert == null)
		{
			Debug.LogError("Sprite not assigned!");
			return;
		}

		Mesh mesh = CreateMeshFromSprite(spriteToConvert);

		// メッシュをMeshFilterに適用
		GetComponent<MeshFilter>().mesh = mesh;

		// マテリアルを設定
		Material material = new Material(Shader.Find("Sprites/Default"));
		material.mainTexture = spriteToConvert.texture;
		GetComponent<MeshRenderer>().material = material;

		// MeshColliderを設定
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}

	private Mesh CreateMeshFromSprite(Sprite sprite)
	{
		Mesh mesh = new Mesh();

		Vector3[] vertices = new Vector3[sprite.vertices.Length];
		for (int i = 0; i < sprite.vertices.Length; i++)
		{
			Vector2 uv = sprite.vertices[i];
			vertices[i] = new Vector3(uv.x, uv.y, depthZ);
		}

		mesh.vertices = vertices;
		
		var triangles = new int[sprite.triangles.Length];
		for (var i = 0; i < sprite.triangles.Length; i++)
		{
			triangles[i] = sprite.triangles[i];
		}
		mesh.triangles = triangles;
		mesh.uv = sprite.uv;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}
}

