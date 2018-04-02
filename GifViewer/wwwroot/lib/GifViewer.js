//all the source code for gif viewer base on w3.css and jQuery
//2017/12/9 by TEI

//globla var
var uri = '/api/gifdata';//web api URL
var supportLanguages = ['en', 'zh', 'ja'];//supported UI languages,必须都是小写, https://www.w3schools.com/tags/ref_country_codes.asp
var nCurrentLanguage = 0;	//current UI Language index. default is 0. supportLanguages[nCurrentLanguage = 0]=en-us
var oUIStringDic = null;//UI strings for current UI language
var biOS = false;
var bMSIE = false;
var bCompositioning = false;	//is using IME
var resetImageFiFo = [];//a record contains the sequence of get next data
var strCurrentFileName = '';//current file name is showing
$.guid = 0;//an unique ID for call ID 

//************************************************************************
//Name: 	$(document).ready
//Author: 	TEI (2017/12/9)
//Modify: 	
//Return:  	
//Description: initailization
//
$(document).ready(function () {
	ShowHideOnProcessing(true);//show on processing
	Init();
	GetUIStrings();//get all the strings for UI in current UI language

	ResetUIString();//set UI string
	//handle IME composition
	$('#LetteringText').on('compositionstart', function (ev) {
		bCompositioning = true;
	});
	$('#LetteringText').on('compositionend', function (ev) {
		bCompositioning = false;
	});

	//stop show onprocessing
	ShowHideOnProcessing(false);
	//hide upload progress bar
	$("#UploadProgressbar").hide();
	//lesten upload file
	$('#UploadFiles').change(function (e) {
		UploadFiles(e);
	});
	//start show
	ShowNextData();
});

//************************************************************************
//Name: 	GetUIStringsDictionary
//Author: 	TEI (2017/12/11)
//Modify:
//Return:  	
//Description: get all the strings for UI in current UI language
//
function GetUIStrings() {
	$('#ShowErrorMessage').hide();//首先关闭所显示的Error
	var jsondata = getGetUIStringsParamJsonData();
	var settings = {
		"async": false,
		"crossDomain": true,
		"url": uri + "/GetUIStrings",//gifdataController.GetUIStrings() will be called
		"method": "POST",
		"headers": {
			"content-type": "application/json",//tell server that it will get json data
			"cache-control": "no-cache"
		},
		"data": jsondata,//use gifdataController.GetUIStrings([FromBody]GetUIStringsParam value) in gifdataController.cs
		"success": function (data, status, xhr) {//get contents from ResponseParam
			var contentType = xhr.getResponseHeader("content-type") || "";
			if (contentType.indexOf("json") >= 0) {//we need JSON data
				if ((data.type) && (data.type.indexOf('Json') >= 0) && //is Json data
					(data.data) && (data.data.length > 0)) {//has real data
					oUIStringDic = null;
					try {
						var oJsonObj = JSON.parse(data.data);
						if (oJsonObj) {
							oUIStringDic = oJsonObj;
						}
					} catch (e) {
						ShowErrorMessage('Data Error!', 'Bad UI strings JSON Data:' + e);
					}
				}
				else {
                    var strMessage = 'Can not translate UI.';
                    if(data.type) {
                        strMessage += ',DataType:' + data.type;
                        if(data.type.indexOf('Text')>=0){
                            strMessage += ',Message:' + data.data;
						}
                    }
					alert(strMessage);//show a message that means could not get uistrings
				}
			}
		},
		"error": function (jqXHR, textStatus, errorThrown) {//for http error
			//显示错误信息
			ShowErrorMessage('Server Error!', textStatus);
		},
	};

	$.ajax(settings);
}

