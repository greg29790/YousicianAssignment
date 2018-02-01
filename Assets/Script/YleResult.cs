using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// json response from server
/// </summary>
[System.Serializable]
public struct YleResult
{
	public string apiVersion;
	public List<YleData> data;

}

/// <summary>
/// Contains program data
/// </summary>
[System.Serializable]
public struct YleData
{
	public string id;
	public string duration;
	public YleDataTitle title;
	public YleDataDescription description;
	public List<YleDataPublicationEvent> publicationEvent;
	public YleDataPartOfSeries partOfSeries;
	public YleDataImage image;

}

/// <summary>
/// Represent title data
/// </summary>
[System.Serializable]
public struct YleDataTitle
{
	public string fi;
	public string sv;
	public string und;
	public string se;
}
	
/// <summary>
/// Represent description data
/// </summary>
[System.Serializable]
public struct YleDataDescription
{
	public string fi;
	public string sv;
	public string se;
}

/// <summary>
/// Represent publication event data
/// </summary>
[System.Serializable]
public struct YleDataPublicationEvent
{
	public string startTime;
}
	
/// <summary>
/// Represent part of series data
/// </summary>
[System.Serializable]
public struct YleDataPartOfSeries
{
	public YleDataImage image;
}

/// <summary>
/// Represent image data
/// </summary>
[System.Serializable]
public struct YleDataImage
{
	public string id;
}


