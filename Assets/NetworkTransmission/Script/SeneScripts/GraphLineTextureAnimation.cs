using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphLineTextureAnimation : VisibilityDependantComponent
{

	public float changeInterval = 0.05F;
	public float Speed = 5;

    void Start()
    {
        Speed = Random.Range(0.5f, 2);
    }

	void Update() {
		LineRenderer rend = GetComponent<LineRenderer>();

		float index = Time.time * Speed;

		Vector2 textureShift = new Vector2 (-index, 0);
		rend.material.mainTextureScale = new Vector2 ((rend.GetPosition(0) - rend.GetPosition(rend.positionCount - 1)).magnitude, 0.5f);
		rend.material.mainTextureOffset = textureShift;
	}
}