using UnityEngine;
using System.Collections;

public class ScrollingUVs_Layers : MonoBehaviour 
{
	//public int materialIndex = 0;
	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	public string textureName = "_MainTex";
    public float textureSlowdownSpeed =1;
	
	Vector2 uvOffset = Vector2.zero;
	
	void LateUpdate() 
	{
		uvOffset += ( uvAnimationRate * Time.deltaTime / textureSlowdownSpeed);
		if( GetComponent<Renderer>().enabled )
		{
			GetComponent<Renderer>().sharedMaterial.SetTextureOffset( textureName, uvOffset );
		}
	}
}