using UnityEngine;
using Vuforia;
using System.Collections;
using System.Collections.Generic;

public class Button1Handler : MonoBehaviour, IVirtualButtonEventHandler
{
	private GameObject _modelSamurai;
	private GameObject _modelPlane;
	private GameObject _modelCurrent;
	private GameObject _samuraiText;
	private GameObject _planeText;
	public Material button;
	private bool rotateFlag;
	private List<int> Counter = new List<int>();
	private int index;

	public static Vector3 initialScale;
	public static float factor;
	// Use this for initialization
	void Start () {
		// Search for all Children from this ImageTarget with type VirtualButtonBehaviour
		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
		for (int i = 0; i < vbs.Length; ++i) {
			// Register with the virtual buttons TrackableBehaviour
			vbs[i].RegisterEventHandler(this);
		}
		_modelSamurai = transform.FindChild("Samurai").gameObject;
		_modelPlane = transform.FindChild("Plane").gameObject;
		_samuraiText = transform.FindChild ("CharacterInfo").FindChild ("samuraiText").gameObject;
		_planeText = transform.FindChild ("CharacterInfo").FindChild ("planeText").gameObject;
		initialScale = _modelPlane.transform.localScale;
		factor = initialScale.x / 20 * 5;
		_modelPlane.SetActive(false);
		_planeText.SetActive(false);

		_modelCurrent = _modelSamurai;
		index = 0;
		for (int i=0; i<2; ++i) {
			Counter.Add(0);
//			outCounter.Add(0);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (rotateFlag == true)
			_modelCurrent.transform.Rotate (0, 60 * Time.deltaTime, 0);

	}

	/// <summary>
	/// Called when the virtual button has just been pressed:
	/// </summary>
	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb) {

		switch(vb.VirtualButtonName) {
		case "btnChange":
			if(_modelSamurai.activeSelf==true){
				_modelSamurai.SetActive(false);
				_samuraiText.SetActive(false);
				_modelPlane.SetActive(true);
				_planeText.SetActive(true);
				index = 1;
				_modelCurrent=_modelPlane;
			}
			else{
				_modelSamurai.SetActive(true);
				_samuraiText.SetActive(true);
				_modelPlane.SetActive(false);
				_planeText.SetActive(false);
				index = 0;
				_modelCurrent=_modelSamurai;
			}

			break;
		case "btnZoomIn":
			if(Counter[index]<5){
				_modelCurrent.transform.localScale = new Vector3 (_modelCurrent.transform.localScale.x + factor, _modelCurrent.transform.localScale.y + factor, _modelCurrent.transform.localScale.z + factor);
				Counter[index] ++;
			}
			break;
		case "btnZoomOut":
			if(Counter[index]>-5){
				_modelCurrent.transform.localScale = new Vector3 (_modelCurrent.transform.localScale.x - factor, _modelCurrent.transform.localScale.y - factor, _modelCurrent.transform.localScale.z - factor);
				Counter[index] --;
			}
			break;
		case "btnPlay":
			if(_modelSamurai.activeSelf==true){ 
				var anim = _modelSamurai.GetComponent<Animation>();
				if(anim.IsPlaying("Attack"))
					anim.Play("idle");
				else
					anim.Play("Attack");
			}
			break;
		case "btnRotate":
			rotateFlag = rotateFlag==true ? false : true; 
			break;
		default:
			throw new UnityException("Button not supported: " + vb.VirtualButtonName);
			break;
		}
	}
	
	/// <summary>
	/// Called when the virtual button has just been released:
	/// </summary>
	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb) { }
}

