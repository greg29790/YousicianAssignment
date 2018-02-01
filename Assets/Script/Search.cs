using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System;
using System.Xml;

public class Search : MonoBehaviour {


	private YleResult res;
	private int offset = 0;
	public int Limit = 10;

	// Search View Objects
	public GameObject	SearchView;
	public InputField 	SearchViewInput;
	public GameObject 	SearchViewItem;
	public VerticalLayoutGroup SearchViewContainer;
	public ScrollRect SearchViewScrollView;
	public GameObject SearchViewLoading;
	public GameObject SearchViewError;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Start new search
	/// Use for Search Button
	/// </summary>
	public void StartSearch()
	{
		ShowLoading (true);
		offset = 0;
		ClearItems ();
		SearchPrograms ();
	}

	/// <summary>
	/// Triggered when user scroll the middle mouse or dragging  the vertical scrollbar
	/// </summary>
	public void onListScroll()
	{
		// check if user scroll near the bottom
		if (SearchViewScrollView.verticalNormalizedPosition <= 0.01 && !SearchViewLoading.activeSelf)
		{
			ShowLoading (true);
			SearchPrograms();
		}
	}

	/// <summary>
	/// Show Loading during network search
	/// </summary>
	/// <param name="show"></param>
	private void ShowLoading(bool show){
		if (show) {
			SearchViewLoading.transform.SetSiblingIndex (SearchViewContainer.transform.childCount - 1);
		}
		SearchViewLoading.SetActive (show);
	}
	
	/// <summary>
	/// Search programs based on existing keyword, limit and offset.		
	/// </summary> 
	private void SearchPrograms(){
		string keyword = SearchViewInput.text;
		string search = YleConfig.API_REQUEST;

		// set keywords
		if (!string.IsNullOrEmpty(keyword))
		{
			search += "&q=" + keyword;
		}
		// Set data limit
		if (Limit > 0)
		{
			search += "&limit=" + Limit;
		}
		// Set offset
		if (offset > 0) {
			search += "&offset=" + offset;
		}
		// send request
		StartCoroutine(GetPrograms(search));
	}
	
	/// <summary>
	/// Send request to get list of programs
	/// </summary>
	private IEnumerator GetPrograms(string request){
		using (UnityWebRequest www = UnityWebRequest.Get (request)) {
			yield return www.SendWebRequest ();

			ShowLoading (false);
			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
				SearchViewError.SetActive(true);
			} else {
				SearchViewError.SetActive(false);
				res = JsonUtility.FromJson<YleResult>(www.downloadHandler.text);

				foreach (var data in res.data)
				{
					// extract program ID
					string id = data.id ;
					// extract program title
					string title = data.title.fi ?? data.title.se ?? data.title.sv ?? data.title.und;
					//exctract image id
					string imgId = data.image.id ?? data.partOfSeries.image.id;
					// add list item to UI
					AddProgramToList(id, imgId, title);
				}
				offset += res.data.Count;
				Debug.Log(offset);
			}
		}
	}

	/// <summary>
	/// Add program to the list in SearchView
	/// </summary>
	/// <param name="id">program id</param>
	/// <param name="imgId">program image id</param>
	/// <param name="title">program title</param>
	private void AddProgramToList(string id, string imgId, string title)
	{
		GameObject newItem = Instantiate(SearchViewItem) as GameObject;
		newItem.name = id;

		// Set Program title on item
		Text _title = newItem.transform.Find("Title").GetComponent<Text>();
		_title.text = title;
	
		// Set Program image on item
		Image _img = newItem.transform.Find ("Image").GetComponent<Image> ();
		if (string.IsNullOrEmpty (imgId)) {
			_img.gameObject.SetActive (false);
		} else {
			_img.gameObject.SetActive (true);
			string _imgRequest = YleConfig.URL_IMAGE + "w_" + YleConfig.WIDTH + ",h_" + YleConfig.HEIGHT + "," + YleConfig.SCALE + "/" + imgId + "." + YleConfig.FORMAT;
			StartCoroutine (LoadItemImage (_imgRequest, _img));
		}
		newItem.SetActive(true);
		newItem.transform.SetParent(SearchViewContainer.transform, false);
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

	/// <summary>
	/// Clear items list
	/// </summary>
	private void ClearItems()
	{
		foreach (Transform child in SearchViewContainer.transform) {
			// Don't destroy Loading and Error GameObject
			if (child.name == SearchViewLoading.name || child.name == SearchViewError.name)
				continue;
			Destroy (child.gameObject);
		}
	}
}
