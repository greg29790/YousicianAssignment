using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Description : MonoBehaviour {
	private YleResult res;

	public GameObject	SearchView;
	// Description View Objects
	public GameObject	DescriptionView;
	public Text			DescriptionViewTitle;
	public Image 		DescriptionViewImage;
	public Text 		DescriptionStartTime;
	public Text 		DescriptionViewDuration;
	public Text 		DescriptionViewDescription;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Return to SearchView
		if (Input.GetKey (KeyCode.Escape) && DescriptionView.activeSelf) {
			DescriptionView.SetActive (false);
			SearchView.SetActive (true);
		}
	}

	/// <summary>
	/// Active Description View for the selected program
	/// </summary>
	public void GoToDescription(){
		string _dataId = EventSystem.current.currentSelectedGameObject.name;
		string request = YleConfig.API_REQUEST + "&id=" + _dataId;

		StartCoroutine (GetProgramWithId (request));
	}

	/// <summary>
	/// Send request to get program with its id
	///	</summary>
	/// <param name="request">request to get program with its id</param>
	/// <returns></returns>
	private IEnumerator GetProgramWithId(string request)
	{
		using (UnityWebRequest www = UnityWebRequest.Get (request)) {
			yield return www.SendWebRequest ();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				res = JsonUtility.FromJson<YleResult> (www.downloadHandler.text);
				YleData _data = res.data [0];
				SearchView.SetActive (false);
				DescriptionView.SetActive (true);
				//Set Title
				DescriptionViewTitle.text = _data.title.fi;

				// Set Start Time
				if (_data.publicationEvent != null && _data.publicationEvent.Count > 0) {
					string dateString = _data.publicationEvent[0].startTime;
					DateTime date = Convert.ToDateTime(dateString);
					DescriptionStartTime.text = string.Format("{0:dd-MM-yyyy}", date);	
				} else {
					DescriptionStartTime.text = "Not Available";
				}

				// Set Duration
				if (!string.IsNullOrEmpty(_data.duration))
				{
					TimeSpan ts = XmlConvert.ToTimeSpan (_data.duration);
					DescriptionViewDuration.text = "Duration : " + string.Format ("{0:0} h {1:0} min", ts.Hours, ts.Minutes);
				} else {
					DescriptionViewDuration.text = "Duration : not available" ;
				}

				// Set Description
				DescriptionViewDescription.text = _data.description.fi;

				// Set Image
				if (string.IsNullOrEmpty (_data.image.id)) {
					DescriptionViewImage.gameObject.SetActive (false);
				} else {
					DescriptionViewImage.gameObject.SetActive (true);
					string _imgRequest = YleConfig.URL_IMAGE + "w_" + YleConfig.WIDTH + ",h_" + YleConfig.HEIGHT + "," + YleConfig.SCALE + "/" + _data.image.id + "." + YleConfig.FORMAT;
					StartCoroutine (LoadItemImage (_imgRequest, DescriptionViewImage));
				}
			}
		}
	}

	/// <summary>
	/// Send request to get program's image
	/// </summary>
	/// <param name="request">request to get image with its id</param>
	/// <param name="img">Item image</param>
	/// <returns></returns>
	private IEnumerator LoadItemImage(string request, Image img){
		using (UnityWebRequest www = UnityWebRequestTexture.GetTexture (request)) {
			yield return www.SendWebRequest ();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
				img.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
			}
		}
	}
}
