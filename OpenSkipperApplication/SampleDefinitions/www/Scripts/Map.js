      var map = null;
      var marker = null;
      var curLat;
      var curLon;
      var curZoom=11;
      var OrientationAbsolute=0;

      function ResizeMap() {
        if (map!=null) {
          var center = map.GetCenter();
          curZoom = map.GetZoomLevel();
          curLat = center.Latitude;
          curLon = center.Longitude;
          marker=null; map=null;
          SetPosition(curLat,curLon);
        }
      }
      
      function SetPosition(_lat,_lon) {
        var lat=(isNaN(_lat)?curLat:_lat);
        var lon=(isNaN(_lon)?curLon:_lon);
        if (map==null) {
          map = new VEMap('PositionMap');            
          map.LoadMap(new VELatLong(lat, lon), curZoom, VEMapStyle.Aerial, false);
          marker = new VEShape(VEShapeType.Pushpin, (new VELatLong(lat, lon)));
          map.AddShape(marker);
        } else {
          marker.SetPoints(new VELatLong(lat, lon));
        }
        curLat=lat; curLon=lon;
      }
      
      function SetPositionStr(lat,lon,hdg,speed) {
        document.getElementById("Position").innerHTML = lat+" "+lon+"   Heading="+hdg+"  Speed="+speed;
      }
      
      function ShowSizeInfo() {
          document.getElementById('Dimensions').innerHTML=window.innerWidth.toString()+"/"+window.outerWidth.toString()+"/"+screen.width.toString()
                                                          +", "+window.innerHeight.toString()+"/"+window.outerHeight.toString()+"/"+screen.height.toString()
                                                          +", "+OrientationAbsolute.toString()
                                                          ;
      }
      
      function ResizeView() {
          var w = Math.min(window.innerWidth, window.outerWidth, screen.width);
          var h = Math.min(window.innerHeight, window.outerHeight, screen.height);
          var viewW=w-30;
          document.getElementById('MainDiv').style.width = viewW.toString()+"px";
          var HeadDiv = document.getElementById('HeadDiv');
          var MainDiv = document.getElementById('MainDiv');
          var DataDiv = document.getElementById('DataDiv');
          var viewH=h-HeadDiv.scrollHeight-HeadDiv.offsetTop-10;
          var MapH=viewH-DataDiv.scrollHeight-10;
          document.getElementById('MainDiv').style.width = viewW.toString()+"px";
          document.getElementById('MainDiv').style.height = viewH.toString()+"px";
          document.getElementById('PositionMap').style.height = MapH.toString()+"px";
          ResizeMap();
//              ShowSizeInfo();
      }

      function OrientationChange(event) {
        OrientationAbsolute++;
        var alpha    = event.alpha;
        var beta     = event.beta;
        var gamma    = event.gamma;
      }
      window.addEventListener("deviceorientation", OrientationChange, true);
      window.addEventListener("resize", ResizeView, false);
      //  "onresize="ResizeView()"
