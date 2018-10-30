import { Component, OnInit, OnDestroy } from '@angular/core';
import { RobotService } from 'src/app/core/services/robot.service';
import { Subscription, TimeInterval } from 'rxjs';
import { delay } from 'q';

@Component({
  selector: 'app-qrcode',
  templateUrl: './qrcode.component.html',
  styleUrls: ['./qrcode.component.css']
})
export class QrcodeComponent implements OnInit, OnDestroy {
  imageData = 'https://d2gg9evh47fn9z.cloudfront.net/800px_COLOURBOX19924183.jpg';
  guideTitle = 'Loading the QR code from Whatsapp...';
  private qrImageSub:Subscription;
  private isRobotLoginSub:Subscription;
  private qrTimer = null;
  private isRobotLoggedIn = false;
  
  constructor(private robotService: RobotService) { 
    this.qrImageSub = this.robotService.QrImageResult$.subscribe(result => {
      if (result){
        var qrcode = JSON.parse(result)
        this.imageData = qrcode.QRCodeBase64;
      }
    });
    this.isRobotLoginSub = this.robotService.RobotLoginStatusResult$.subscribe(result=>{
      var newStatus = result.toLowerCase()=='true'?true:false;
      if(newStatus!=this.isRobotLoggedIn){
        this.isRobotLoggedIn = newStatus;
        if(this.isRobotLoggedIn){
          clearInterval(this.qrTimer);
        }
      }
    });
  }

  ngOnInit() {
    delay(1000).then(()=>{
      this.getQRCode();
    });
    this.qrTimer = setInterval(()=>{
      this.getQRCode();
    }, 15000);
  }

  ngOnDestroy() {
    this.qrImageSub.unsubscribe();
    clearInterval(this.qrTimer);
  }

  refreshQrCode(){
    this.getQRCode();
  }

  private getQRCode(){
    this.robotService.sendMessage(<any>{}, 'GetQRCodeMessage');
  }
}