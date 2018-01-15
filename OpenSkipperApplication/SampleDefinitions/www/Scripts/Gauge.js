// Gauge by Timo Lappalainen, Kave Oy
// Use for free, keep author information on code.
// No warranty. Use with your own risk!
// All scaling etc. setting has not been completely tested.

// Using Gauge:
//  <body>
//    <canvas id="RPMGauge" width="220" height="220"></canvas>
//    <script>
//      var RPMGauge=new Gauge("RPMGauge");
//      // You can add a new range
//      RPMGauge.Ranges.push({End:4000,Color:"Blue"});
//      // You can change MinMax
//      RPMGauge.SetMinMax(0,4000);
//      // You can define tics distance
//      RPMGauge.SetTics(400,40);
//      // You can make it larger, if you have canvas big enough. 
//      RPMGauge.SetScale(1.5,1.5); 
//      RPMGauge.Set(0);
//    </script>
//  </body>

      function Polygon(ctx,points) {
        ctx.beginPath();
        ctx.moveTo(points[0][0],points[0][1]);
        for (var i = 1; i < points.length; i++) {
          ctx.lineTo(points[i][0],points[i][1]);
        }
        ctx.closePath();
        ctx.fill(); 
      }
      
      function DrawGaugeRange(ctx,rangeStartAngle,rangeEndAngle,radius,width,color) {
        ctx.fillStyle = color;
        ctx.beginPath();
        ctx.moveTo( (radius - width) * Math.cos(rangeStartAngle) , (radius - width) * Math.sin(rangeStartAngle) );
        ctx.lineTo( (radius) * Math.cos(rangeStartAngle), (radius) * Math.sin(rangeStartAngle) );
        ctx.arc(0,0,radius,rangeStartAngle,rangeEndAngle);
        ctx.lineTo( (radius-width) * Math.cos(rangeEndAngle), (radius-width) * Math.sin(rangeEndAngle) );
        ctx.arc(0,0,radius-width,rangeEndAngle,rangeStartAngle,true);
        ctx.closePath();
        ctx.fill();
      }
      
      function ScalePoints(scale,points) {
        for (var i = 0; i < points.length; i++) {
          points[i][0]*=scale;
          points[i][1]*=scale;
        }
      }
      
      function Gauge(CanvasName) {
        this.NeedleRight=[[-5,0],[-20,-5],[70,0]];
        this.NeedleLeft=[[-5,0],[-20,5],[70,0]];
        this.NeedleBack=[[-5,0],[-20,-5],[-20,5]];
        this.NeedleErase=[[70,0],[-25,-25],[-25,25]];
        this.NeedleCenterRadius=8;
        this.StartAngle=-225;
        this.EndAngle=45;
        this.MinVal=0;
        this.MaxVal=3000;
        this.MainDiv=200;
        this.MinorDiv=20;
        this.valToAngleMult=(this.EndAngle-this.StartAngle)/(this.MaxVal-this.MinVal);
        this.Initialized=false;
        this.Ranges=[{End:600,Color:"yellow"},{End:2700,Color:"lime"},{End:3000,Color:"red"}];
        this.RangeWidth=10;
        
        this.canvas = document.getElementById(CanvasName);
        this.GaugeRadius=70;
        this.ScaleX=1;
        this.ScaleY=1;
        this.CanvasW=this.canvas.width;
        this.CanvasH=this.canvas.height;
        
        this.Value=0;
        this.Unit=" RPM";
        this.Desim=0;
        this.AutoScale=true;
        this.State="NR";
        this.StateOK=false;
        
        this.ctx = this.canvas.getContext('2d');
      }
      
      // Functions for chaning gauge layout
      
      Gauge.prototype.Reset = function() {
        if (this.Initialized) {
          this.Initialized=false;
          this.ctx.setTransform(1, 0, 0, 1, 0, 0);
          this.ctx.clearRect(0,0,this.canvas.width,this.canvas.height);
        }
      }
      
      Gauge.prototype.SetGaugeRadius = function(radius,width) {
        ScalePoints(radius/this.GaugeRadius,this.NeedleRight);
        ScalePoints(radius/this.GaugeRadius,this.NeedleLeft);
        ScalePoints(radius/this.GaugeRadius,this.NeedleBack);
        ScalePoints(radius/this.GaugeRadius,this.NeedleErase);
        this.NeedleCenterRadius*=radius/this.GaugeRadius;
        this.GaugeRadius=radius;
        this.RangeWidth=width;
        this.Reset();
      }
      
      Gauge.prototype.SetTics = function(mainDiv,minorDiv) {
        this.MainDiv=mainDiv;
        this.MinorDiv=minorDiv;
        this.Reset();
      }
      
      Gauge.prototype.SetScale = function(sx,sy) {
        this.ScaleX=sx;
        this.ScaleY=sy;
        this.AutoScale=false;
        this.Reset();
      }
      
      Gauge.prototype.SetMinMax = function(min,max) {
        this.MinVal=min;
        this.MaxVal=max;
        this.valToAngleMult=(this.EndAngle-this.StartAngle)/(this.MaxVal-this.MinVal);
        this.Ranges[this.Ranges.length-1].End=max;
        this.Reset();
      }
      
      Gauge.prototype.SetStartEndAngle = function(sa,ea) {
        this.StartAngle=sa;
        this.EndAngle=ea;
        this.valToAngleMult=(this.EndAngle-this.StartAngle)/(this.MaxVal-this.MinVal);
        this.Reset();
      }
      
      Gauge.prototype.valToAngle = function(val) {
        return (this.StartAngle+val*this.valToAngleMult)* Math.PI / 180.0;
      }
      
      Gauge.prototype.EraseNeedle = function() {
        this.ctx.globalCompositeOperation = 'destination-out';
//        Polygon(ctx,NeedleErase);
        Polygon(this.ctx,this.NeedleErase);
        this.ctx.globalCompositeOperation="source-over";
      }
      
      Gauge.prototype.DrawNeedle = function() {
        // Center circle
        this.ctx.fillStyle = '#666666';
        this.ctx.beginPath();
        this.ctx.moveTo(0,this.NeedleCenterRadius);
        this.ctx.arc(0,0,this.NeedleCenterRadius,0,2*Math.PI);
        this.ctx.closePath();
        this.ctx.fill();
        // Needle
        this.ctx.fillStyle = '#111111';
        Polygon(this.ctx,this.NeedleLeft);
        this.ctx.fillStyle = '#888888';
        Polygon(this.ctx,this.NeedleRight);
        this.ctx.fillStyle = '#444444';
        Polygon(this.ctx,this.NeedleBack);
      }

      Gauge.prototype.DrawTics = function() {
        var OutRad=this.GaugeRadius+this.RangeWidth;
        var mtl = this.RangeWidth;
        var ntics = (this.MaxVal-this.MinVal)/this.MainDiv;
        var angleDiv=(this.EndAngle-this.StartAngle)/ntics;
        this.ctx.beginPath();
        for (var I = 0; I < ntics+1; I++) {
            var angle = (this.StartAngle + I * angleDiv) * Math.PI / 180.0;
            this.ctx.moveTo( (OutRad - mtl) * Math.cos(angle) , (OutRad - mtl) * Math.sin(angle) );
            this.ctx.lineTo( (OutRad) * Math.cos(angle), (OutRad) * Math.sin(angle) );
        }
        mtl=mtl/2;
        ntics=(this.MaxVal-this.MinVal)/this.MinorDiv;
        angleDiv=(this.EndAngle-this.StartAngle)/ntics;
        for (var I = 0; I < ntics+1; I++) {
            var angle = (this.StartAngle + I * angleDiv) * Math.PI / 180.0;
            this.ctx.moveTo( (OutRad - mtl) * Math.cos(angle) , (OutRad - mtl) * Math.sin(angle) );
            this.ctx.lineTo( (OutRad) * Math.cos(angle), (OutRad) * Math.sin(angle) );
        }
        this.ctx.closePath();
        this.ctx.stroke();
      }

      Gauge.prototype.DrawValues = function() {
        var ntics = (this.MaxVal-this.MinVal)/this.MainDiv;
        var angleDiv=(this.EndAngle-this.StartAngle)/ntics;
        this.ctx.textBaseline="middle";
        this.ctx.font="10px Arial";
        this.ctx.textAlign = 'center';
        // Calculate drawing radius for text by using MaxValue string.
        var radius=this.GaugeRadius+this.RangeWidth+this.ctx.measureText(this.MaxVal.toString()).width/1.7;
        for (var I = 0; I < ntics+1; I++) {
          var angle = (this.StartAngle + I * angleDiv) * Math.PI / 180.0;
          var val=this.MinVal+I*this.MainDiv;
          this.ctx.fillText(val.toString(),(radius) * Math.cos(angle), (radius) * Math.sin(angle));
        }
      }
      
      Gauge.prototype.DrawValue = function(value) {
        if (this.StateOK) {
          this.ctx.clearRect(-50,this.GaugeRadius-25,100,30);
          this.ctx.font="16px Arial";
          this.ctx.textAlign = 'right';
          this.ctx.fillStyle = '#000000';
          var text=(this.StateOK?value.toFixed(this.Desim).toString()+this.Unit:this.State);
          this.ctx.fillText(value.toFixed(this.Desim).toString()+this.Unit,35,this.GaugeRadius-10);
        } else {
          this.ctx.font="16px Arial";
          this.ctx.textAlign = 'center';
          if (this.State=="Lost") {
            this.ctx.fillStyle = 'yellow';
          } else {
            this.ctx.fillStyle = 'red';
          }
          this.ctx.fillRect(-50,this.GaugeRadius-25,100,30);
          this.ctx.fillStyle = '#000000';
          this.ctx.fillText(this.State,0,this.GaugeRadius-10);
        }
      }
      
      Gauge.prototype.DrawRanges = function() {
        var OutRad=this.GaugeRadius+this.RangeWidth;
        var RangeStartAngle=this.valToAngle(this.MinVal);
        var RangeEndAngle=this.valToAngle(this.MaxVal);
        var RangeStart=this.MinVal;
        
        for (var i=0; i<this.Ranges.length;i++) {
          DrawGaugeRange(this.ctx,this.valToAngle(RangeStart),this.valToAngle(this.Ranges[i].End),OutRad,this.RangeWidth,this.Ranges[i].Color);
          RangeStart=this.Ranges[i].End;
        }
        this.ctx.fillStyle = '#555555';
        this.ctx.beginPath();
        this.ctx.moveTo( (OutRad) * Math.cos(RangeStartAngle) , (OutRad) * Math.sin(RangeStartAngle) );
        this.ctx.arc(0,0,OutRad,RangeStartAngle,RangeEndAngle);
        this.ctx.stroke();
      }
      
      Gauge.prototype.Set = function(val) {
        var value=this.MinVal;
        var stateOK=(!isNaN(val));
        // Do drawing only on change.

        if ((this.Initialized) && (value==this.Value) && (stateOK==this.StateOK)) return;
        
        if (stateOK) {
          value=val;
          this.State="OK";
        } else {
          this.State=val;
        }
        this.StateOK=stateOK;

        if (value<this.MinVal) value=this.MinVal;
        if (value>this.MaxVal) value=this.MaxVal;
        
        this.ctx.setTransform(1, 0, 0, 1, 0, 0);
        if (this.AutoScale) {
          if ((!this.Initialized) || (this.CanvasW!=this.canvas.width) || (this.CanvasH=this.canvas.height)) {
            this.SetScale(this.canvas.width/220,this.canvas.height/220); 
          }
        }
        if (!this.Initialized) {
          this.ctx.translate(this.canvas.width/2,this.canvas.height/2);
          this.ctx.scale(this.ScaleX,this.ScaleY);
          this.DrawRanges();
          this.DrawTics();
          this.DrawValues();
          this.DrawValue(value);
          this.Initialized=true;
          this.ctx.rotate(this.valToAngle(value));
          this.CanvasW=this.canvas.width;
          this.CanvasH=this.canvas.height;
        } else {
          this.ctx.translate(this.canvas.width/2,this.canvas.height/2);
          this.ctx.scale(this.ScaleX,this.ScaleY);
          this.DrawValue(value);
          this.ctx.rotate(this.valToAngle(this.Value));
          this.EraseNeedle();
        }
        this.ctx.setTransform(1, 0, 0, 1, 0, 0);
        this.ctx.translate(this.canvas.width/2,this.canvas.height/2);
        this.ctx.scale(this.ScaleX,this.ScaleY);
        this.ctx.rotate(this.valToAngle(value));
        this.DrawNeedle();
        this.Value=value;
      }