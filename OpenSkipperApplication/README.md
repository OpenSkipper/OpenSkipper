## Change History
  
Here is list of changes to Open Skipper application made by Timo Lappalainen.
My goal is to make this real tool for showing navigation and other boat information. 
Since I already have WLAN on my yacht, with current version it is possible to use e.g. 
tablets for showing all the information.

##### Version 1.6.2016.1217
- Added missing PGN 130316 to PGNDefns.N2kDfn.xml.
- Improved web server sailgauge and compass.

##### Version 1.6.2016.1210
- Fixes for PGNDefns.N2kDfn.xml 
- Several changes on www sample:
    - OpenSkipper.js moved under Scripts
    - New icons for menus
    - Menu loading changed - now does not need $( document ).ready -> simplifies single page code.
    - MyYacht.js for yacht personalization. Currently only name.
    - Added Sailgauge - thanks for Teppo Kurki for Sailgauge base code
- NMEA2000 devices form. Now you can see list of devices on NMEA 2000 stream.
- Parameter filtering for NMEA2000 Source/Destination with device unique ID. Earlier it was possible to filter parameterswith Source/Destination address only. So e.g. if you had two devices providing cabin temperature, you could filter it with e.g. source address 20. Unfortunately if you add new device to your system, the cabin temperature device may change its source address from 20 to 21 due to address claiming. Now it is possible to go to see device unique ID on NMEA2000 devices form (View-"NMEA2000 Devices...") and copy that to the hook Source (Tools-"Parameter Explorer..."-"<under selected parameter hooks>") with prefix ID:. So instead of having Source set as "Any" or "20", set it e.g. "ID:C0789100FFBFFFFF"
*You may need to restart application so that ID:<unique ID> setting will fully work.*
- Fixed big performance issue on keeping history. .NET KeyValuePair did not completely clean up, so I changed it simple ring buffer.

##### Version 1.6.2015.0475 Beta
- Fixed handling for PGN fields. Strings will be cut to first null char. Default (hex) handler also failed. 
-Definition file updates: PGNDefns.N2kDfn.xml. Fixed at last PGN 126996, PGN 127489
-Started NMEA2000 devices form.
-Note! Sorry for mistake in previous version numbering. It should have been 1.6.2015.0450.

##### Version 4.10.2015.0450 Beta
- Fixed LatitudeDDMM and LongitudeDDDMM formats. Now they also allow custom format e.g.
#00° 00' 00.0" N;#00° 00' 00.0" S;00° 00' 00.0" N
- Also changed behaviour so that predined formatters gives predifined format, which can be changed.
So now if you choose speed format and you change fortmat for it, it keep speed formater, but
changes format. This is specially important for special data types like date, time, lat/lon.
- Definition file updates: PGNDefns.N2kDfn.xml, N0183Defns.N0183Dfn.xml.

##### Version 1.6.2015.0416 Beta
- Definition file updates: PGNDefns.N2kDfn.xml, N0183Defns.N0183Dfn.xml. 
Fix for GLL and RMC definition on N0183Defns.N0183Dfn.xml 
- ParametersExtender can now handle multi parameter controls.
So you can define e.g.
```
    <Property name="ParameterName">RelativeWindSpeed_ms;RelativeWindAngle;Speed</Property>
    <Property name="PropertyToSet">ValueOfSeries[0];ValueOfSeries[1];ValueOfSeries[2]</Property>
    <Property name="StreamsToMatch">Any;Any;Any</Property>
```
- Chart control for showing values on time based chart. See display definition file Display_WindGraph.xml. Default single chart definition can be overwritten with property ChartDefinitionFile like in Display_WindGraph.xml
- Chart sample for web. See. Wind2.html and Wind menu on web.
- New setting KeepHistory for parameters. For viewing charts on web pages, they have to get parameter history data from server (OpenSkipper) when pages has been changed. See e.g. parameters RelativeWindSpeed_ms and RelativeWindAngle used on wind samples. KeepHistory has been set to 500 seconds.
- Fixed TCP stream outgoing protocol. Now uses selected protocol like NMEA2000. Earlier version used NMEA0183 even NMEA2000 was selected.

