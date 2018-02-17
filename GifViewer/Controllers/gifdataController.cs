using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;//for get server path
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GifViewer.Models;
using System.Net.Http.Headers;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GifViewer.Controllers
{
	//************************************************************************
	//Name: 	gifdataController
	//Author: 	Zheng XM (2017/12/9)
	//Modify:
	//Return:
	//Description: recieve data from client, and then return data to the client
	//				All the data types are define in Models/gifdata.cs
	//			1.support web commands GET,POST,PUT,DELETE, but PUT and DELETE are not uead frequncly.
	//
	[Route("api/[controller]")]
	//[Produces("application/json")]
	//[Consumes("application/json","application/json-patch+json","multipart/form-data")]
    public class gifdataController : Controller
    {
		#region Vars
		private IHostingEnvironment m_oEnveroment;//for get server path
		#endregion Vars

		#region Constructors
		public gifdataController(IHostingEnvironment oEnveroment) //for get server path
		{
			this.m_oEnveroment = oEnveroment;
		}
		#endregion Constructors

		//************************************************************************
		//Name: 	Get
		//Author: 	Zheng XM (2017/12/9)
		//Modify:
		//Return:
		//Description: use [root url]/api/gefdata or $.ajax() get data from server
		//			this function is only for testing purpose.
		//			 
		[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "you", "got your server" };
        }

		//************************************************************************
		//Name: 	Get
		//Author: 	Zheng XM (2017/12/9)
		//Modify:
		//Return:
		//Description: use [root url]/api/gefdata/id or $.ajax() get data from server
		//			this function is only for testing purpose.
		//			 
		[HttpGet("{id}")]
        public string Get(int id)
        {
            return $"you got your data by using an id({id})";
        }

		//************************************************************************
		//Name: 	GetUIStrings
		//Author: 	Zheng XM (201712/10)
		//Modify:
		//Modify:
		//Return:  	async Task<ActionResult>
		//Description: get UI strings for specified UI language
		//			1.follow those styeps in Client to call this WEB API
		//				var settings = {
		//					"async": false,
		//					"crossDomain": true,
		//					"url": '/api/gifdata/GetUIStrings/',
		//					"method": "POST",
		//					"headers": {
		//						"content-type": "application/json",
		//						"cache-control": "no-cache",
		//					},
		//					"data": jsondata,
		//					"success": function(data, status, xhr)
		//					{
		//						//got your data
		//					},
		//					"error": function(jqXHR, textStatus, errorThrown)
		//					{
		//						//error
		//					},
		//				};
		//				$.ajax(settings);
		//
		[HttpPost("GetUIStrings")]
		public async Task<ActionResult> GetUIStrings([FromBody]GetUIStringsParam value)
		{
			ActionResult oRcd = new EmptyResult();
			if ((ModelState.IsValid) && (value != null))
			{//got right param from client
				oRcd = await Task.Run(() =>
				{//use async to make return data
					ActionResult oJson = new EmptyResult();
					string strFontFile = System.IO.Path.Combine(this.m_oEnveroment.WebRootPath, @"App_Data/UIStrings.json");
					try
					{
						using (StreamReader file = System.IO.File.OpenText(strFontFile))
						{
							string strData = string.Empty;
							JsonSerializer oSerializer = new JsonSerializer();
							Object[] oUIStrings = (Object[])oSerializer.Deserialize(file, typeof(Object[]));
							if ((oUIStrings != null) && (oUIStrings.Length > 0))
							{
								Object oStrings = oUIStrings[0];//for defautl to return Enghlish strings
								if ((value.LangguageIndex > 0) &&
								(value.LangguageIndex < oUIStrings.Length))
								{
									oStrings = oUIStrings[value.LangguageIndex];//return certain language string
								}
								strData = JsonConvert.SerializeObject(oStrings);
							}
							ResponseParam oResponseParam = new ResponseParam
							{//return all the strings for UI
								Type = DataType.Json,
								Data = strData,
								CallFiFoGUID = -1
							};
							oJson = Json(oResponseParam);
						}
					}
					catch (Exception e)
					{//一般是没有找到ColorGroups.json文件等问题

					}
					return oJson;
				});
			}
			return oRcd;
		}

		//************************************************************************
		//Name: 	GetNextData
		//Author: 	Zheng XM (201712/13)
		//Modify:
		//Modify:
		//Return:  	async Task<ActionResult>
		//Description: get next gif/tiff data
		//			1.follow those steps in Client to call this WEB API
		//				var settings = {
		//					"async": true,
		//					"crossDomain": true,
		//					"url": '/api/gifdata/GetNextData/',
		//					"method": "POST",
		//					"headers": {
		//						"content-type": "application/json",
		//						"cache-control": "no-cache",
		//					},
		//					"data": jsondata,
		//					"success": function(data, status, xhr)
		//					{
		//						//got your data
		//					},
		//					"error": function(jqXHR, textStatus, errorThrown)
		//					{
		//						//error
		//					},
		//				};
		//				$.ajax(settings);
		//
		[HttpPost("GetNextData")]
		public async Task<ActionResult> GetNextData([FromBody]GetNextDataParam value)
		{
			ActionResult oRcd = new EmptyResult();
			if ((ModelState.IsValid) && (value != null))
			{//got right param from client
				oRcd = await Task.Run(() =>
				{//use async to make return data
					ActionResult oJson = new EmptyResult();
                    string strImgDir = System.IO.Path.Combine(this.m_oEnveroment.WebRootPath, @"App_Data/ImageData");
					try
					{
						if(Directory.Exists(strImgDir))
						{
							//set file name filter
							string strFilter = "*";
							if (!string.IsNullOrEmpty(value.Filter))
							{
								strFilter = $"*{value.Filter}*";
							}
							//get all gif, tif, tiff files and sort it by name
							DirectoryInfo oDirInfo = new DirectoryInfo(strImgDir);
							FileInfo[] oFileInfoTemp = oDirInfo.GetFiles($"{strFilter}.gif");
							FileInfo[] oFileInfos = oFileInfoTemp.Concat(oDirInfo.GetFiles($"{strFilter}.tif?")).OrderBy(p=>p.Name).ToArray();
							if (oFileInfos.Length > 0)
							{
								//find next file to be return. 
								int nNext = 0;//if current name is empty then start from first file
								if (!string.IsNullOrEmpty(value.CurrentFileName))
								{
									for (int i = 0; i < oFileInfos.Length; i++)
									{
										if (value.CurrentFileName.Equals(oFileInfos[i].Name, StringComparison.OrdinalIgnoreCase))
										{//find current file
											nNext = i + 1;//next file number
											break;
										}
									}
								}
								if (nNext >= oFileInfos.Length)
								{
									nNext = 0;
								}

								byte[] oData = null;
								DataType eType = DataType.PngBase64;
								if (value.Animation)
								{//need animation, so retrun gif data
									eType = oFileInfos[nNext].Extension.IndexOf("gif", StringComparison.OrdinalIgnoreCase) >= 0 ? DataType.GifBase64 : DataType.TiffBase64;
									oData = System.IO.File.ReadAllBytes(oFileInfos[nNext].FullName);
								}
								else
								{//none animtion, so return png data
									Image oGifImage = Image.FromFile(oFileInfos[nNext].FullName);
									int nFrames = oGifImage.GetFrameCount(FrameDimension.Time);
									if (nFrames > 0)
									{//at least 1 frame
										Random oRandom = new Random();
										int nSelectedFrame = oRandom.Next(0, nFrames - 1);
										oGifImage.SelectActiveFrame(FrameDimension.Time, nSelectedFrame);
										using (System.IO.MemoryStream omemoString = new System.IO.MemoryStream()) {
											oGifImage.Save(omemoString, ImageFormat.Png);
											oData = omemoString.ToArray();
										}
									}
								}
								string strData = Convert.ToBase64String(oData);
								DataInfo oDataInfo = new DataInfo
								{
									Type = eType,
									Data = strData,
									Total = oFileInfos.Length,
									Index = nNext+1,
									FileName = oFileInfos[nNext].Name
								};
								ResponseParam oResponseParam = new ResponseParam
								{//return all the strings for UI
									Type = DataType.Json,
									Data = JsonConvert.SerializeObject(oDataInfo),
									CallFiFoGUID = value.CallFiFoGUID,
								};
								oJson = Json(oResponseParam);
							}
						}
					}
					catch (Exception e)
					{//file operation error...

					}
					return oJson;
				});
			}
			return oRcd;
		}

		//************************************************************************
		//Name: 	Upload
		//Author: 	Zheng XM (201712/20)
		//Modify:
		//Modify:
		//Return:  	async Task<ActionResult>
		//Description: get next gif/tiff data
		//			1.follow those steps in Client to call this WEB API
		//				var settings = {
		//					"async": true,
		//					"crossDomain": true,
		//					"url": '/api/gifdata/Upload/',
		//					"method": "POST",
		//					"contentType": false,
		//					"processData": false,
		//					"data": jsondata,
		//					"success": function(data, status, xhr)
		//					{
		//						//got your data
		//					},
		//					"error": function(jqXHR, textStatus, errorThrown)
		//					{
		//						//error
		//					},
		//				};
		//				$.ajax(settings);
		//
		[HttpPost("Upload")]
		public async Task<ActionResult> Upload(ICollection<IFormFile> value)//want to get the data form ajax, but does not work
		{
			ActionResult oRcd = await Task.Run(() =>
			{//use async to make return data
				ActionResult oJson = new EmptyResult();
				//file directory to save all the uploaded file
				string strImgDir = System.IO.Path.Combine(this.m_oEnveroment.WebRootPath, @"App_Data/ImageData");
				try
				{
					if (Directory.Exists(strImgDir))
					{
						LinkedList<string> oUnuploadedFiles = new LinkedList<string>();//record unuploaded file(s)
						var oFiles = Request.Form.Files;
						//long nTotalBytes = oFiles.Sum(f => f.Length);//total data bytes to be uploaded
						//long nSize = 0;//uploaded data bytes
						foreach (var oFile in oFiles)
						{
							//var filename = ContentDispositionHeaderValue
							//				.Parse(file.ContentDisposition)
							//				.FileName
							//				.Trim('"');
							oUnuploadedFiles.AddLast(oFile.Name);//assume will not be uploaded
							string strExt = Path.GetExtension(oFile.Name);
							if (!string.IsNullOrEmpty(strExt))
							{
								if (strExt.Equals(".gif", StringComparison.OrdinalIgnoreCase))
								{
									string strFilename = strImgDir + $@"\{oFile.Name}";
									if (!System.IO.File.Exists(strFilename))
									{//start upload
										//nSize += oFile.Length;
										using (FileStream fs = System.IO.File.Create(strFilename))
										{
											oFile.CopyTo(fs);
											fs.Flush();
										}
										//remove from unuploaded file list
										oUnuploadedFiles.RemoveLast();
									}
									//else
									//{//alread has this file
									//	oUnuploadedFiles.AddLast(oFile.Name);
									//}
								}
								//else
								//{// not gif file
								//	oUnuploadedFiles.AddLast(oFile.Name);
								//}
							}
							//else
							//{//not gif file
							//	oUnuploadedFiles.AddLast(oFile.Name);
							//}
						}
						string strMessage = string.Empty;
						foreach (string strTemp in oUnuploadedFiles)
						{
							strMessage += strTemp + ",";
						}
						if (!string.IsNullOrEmpty(strMessage))
						{//found unuploaded files
							strMessage.Substring(0,strMessage.Length-1);
						}
						oJson = Json(strMessage);
					}
				}
				catch (Exception e)
				{//file operation error...
					oJson = Json(e.Message);
				}
				return oJson;
			});
			return oRcd;
		}

		// POST api/values
		//[HttpPost]
		//public void Post([FromBody]string value)
		//{
		//}

		//do not use put
		// PUT api/values/5 
		//[HttpPut("{id}")]
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//we do not use delete
		// DELETE api/values/5
		//[HttpDelete("{id}")]
		//public void Delete(int id)
		//{
		//}
	}
}
