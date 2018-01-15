     var http_request = false;

     function makeRequest(url, parameters) {
        http_request = false;
        if (window.XMLHttpRequest) { // Mozilla, Safari,...
           http_request = new XMLHttpRequest();
           if (http_request.overrideMimeType) {
            // set type accordingly to anticipated content type
              //http_request.overrideMimeType('text/xml');
              http_request.overrideMimeType('text/html');
           }
        } else if (window.ActiveXObject) { // IE
           try {
              http_request = new ActiveXObject("Msxml2.XMLHTTP");
           } catch (e) {
              try {
                 http_request = new ActiveXObject("Microsoft.XMLHTTP");
              } catch (e) {}
           }
        }
        if (!http_request) {
           alert('Cannot create XMLHTTP instance');
           return false;
        }
        http_request.onreadystatechange = alertContents;
        http_request.open('GET', url + parameters, true);
        http_request.send(null);
     }
  
     // AJM Test of using POST insead of GET
     function makePOSTRequest(url, parameters) {
        http_request = false;
        if (window.XMLHttpRequest) { // Mozilla, Safari,...
           http_request = new XMLHttpRequest();
           if (http_request.overrideMimeType) {
            // set type accordingly to anticipated content type
              //http_request.overrideMimeType('text/xml');
              http_request.overrideMimeType('text/html');
           }
        } else if (window.ActiveXObject) { // IE
           try {
              http_request = new ActiveXObject("Msxml2.XMLHTTP");
           } catch (e) {
              try {
                 http_request = new ActiveXObject("Microsoft.XMLHTTP");
              } catch (e) {}
           }
        }
        if (!http_request) {
           alert('Cannot create XMLHTTP instance');
           return false;
        }
        
        http_request.onreadystatechange = alertContents;
        http_request.open('POST', url, true);
        http_request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
//          http_request.setRequestHeader("Content-length", parameters.length);
//          http_request.setRequestHeader("Connection", "close");
        http_request.send(parameters);
     }

     function u(a,b) {
        document.getElementById(a).innerHTML = b;
     }

     function alertContents() {
        var ErrorElem=null;
        if (http_request.readyState == 4) {
           ErrorElem=document.getElementById('error');
           if (http_request.status == 200) {
              //alert(http_request.responseText);
              var response = ""; // This is needed for IE8 to work
              response = http_request.responseText;
              // Display the returned string for debugging purposes
              // document.getElementById('response').innerHTML = response;
              // We execute the Javascript commands returned by the server
              eval(response);
              // Clear any previous error
              if (ErrorElem!=null) ErrorElem.innerHTML = "";
           } else {
              // alert('There was a problem with the request.');
              if (ErrorElem!=null) ErrorElem.innerHTML = "Error: No response received when requesting updated data from the server.";
           }
        }
     }
     
    function UpdateLiveData() {
    	// The {1} is replaced by a counter that distinguishes individual connections
    	// We send this counter value as a parameter in our 
      if (ParamPageName != null) {
        makePOSTRequest("ParamData"+ParamPageName, ''); // This uses the POST method
        setTimeout("UpdateLiveData()",1000)
      }
    }
    
    UpdateLiveData();
