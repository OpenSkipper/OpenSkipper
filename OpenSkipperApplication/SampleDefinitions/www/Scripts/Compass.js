      var RS=2;
      var HeadingMark=[[0,10],[-5,0],[5,0]];
      var RoseBlack=[[[0*RS,0*RS],[0*RS,28*RS],[4*RS,8*RS]],
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[14*RS,14*RS],[8*RS,4*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]] 
                    ];
      var RoseWhite=[[[0*RS,0*RS],[0*RS,28*RS],[-4*RS,8*RS]],
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[14*RS,14*RS],[4*RS,8*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]], 
                     [[0*RS,0*RS],[0*RS,0*RS],[0*RS,0*RS]] 
                    ];
      
      function InitRosePoints() {
        for (var I=1; I<4; I++) {
          var sinRot = Math.sin(I * 90 * Math.PI / 180); 
          var cosRot = Math.cos(I * 90 * Math.PI / 180);
          for (var J=0; J<3; J++) {
            RoseBlack[I][J][0]=(cosRot*RoseBlack[0][J][0]+sinRot*RoseBlack[0][J][1]);
            RoseBlack[I][J][1]=(-sinRot*RoseBlack[0][J][0]+cosRot*RoseBlack[0][J][1]);
            RoseBlack[4+I][J][0]=(cosRot*RoseBlack[4][J][0]+sinRot*RoseBlack[4][J][1]);
            RoseBlack[4+I][J][1]=(-sinRot*RoseBlack[4][J][0]+cosRot*RoseBlack[4][J][1]);
            RoseWhite[I][J][0]=(cosRot*RoseWhite[0][J][0]+sinRot*RoseWhite[0][J][1]);
            RoseWhite[I][J][1]=(-sinRot*RoseWhite[0][J][0]+cosRot*RoseWhite[0][J][1]);
            RoseWhite[4+I][J][0]=(cosRot*RoseWhite[4][J][0]+sinRot*RoseWhite[4][J][1]);
            RoseWhite[4 + I][J][1] = (-sinRot * RoseWhite[4][J][0] + cosRot * RoseWhite[4][J][1]);
          };
        }
      }
      
      function Polygon(ctx,points) {
        ctx.beginPath();
        ctx.moveTo(points[0][0],points[0][1]);
        for (var i = 1; i < points.length; i++) {
          ctx.lineTo(points[i][0],points[i][1]);
        }
        ctx.closePath();
        ctx.fill(); 
      }
      
      function DrawHeadingMark(ctx) {
        ctx.fillStyle = '#f00';
        Polygon(ctx,HeadingMark);
      }
      
      function DrawRose(ctx) {
        ctx.fillStyle = '#000';
        for (var I=0; I<8; I++) {
          Polygon(ctx,RoseBlack[I]);
        }
        ctx.fillStyle = '#f00';
        for (var I=0; I<8; I++) {
          Polygon(ctx,RoseWhite[I]);
        }
      }

      function DrawTics(ctx,CircleRadius) {
        var mtl = 10;
        for (var I = 0; I < 36; I++) {
            var angle = I * 10 * Math.PI / 180.0;
            ctx.moveTo( (CircleRadius - mtl) * Math.cos(angle) , (CircleRadius - mtl) * Math.sin(angle) );
            ctx.lineTo( (CircleRadius) * Math.cos(angle), (CircleRadius) * Math.sin(angle) );
        }
        mtl=mtl/2;
        for (var I = 0; I < 36; I++) {
            var angle = (5+I * 10) * Math.PI / 180.0;
            ctx.moveTo( (CircleRadius - mtl) * Math.cos(angle) , (CircleRadius - mtl) * Math.sin(angle) );
            ctx.lineTo( (CircleRadius) * Math.cos(angle), (CircleRadius) * Math.sin(angle) );
        }
        ctx.stroke();
      }
      
      function DrawCompass(val,canvasName) {
        var canvas = document.getElementById(canvasName);
        var context = canvas.getContext('2d');
        var CircleRadius=100;
        var angle=(isNaN(val)?0:val);
        
        var c2 = canvas.getContext('2d');
        
        c2.setTransform(1, 0, 0, 1, 0, 0);
        c2.translate(110,0);
        DrawHeadingMark(c2);
        c2.translate(0,110);
        c2.rotate(-angle*Math.PI/180);
        c2.moveTo(100,0);  // goto start point to avoid extra line.
        if (val=="Lost") {
          c2.fillStyle = 'yellow';
        } else if (val=="Error") {
          c2.fillStyle = 'red';
        } else c2.fillStyle = '#fff';
        c2.arc(0,0,100,0,2*Math.PI);
        c2.fill();
        DrawTics(c2,CircleRadius);
        DrawRose(c2);
        
        c2.font="18px Arial";
        c2.textAlign = 'center';
        c2.fillText("N",0,-CircleRadius+30);
        c2.rotate(90*Math.PI/180);
        c2.fillText("E",0,-CircleRadius+30);
        c2.rotate(90*Math.PI/180);
        c2.fillText("S",0,-CircleRadius+30);
        c2.rotate(90*Math.PI/180);
        c2.fillText("W",0,-CircleRadius+30);
      }
      
      function SetHeading(b) {
         DrawCompass(b);
         document.getElementById("Heading").innerHTML = parseFloat(b).toFixed(0);
      }

      function SetPosition(lat,lon) {
         document.getElementById("Position").innerHTML = parseFloat(lat).toFixed(3).toString()+" "+parseFloat(lon).toFixed(3).toString();
      }
      InitRosePoints();

