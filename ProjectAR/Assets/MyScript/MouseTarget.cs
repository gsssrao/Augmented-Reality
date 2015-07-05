using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseTarget : MonoBehaviour {

	private static MouseTarget mInstance;
	public static MouseTarget instance {get {return mInstance;}}

	public SpriteRenderer targetSprite;

	// Use this for initialization
	void Start () {
		if (mInstance == null) {
			mInstance = this;
		}
	
	}
	
	public void targetChosen(bool choosen){
		if (choosen) {
			targetSprite.color = Color.white;
		} else {
			targetSprite.color = Color.red;
		}
	}
}
