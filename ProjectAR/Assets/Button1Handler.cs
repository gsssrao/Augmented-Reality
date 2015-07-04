using UnityEngine;
using Vuforia;
using System.Collections;

public class Button1Handler : MonoBehaviour, IVirtualButtonEventHandler
{
	private GameObject _modelSamurai;
	private GameObject _modelPlane;
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
		initialScale = _modelPlane.transform.localScale;
		factor = initialScale.x / 20 * 5;
		_modelSamurai.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Called when the virtual button has just been pressed:
	/// </summary>
	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb) {
		switch(vb.VirtualButtonName) {
		case "btnPlane":
//			_modelSamurai.SetActive(false);
//			_modelPlane.SetActive(true);
			_modelPlane.transform.localScale = new Vector3 (_modelPlane.transform.localScale.x - factor, _modelPlane.transform.localScale.y - factor, _modelPlane.transform.localScale.z - factor);
			break;
		case "btnSamurai":
//			_modelSamurai.SetActive(true);
//			_modelPlane.SetActive(false);
			_modelPlane.transform.localScale = new Vector3 (_modelPlane.transform.localScale.x + factor, _modelPlane.transform.localScale.y + factor, _modelPlane.transform.localScale.z + factor);
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

