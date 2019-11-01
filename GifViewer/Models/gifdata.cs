using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace GifViewer.Models
{
	//************************************************************************
	//Name: 	DataType
	//Author: 	Zheng XM (2017/12/10)
	//Modify: 	
	//Return:  	
	//Description: data types
	//
	public enum DataType
	{
		Non = 0,
		PngBase64,
		JpgBase64,
		GifBase64,
		TiffBase64,
		SvgBase64,
		Text,
		Json,
	}

	//************************************************************************
	//Name: 	GetUIStringsParam
	//Author: 	Zheng XM (2017/12/10)
	//Modify: 	
	//Return:  	
	//Description: use this param to get UI strings
	//
	public class GetUIStringsParam
	{
		//language index
		public int LangguageIndex { get; set; } = 0;//index of supportLanguages[] in GifView.js,
	}

	//************************************************************************
	//Name: 	GetNextDataParam
	//Author: 	Zheng XM (2017/12/13)
	//Modify: 	
	//Return:  	
	//Description: use this param to get gif/data
	//
	public class GetNextDataParam
	{
		//calling GIUD
		public int CallFiFoGUID { get; set; } = -1;
		//language indexindex of supportLanguages[] in GifView.js
		public int LangguageIndex { get; set; } = 0;
		//filter string
		public string Filter { get; set; }=string.Empty;
		//need gif, if this is false return a png (none animation image)
		public bool Animation { get; set; } = true;
		//Current file name
		public string CurrentFileName { get; set; } = string.Empty;
	}

	//************************************************************************
	//Name: 	GetNextDataParam
	//Author: 	Zheng XM (2017/12/13)
	//Modify: 	
	//Return:  	
	//Description: use this param to get gif/data
	//
	//public class UploadFileParam
	//{
	//	//language indexindex of supportLanguages[] in GifView.js
	//	public int LangguageIndex { get; set; } = 0;
	//	//FormFile (s)
	//	public IList<IFormFile> Files { get; set; } = null;
	//}

	//************************************************************************
	//Name: 	DataInfo
	//Author: 	Zheng XM (2017/12/15)
	//Modify: 	
	//Return:  	
	//Description: gif/tiff data info
	//
	public class DataInfo
	{
		//data type for this.Data
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DataType Type { get; set; } = DataType.GifBase64;
		//gif/tiff data (base64)
		public string Data { get; set; } = string.Empty;
		//gif data file name (if have)
		public string FileName { get; set; } = string.Empty;
		//number of gif files to be shown
		public int Total { get; set; } = 0;
		//this file index
		public int Index { get; set; } = 0;
	}

	//************************************************************************
	//Name: 	ResponseParam
	//Author: 	Zheng XM (2017/12/11)
	//Modify: 	
	//Return:  	
	//Description: data type to return client
	//
	public class ResponseParam
	{
		#region inner class
		#endregion inner class

		#region Constructor
		//public ResponseParam()
		//{
		//}
		//public ResponseParam(DataType eType,string strData, int nCallFiFoGUID)
		//{
		//	Type = eType;
		//	Data = string.IsNullOrEmpty(strData) ? string.Empty:strData ;
		//	CallFiFoGUID = nCallFiFoGUID;
		//}
		#endregion Constructor


		//call sequenc ID, copy from XXXXXXXXParam, when async
		public int CallFiFoGUID { get; set; } = -1;
		//data type for this.Data
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DataType Type { get; set; } = DataType.Non;
		//data
		public string Data { get; set; } = string.Empty;
	}

}
