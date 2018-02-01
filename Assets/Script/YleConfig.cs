using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YleConfig : MonoBehaviour {

	private const string API_ID = "b20c66c7";
	private const string API_KEY = "a49957df7a35ca820e8e4f4fd123623d";
	public const string API_REQUEST = "https://external.api.yle.fi/v1/programs/items.json?app_id=" + API_ID + "&app_key=" + API_KEY;
	public const string URL_IMAGE = "http://images.cdn.yle.fi/image/upload/";
	public const string WIDTH = "150";
	public const string HEIGHT = "150";
	public const string SCALE = "c_fit";
	public const string FORMAT = "jpg";
}
