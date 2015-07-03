/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * Confidential and Proprietary – Qualcomm Connected Experiences, Inc. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;

/// <summary>
/// Splash manager.
/// Loads the splash page
/// </summary>
public class SplashManager : MonoBehaviour {

    #region PRIVATE_MEMBER_VARIABLES
	private Texture2D mSplashTexture;
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region UNITY_MONOBEHAVIOUR_METHODS
    void Start () {
        
		mSplashTexture = Resources.Load("SplashScreen/AndroidEyewear") as Texture2D;
        Screen.orientation = ScreenOrientation.Landscape;
        StartCoroutine(LoadAboutPageAfter(2));
    }
    
    void OnGUI()
    {
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), mSplashTexture, ScaleMode.ScaleAndCrop);
    }
    
    private IEnumerator LoadAboutPageAfter(float secs)
    {
        yield return new WaitForSeconds(secs);
		Application.LoadLevel("Vuforia-1-ImageTarget");
    }
    #endregion UNITY_MONOBEHAVIOUR_METHODS
}