##### Version 1.5.2014.0701
- Definition file updates: PGNDefns.N2kDfn.xml, PGNDefns.N2kDfn.xml, N0183Defns.N0183Dfn.xml. 
Note! N0183Defns.N0183Dfn.xml BWC segment 9 field name changed from "Nautical Miles" to "Distance"
- Fixed error reading lat/long from NMEA0183 messages (see. LatitudeField/LongitudeField).
- On "Data streams"-Logging file name can contain special fields:
##date## - current date
##time## - current time
- Added MainWindow context menu submenu logging for quick access to defined logging streams. This is easier way to start/stop logging streams. Also added property ShowOnMenu for logger stream, so user can choose will stream be viewed on menu or not.
- Fixed PGN 127488. Engine RPM is 1/4 units, so the scale will be 0.25.

##### Version 1.5.2014.0608
- Now it can show parameters on web even does not exist on any application view
- Added "Streams" definition for web parameters. So now you can e.g. define parameter:
{OpenSkipperParam:Latitude,Longitude:Position:ResultType=Numeric:CallName=SetPosition:CallType=ValueOnly:Streams=Main NMEA2000} so Latitude, and Longitude will be cathed only from "Main NMEA2000" stream.

##### Version 1.5.2014.0605
- New web pages. These are samples, which can be done with my 2 week JavaScript learning. The click menu in new web pages works also on mobile, IE, Chrome and FireFox. There was actually rather much tuning to get pages to work in same way on all those browsers.
- New way to handle web data. With configuration files under ParamData you can cofigure own JavaScript function,  which will be called for listed parameter. E.g. in Map.conf there is row:
{OpenSkipperParam:Latitude,Longitude:Position:ResultType=Numeric:CallName=SetPosition:CallType=ValueOnly} which defines that we would like to get Latitude and longitude values in Numeric format for function SetPosition.  So in Map.js we have "function SetPosition(_lat,_lon)", which will be then called. In Map.html we still use OpenSkipper.js default updater "function UpdateLiveData()", but e.g. in Compass.html we have own faster 200 ms updater "function UpdateCompassData()".  So there are now several ways to build your web pages.  The old quick way like in Battery.html works still fine.
- Fixes for web server. Now works better with complicate code like my new pages. See my new views with click menus, which works with
browsers and mobile.
- New parameter type NumericMultiplication. Now it is possible to show e.g. tank capacity in liters by multilying Level and Capacity.

Notes:
- Web server still can not show parameters, which does not exist on any application view. I'll try to resolve this some time. 

##### Version 1.4.2014.0529
- Performance improvement. On my computer earlier release took on test 12% of CPU - now only 3%. There are still places to improve.
- PGN 129540 differs between newer packetlogger and old PGN definitions, so old PGN defnition for that PGN was copied to keep compatibility with Kees NMEA2000 Sample.xml.
- Fixed crash for field length >8 bits like 11 bit manufacturer code in PGN 130840
- Fixed lost data handling to allow Replay for old log.

##### Version 1.4.2014.0526
- Added formatters latitudeDDMM and longitudeDDDMM for more visual output to be used on Parameters.N2kParams.xml.

##### Version 1.3.2014.0523
- Improved Serial port listener and fixed bugs on it.
- PGNDefns.N2kDfn.xml fixes. Loaded Datalogger 15.04.2012 version. Note that this caused some changes in parameter names. E.g. Speed is now "Speed Water Referenced" and SOG is now "Speed Ground Referenced". This is fixed on Parameters.N2kParams.xml, but if you have your own, you have to fix it.
- Fixed bug on parameter filter handling. Now works better with NMEA2000 values. 