//************************************************************************
//Name: 	ShowNextData
//Author: 	TEI (2017/12/13)
//Modify:
//Return:  	
//Description: show next gif/tiff data
//
function ShowNextData() {
	$('#ShowErrorMessage').hide();//首先关闭所显示的Error
	var nUID = $.guid++;
	resetImageFiFo.push(nUID);	//记录CallStack
	var jsondata = getGetNextDataParamJsonData(nUID);
	var settings = {
		"async": true,
		"crossDomain": true,
		"url": uri + "/GetNextData",//gifdataController.GetUIStrings() will be called
		"method": "POST",
		"headers": {
			"content-type": "application/json",//tell server that it will get json data
			"cache-control": "no-cache"
		},
		"data": jsondata,//use gifdataController.ShowNextData([FromBody]GetNextDataParam value) in gifdataController.cs
		"success": function (data, status, xhr) {//get contents from ResponseParam
			Imagedata.src = '';
			var nCurrentFileNo = 0;
			var nTotalFiles = 0;
			var contentType = xhr.getResponseHeader("content-type") || "";
			if (contentType.indexOf("json") >= 0) {//we need JSON data
				var nStackNo = -1;//if the ID(data.callFiFoGUID) do not apear in the call stack(resetImageFiFo), then do nothing.
				if (data.callFiFoGUID >= 0) {
					var nStackNo = resetImageFiFo.indexOf(data.callFiFoGUID);
				}
				var bGetGifFile = false;
				if ((data.type) && (nStackNo >= 0)) {
					resetImageFiFo.splice(0, nStackNo + 1);//clear the call ID before this ID.
					if (resetImageFiFo.length <= 0) {
						ShowHideOnProcessing(false);
					}
					if (data.type.indexOf('Json') >= 0) {
						var oJsonObj = JSON.parse(data.data);
						if (oJsonObj) {
							if (oJsonObj.Type.indexOf('GifBase64') >= 0) {//GifBase64
								Imagedata.src = "data:image/gif;base64," + oJsonObj.Data;
							}
							else if (oJsonObj.Type.indexOf('PngBase64') >= 0) {//PngBase64
								Imagedata.src = "data:image/png;base64," + oJsonObj.Data;
							}
							else if (oJsonObj.Type.indexOf('TiffBase64') >= 0) {//TiffBase64, currently do not support this type
								Imagedata.src = "data: image/tiff;base64," + oJsonObj.Data;
							}
							//last file name
							strCurrentFileName = oJsonObj.FileName;
							nCurrentFileNo = oJsonObj.Index;
							nTotalFiles = oJsonObj.Total;
							bGetGifFile = true;//we got all a Gif file
						}
					}
				}
				if (!bGetGifFile) {
					var strMessage = 'Can not get the GIF file';
					if (data.type) {
						strMessage += ',DataType:' + data.type;
						if (data.type.indexOf('Text') >= 0) {//can get a message from server
							strMessage += ',Message:' + data.data;
						}
					}
					ShowErrorMessage('Caution:', strMessage);
					//alert(strMessage);//show a message that means could not get uistrings
				}
			}
			//set progress bar
			var oProgressBar = $(Progressbar);
			var nWidth = 0;
			var strText = '';
			if (nTotalFiles > 0) {
				strText = '(' + strCurrentFileName + ') ' + nCurrentFileNo + '/' + nTotalFiles;
				nWidth = nCurrentFileNo / nTotalFiles * 100;
			}
			else {//No Gif file to be shown
				if (oUIStringDic) {
					strText = oUIStringDic.Constant.NoGifFileOnServer;
				}
			}
			oProgressBar.css("width", nWidth + "%");
			oProgressBar.html(strText);
			//for get next image data
			setTimeout(function(){
				ShowNextData();
			}, GetTimeInterval($(TimeInterval).val()));
		},
		"error": function (jqXHR, textStatus, errorThrown) {//for http error
			//show error
			ShowErrorMessage('Server Error!', textStatus);
		},
	};

	$.ajax(settings);
}

