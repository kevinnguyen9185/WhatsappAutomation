import { Component, OnInit } from '@angular/core';
import { RobotService } from 'src/app/core/services/robot.service';

@Component({
  selector: 'app-chatmain',
  templateUrl: './chatmain.component.html',
  styleUrls: ['./chatmain.component.css']
})
export class ChatmainComponent implements OnInit {

  constructor(private robotService: RobotService) {
    this.robotService.getInstanceStatus(localStorage.getItem('phoneno')).subscribe(result=>{
      console.log(result.data);
      if (result.data == ''){

      }
    });
  }

  ngOnInit() {
  }

  public getQRCode(){
    this.robotService.sendMessage(JSON.stringify(<SendMessage>{
      Sender : localStorage.getItem('phoneno'),
      MessageType : 'GetQRCode',
      Message : '{}'
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
