﻿using UnityEngine;
using System.Collections;

public class HeaderColor : MonoBehaviour {
	static public Color pageColor = new Color32(255,222,23,255);
	public GameObject[] ColorObject;

	// Use this for initialization
	void Start () {
		foreach (GameObject obj in ColorObject) {
			if (obj != null)
			{
				SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
				renderer.color = pageColor;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}