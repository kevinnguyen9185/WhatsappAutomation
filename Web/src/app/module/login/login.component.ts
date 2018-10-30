import { Component, OnInit, OnDestroy } from '@angular/core';
import { RobotService } from '../../core/services/robot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/core/services/user.service';
import { Subscription } from 'rxjs';
import { delay } from 'q';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [RobotService]
})
export class LoginComponent implements OnInit, OnDestroy {
  isOpenResultSub:Subscription;
  phoneNumber:string;
  password:string;

  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router, 
    private userService: UserService){

    this.isOpenResultSub = this.robotService.IsOpenResult$.subscribe(result => {
      var self = this;
      if(result){
        //Do real login
        var mess = <any>{
          UserName : self.phoneNumber,
          Password : self.password
        };
        delay(1000).then(()=>{
          this.robotService.sendMessage(mess, 'LoginMessage', self.phoneNumber);
        });
      } else {
        alert('No connection to server, please login');
      }
    });
  }

  doLogin(phoneNo: string, password: string) {
    this.phoneNumber = phoneNo;
    this.password = password;
    this.userService.login(phoneNo, password).subscribe(result => {
      if(result && result.loginToken){
        this.userService.setLoggedIn(result.loginToken, this.phoneNumber);
        this.robotService.connectWs(this.phoneNumber, result.loginToken, 'web');
        this.router.navigate(['/chatmain']);
      } else {
        alert('Login failed');
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.isOpenResultSub.unsubscribe();
  }

}

export class SendMessage{
  constructor(
    public Sender:string,
    public MessageType:string,
    public Message:string,
  ) {}
}