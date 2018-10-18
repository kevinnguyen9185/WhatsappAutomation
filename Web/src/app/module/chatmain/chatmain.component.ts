import { Component, OnInit } from '@angular/core';
import { RobotService } from 'src/app/core/services/robot.service';

@Component({
  selector: 'app-chatmain',
  templateUrl: './chatmain.component.html',
  styleUrls: ['./chatmain.component.css']
})
export class ChatmainComponent implements OnInit {
  public imageData:String = '';

  constructor(private robotService: RobotService) {
    this.robotService.getInstanceStatus(localStorage.getItem('phoneno')).subscribe(result=>{
      console.log(result.data);
      var mess = JSON.parse(result.data);
      switch (mess.MessageType)
      {
        case "PairRobotUIResponseMessage":
          var robotConnId = JSON.parse(mess.Message).RobotConnId;
          localStorage.setItem('robotConnId', robotConnId);
          console.log(robotConnId);
          break;
        case "ErrorMessage":
          console.log(mess.Message);
          break;
        case "GetQRCodeResponseMessage":
          var qrObj = JSON.parse(mess.Message);
          this.imageData = qrObj.QRCodeBase64;
          break;
        default:
          break;
      }
    });
  }

  ngOnInit() {
  }

  public getQRCode(){
    this.robotService.sendMessage(JSON.stringify(<SendMessage>{
      Sender : localStorage.getItem('phoneno'),
      MessageType : 'GetQRCodeMessage',
      Message : '{}'
    }));
  }

  public pairWithRobot(){
    this.robotService.sendMessage(JSON.stringify(<SendMessage>{
      Sender : localStorage.getItem('phoneno'),
      MessageType : 'PairRobotUIMessage',
      Message : '{}'
    }));
  }

  public unpairWithRobot(){
    var mess = <any>{
      UIId : localStorage.getItem('phoneno'),
      RobotId : localStorage.getItem('robotConnId')
    };
    console.log(JSON.stringify(<SendMessage>{
      Sender : localStorage.getItem('phoneno'),
      MessageType : 'UnPairRobotUIMessage',
      Message : mess
    }));
    this.robotService.sendMessage(JSON.stringify(<SendMessage>{
      Sender : localStorage.getItem('phoneno'),
      MessageType : 'UnPairRobotUIMessage',
      Message : JSON.stringify(mess)
    }));
  }
}

export class SendMessage{
  constructor(
    public Sender:string,
    public MessageType:string,
    public Message:string
  ) {}
}