##### Version 1.3.2014.0511
- Changed default setting for "Hide Menus On Start" to false so it will not confuse old users.
- Added WWW port configuration so that OpenSkipper web server can be used in combination with other systems.

##### Version 1.3.2014.0501
- New parameters TopMost, DefLocation and StartMinimized for display definitions.  Now application also saves user defined form positions.
- Changed Tools-"Actisense NGT-1 USB Explorer ..." to more common Tools-"Serial port listener...". The modified listener has option to listen in NMEA2000, NMEA0183 (Ascii) or HEX mode. You can choose also desired baud rate. The listener form has changed modeless, so you can use it as separate form. With com0com and hub4com you can spy the traffic while data is shown on controls.
- Now display selection chort cut keys works also on all forms.

##### Version 1.3.2014.0427
- Now also display type Form is supported. So you can have all displays in own forms instead of tab pages.
- Form can be moved by mouse down almost anywere withing form. This makes positioning much easier.
- Forms also have context menu (right click over form) for e.g. minimizing, choosing other views (tabpage or form) and some other tasks. It may grow in future.

##### Version 1.3.2014.0423
- There is no more default dials page. All pages are loaded through definition .xml files. Displays.xml defines a list of displays (currently only TabPage type is supported.). In sample Displays.xml there are four tabpages, where the first is like original Dials, but it has been defined with Display_Dials.xml. So you do not need Visual Studio to define your own tab pages at all. Unfortunately you will need some knowledge of available components and how to use them. Hopefully four sample pages gives good start.
- You can also edit display definition files on the fly. After you have made your changes just press ctrl-shift-R to update changes.
- Now application opens as default to minimal view, which has fixed position. By pressing ctrl-M, you get menu view so that you can move window and also have all old menus back. Then switch back to minimal view with ctrl-M. You can set startup behaviour under File-Settings...I will implement in future also some way to move window in minimal state.
- If you have definition files under application path, application will use them as default.
- Better Web server. Now you can write your www code all by your self. There are sample files under www directory. As in sample, you can also use images and style files for your web pages as in normal web programming.
    Currently parameter delivery works as in old version with POST. There are few important things. 
    Let's look e.g. Steer.html sample file:
    - ```<script type="text/javascript" language="javascript">var ParamPageName="/Steer.html"</script>```
This definition tells that for this file we use parameters defined on page Steer.html. At beginning it is easiest to define parameters on same page as your display.	  
    - ```<script src="OpenSkipper.js"></script>```
Include also default script file. OpenSkipper has special handling for requests to /ParamData/.... 
It then responces u(ParamID,Value); u(...)... with parameters defined for that page. In default OpenSkipper.js there is handler for that u(...) function. At beginning it is easiest to use that and after getting in to system you can make more pfroessional views. But just keep in mind special handling for requests to /ParamData/... and responce for that is format u(ParamID,Value);
    - ```<td><span name="t1" id="{OpenSkipperParam:Heading}"></span></td>```
With this we define that at this position comes parameter "Heading". If you want to show same parameter several times on same page, you need to add parameter ID after definition like: {OpenSkipperParam:Heading:Heading_1}.
- You can also set web server to start automatically. If you go to the View-"Web Server..." and 
set "Enable Web server" and "Start Web server to last state" to checked, it will be automatically 
started when application starts.
- Web server also works now with normal user rights for all network. You need to once start web server as "Local host only" uncheked so that application will ask to give rights for normal user to listen all network. Appliction will give these rights, but for one time you need to use administrator rights.
- Multiplier for parameters. In Parameters.N2kParams.xml you can define multiplier, which multiplies parameter received from hook.
- Hook filtering. E.g. NMEA0183 message MWV can contain true or apparent wind speed depending field "Reference, R = Relative, T = True" value. So as definition Parameters.N2kParams.xml file you can add filter "R" relative wind speed parameter and filter "T" for true values.
- Message instance filtering for "Decoded messages..." view.
- Some fixes too.
