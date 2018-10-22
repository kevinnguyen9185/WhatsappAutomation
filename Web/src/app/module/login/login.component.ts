import { Component, OnInit, OnDestroy } from '@angular/core';
import { RobotService } from '../../core/services/robot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/core/services/user.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [RobotService]
})
export class LoginComponent implements OnInit, OnDestroy {
  loginResultSub:Subscription;
  isLoginSub:Subscription;
  phoneNumber:string;

  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router, 
    private userService: UserService){
    this.loginResultSub = this.robotService.LoginResult$.subscribe(result => {
      if(result){
        this.userService.setLoggedIn(true, this.phoneNumber);
        this.router.navigate(['/chatmain']);
      } else {
        this.userService.setLoggedIn(false, this.phoneNumber);
        alert('Login failed');
      }
    });
  }

  doLogin(phoneNo: string) {
    this.phoneNumber = phoneNo;
    this.robotService.doLogin(phoneNo);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.loginResultSub.unsubscribe();
  }

}
