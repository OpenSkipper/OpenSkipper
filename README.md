# OpenSkipper

Welcome to the **OpenSkipper** project, which provides Open Source C# code for Windows for integrating and displaying NMEA 0183, NMEA 2000 and AIS data from nautical instruments, GPS units and internet data sources. OpenSkipper can be run on a laptop aboard your boat to show electronic instruments displaying speed, heading, etc. OpenSkipper can also receive and transmit data over multiple connections, including a serial port (for NMEA 0183), an ActiSense NGT-1-USB NMEA-2000-to-USB converter to read NMEA 2000 (N2K) data, and wired and wireless network connections (including TCP and UDP). It also contains a built-in webserver, so you can run OpenSkipper on a laptop and use this to display data on an iPad or Android phone or tablet.

OpenSkipper was initially developed by [Dr Andrew Mason](http://des.auckland.ac.nz/Mason) in the [Yacht Research Unit](http://www.mech.auckland.ac.nz/uoa/home/about/ourresearch/research-facilities/yachtresearchunit) and [Dept of Engineering Science](http://www.des.auckland.ac.nz/) at the [University of Auckland](http://auckland.ac.nz/), New Zealand, with fantastic assistance from student Jason Drake. OpenSkipper was updated in 2014 by Timo Lappalainen and Kave Oy from Finland.

If you are interested in the NMEA 2000 standard, then OpenSkipper provides both code and definition files for interpreting NMEA 2000 messages. This work uses the definition files developed by [Kees Verruijt](https://plus.google.com/106761082826017451274) as part of his excellent [CanBoat](https://github.com/canboat/canboat) project.

NMEA 2000 messages can be received using the [Actisense](http://www.actisense.com) NGT-1-USB NMEA-to-USB converter (a product that we recommend). We have not signed any non disclosure agreements with Actisense, but instead written our own low level COM-port driver. Our accessing of an NGT-1 in this way is not officially supported by Actisense. It works for us, but we take no responsibility in any way for any consequences.

OpenSkipper contains XML definition files used to describe how an NMEA 2000, AIS and NMEA 0183 message should be decoded. These definition files are a beta release, have not been tested and contain errors, so please do not rely on any output from open skipper for your navigation. These definitions are unofficial, and are not supported by [National Marine Electronics Association (NMEA)](http://www.nmea.org) or any other body in any way. We welcome community feedback on improvements to these.

OpenSkipper is beta software designed to serve only as an aid for navigators. It in no way replaces the need to follow good nautical practices.

By using this software, you agree to the terms of the [GNU Public License v3](http://www.gnu.org/licenses/gpl.html), and in particular that:

There is no warranty for the program, to the extent permitted by applicable law. Except when otherwise stated in writing the copyright holders and/or other parties provide the program "as is" without warranty of any kind, either expressed or implied, including, but not limited to, the implied warranties of merchantability and fitness for a particular purpose. The entire risk as to the quality and performance of the program is with you. Should the program prove defective, you assume the cost of all necessary servicing, repair or correction.

In no event unless required by applicable law or agreed to in writing will any copyright holder, or any other party who modifies and/or conveys the program as permitted above, be liable to you for damages, including any general, special, incidental or consequential damages arising out of the use or inability to use the program (including but not limited to loss of data or data being rendered inaccurate or losses sustained by you or third parties or a failure of the program to operate with any other programs), even if such holder or other party has been advised of the possibility of such damages.

All trademarked terms are the property of their respective owners.