//************************************************************************
//Name: 	UploadFiles
//Author: 	TEI (2017/12/14)
//Modify:
//Return:  	
//Description: upload selected files
//
function UploadFiles(e) {
	$('#ShowErrorMessage').hide();//首先关闭所显示的Error
	var files = e.currentTarget.files;
	var data = new FormData();
	for (var i = 0; i < files.length; i++) {
		data.append(files[i].name, files[i]);
	}
	var settings = {
		//async: false,
		//crossDomain: true,
		url: uri + "/Upload",//gifdataController.Upload() will be called
		method: "POST",
		headers: {//Use header to set the content type
			'content-type': "multipart/form-data"//tell server that it will get form data
			//'content-type': " undefined"//
			//"cache-control": "no-cache"
		},
		//contentType: false,//use headers.content-type to set the content type
		processData: false,
		cache: false,
		dataType: 'json',
		data: data,
		xhr: function () {
			var oUploadProgress = $("#UploadProgress");
			oUploadProgress.css("width", "0%");
			oUploadProgress.html("0%");
			var xhr = $.ajaxSettings.xhr();
			//var xhr = new window.XMLHttpRequest();//this is a another way to get xhr
			xhr.upload.addEventListener("progress", function (evt) {
				if (evt.lengthComputable) {
					var progress = Math.round((evt.loaded / evt.total) * 100);
					oUploadProgress.css("width", progress + "%");
					oUploadProgress.html(progress + "%");
				}
			}, false);
			return xhr;
		},
		success: function (data, status, xhr) {//get contents from ResponseParam
			//hide upload progress
			$("#UploadProgressbar").hide();
			//check completed message 
			var contentType = xhr.getResponseHeader("content-type") || "";
			if (contentType.indexOf("json") >= 0) {//we need JSON data
				if ((data) && (data != '')) {
					var strMsg = data;
					if (oUIStringDic) {
						strMsg = oUIStringDic.Constant.UnuploadedFiles + data;
					}
					alert(strMsg);
				}
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			//hide upload progress
			$("#UploadProgressbar").hide();
			//show error
			ShowErrorMessage('Server Error!', textStatus);
		},
	};

	//Show upload progress
	$("#UploadProgressbar").show();
	$.ajax(settings);
}

//************************************************************************
//Name: 	OnIntervalChange
//Author: 	TEI (2017/12/15)
//Modify:
//Return:  	
//Description: check input of interval
//
function OnIntervalChange(sender) {
	if (sender) {
		if (!bCompositioning) {//not IME 
			var oTimeInterval = $(sender);
			var nVal = GetTimeInterval(oTimeInterval.val());
			//reset value
			oTimeInterval.val(nVal);
		}
	}
}

//************************************************************************
//Name: 	OnAnimationOnOffSwitchChanged
//Author: 	TEI (2017/12/15)
//Modify:
//Return:  	
//Description: turn on/off animation
//
function OnAnimationOnOffSwitchChanged(sender) {
	if (sender) {
		var bChecked = isChecked($(sender));
		var oSlider = $('#AnimationOnOffSwitch > span');
		if (bChecked) {//has checked
			oSlider.html('ON');
			oSlider.removeClass('w3-right-align');
			oSlider.addClass('w3-left-align');
		}
		else {
			oSlider.html('OFF');
			oSlider.removeClass('w3-left-align');
			oSlider.addClass('w3-right-align');
		}
	}
}

//************************************************************************
//Name: 	isChecked
//Author: 	TEI (2017/12/16)
//Modify:
//Return:  	
//Description: check a check button is checed
//
function isChecked(oCheck) {
	var bRcd = oCheck.prop('checked');
	return bRcd;
}

//************************************************************************
//Name: 	LimitTimeInterval
//Author: 	TEI (2017/12/15)
//Modify:
//Return:  	
//Description: make correct time interval value
//
function GetTimeInterval(oVal) {
	var nRcd = parseInt(oVal);
	if (nRcd == "NaN") {//use default value
		nRcd = 4000;
	}
	if (nRcd > 20000) {//use max. value
		nRcd = 20000;
	}
	if (nRcd < 1000) {//use min. value
		nRcd = 1000;
	}
	return nRcd;
}

//************************************************************************
//Name: 	getGetUIStringsParamJsonData
//Author: 	TEI (2017/12/11)
//Modify:
//Return:  	JSON data(see GetUIStringsParam in gifdata.cs)
//Description: return a json data that is for get UI strings from server
//
function getGetUIStringsParamJsonData() {
	var strRcd = '';
	//var nIndex = 0;//default is English
	//if (supportLanguages &&
	//	(nCurrentLanguage >= 0) && (nCurrentLanguage < supportLanguages.length)) {
	//	nIndex = nCurrentLanguage;
	//}
	var nIndex = getLauguageIndex();
	var oJsonData = {
		LangguageIndex: nIndex,
	};
	strRcd = JSON.stringify(oJsonData);
	return strRcd;
}

//************************************************************************
//Name: 	getGetNextDataParamJsonData
//Author: 	TEI (2017/12/13)
//Modify:
//Return:  	JSON data(see GetUIStringsParam in gifdata.cs)
//Description: return a json data that is for get next gif data from server
//
function getGetNextDataParamJsonData(nUID) {
	var strRcd = '';
	//var nIndex = 0;//default is English
	//if (supportLanguages &&
	//	(nCurrentLanguage >= 0) && (nCurrentLanguage < supportLanguages.length)) {
	//	nIndex = nCurrentLanguage;
	//}
	var nIndex = getLauguageIndex();
	var strFilter = $("#FileNameFilter").val();
	var bNeedAnimation = isChecked($('#AnimationOnOffSwitch > input'));

	var oJsonData = {
		CallFiFoGUID: nUID,
		LangguageIndex: nIndex,
		Filter: strFilter,
		Animation: bNeedAnimation,
		CurrentFileName: strCurrentFileName
	};
	strRcd = JSON.stringify(oJsonData);
	return strRcd;
}

//************************************************************************
//Name: 	getUploadParamJsonData
//Author: 	TEI (2017/12/20)
//Modify:
//Return:  	JSON data(see UploadFileParam in gifdata.cs)
//Description: return a json data that is for upload files to server
//
//function getUploadParamJsonData(data) {
//	var nIndex = getLauguageIndex();
//	var oJsonData = {
//		LangguageIndex: nIndex,
//		Files:data
//	};
//	return JSON.stringify(oJsonData);
//}


//************************************************************************
//Name: 	getUploadParamJsonData
//Author: 	TEI (2017/12/20)
//Modify:
//Return:  	langguage index of current ui language
//Description: return a correc language index(an index of supportLanguages)
//
function getLauguageIndex() {
	var nIndex = 0;//default is English
	if (supportLanguages &&
		(nCurrentLanguage >= 0) && (nCurrentLanguage < supportLanguages.length)) {
		nIndex = nCurrentLanguage;
	}
	return nIndex;
}

//************************************************************************
//Name: 	ResetUIString
//Author: 	TEI (2017/12/11)
//Modify:
//Return:  	
//Description: set UI strings. call this after GetUIStrings()
//
function ResetUIString() {
	if (oUIStringDic) {
		//find all span tab that its class has .needTranslation
		$('.needTranslation').each(function (index) {
			var strTemp = oUIStringDic[this.id];
			if (strTemp) {
				var oElement = $(this);
				$.each(strTemp, function (key, value) {
					if (key == 'html') {
						oElement.html(value);
					}
					else {
						oElement.attr(key, value);
					}
				});
			}
		});
	}
}



//************************************************************************
//Name: 	Init
//Author: 	TEI (2017/12/9)
//Modify: 	
//Return:  	
//Description: initialize Elements...
//
function Init() {
	biOS = !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
	bMSIE = !!navigator.userAgent.match(/Trident/g) || !!navigator.userAgent.match(/MSIE/g);

	if (biOS) {//for iOS
		$('#btnSaveAs').css('display', 'none');//do not support save image(hide save button)
	}

	//set UI language. default is english
	if (supportLanguages && (supportLanguages.length > 1)) {
		var navLangCountry = '';
		if (navigator.language) {
			navLangCountry = navigator.language.toLowerCase();
			if (navLangCountry == 'ja') {//Chrome的日语版返回ja而不是ja-JP
				navLangCountry = 'ja-jp';//强行改为ja-jp
			}
		}
		else if (navigator.browserLanguage) {
			navLangCountry = navigator.browserLanguage.toLowerCase();
		}
		var navLang = navLangCountry.split('-')[0];//get language
		//nCurrentLanguage = supportLanguages.indexOf(navLang);//IE8の場合indexof()はサポートされていない
		nCurrentLanguage = jQuery.inArray(navLang, supportLanguages);
		if (nCurrentLanguage < 0) {
			nCurrentLanguage = 0;	//实在没有的时候，使用英文
		}
	}
	else {
		nCurrentLanguage = -1;	//supportLanguages变量没有初始化好
	}
}

//************************************************************************
//Name: 	ShowErrorMessage
//Author: 	TEI (2017/12/10)
//Modify:
//Return:  	
//Description: show message (error, warning...)
//
function ShowErrorMessage(title, msg) {//标题，和具体内容；其一为空的时候表示要关闭此信息框
	var errorMessageBox = $('#ShowErrorMessage');
	if (errorMessageBox) {
		if ((title == "") && (msg == "")) {
			//没有给出显示内容时，认为是关闭显示
			//errorMessageBox.style.display = "none";
			errorMessageBox.hide();
		} else {
			//errorMessageBox.empty();//清空当前显示内容
			//从后面去掉两个(default.html中必须有两个element，<h3></h3>和<p></p>)
			var messageHeader = errorMessageBox.children('h3')
			if (messageHeader) {
				messageHeader.html(title);
			}
			var messageParagraph = errorMessageBox.children('p')
			if (messageParagraph) {
				messageParagraph.html(msg);
			}
			//errorMessageBox.html('<h3>' + title + '</h3>' + '<p>' + msg + '</p>'); //添加好显示内容
			//errorMessageBox.style.display = "block";//显示
			errorMessageBox.show();//显示
		}
	}
}

//************************************************************************
//Name: 	ShowHideOnProcessing
//Author: 	TEI (2017/12/10)
//Modify: 	
//Return:  	
//Description: show / hide an onprocessing animation
//
function ShowHideOnProcessing(bShow) {	//true:show
	if (bShow) {
		$("#OnProcessing").show();
	} else {
		$("#OnProcessing").hide();
	}
}

//************************************************************************
//Name: 	openNav
//Author: 	TEI (2017/12/10)
//Modify:
//Return:  	
//Description: Open control pannle
//
function openNav() {
	document.getElementById("ControlPanel").style.display = "block";//show control pannel
	document.getElementById("buttonOpenControlPanel").style.display = "none";//hide open control pannle button
}

//************************************************************************
//Name: 	closeNav
//Author: 	TEI (2017/12/10)
//Modify:
//Return:  	
//Description: Close control pannle
//
function closeNav() {
	document.getElementById("ControlPanel").style.display = "none";//hide contorl pannel
	document.getElementById("buttonOpenControlPanel").style.display = "block";//show open control pannel button
}
