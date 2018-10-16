import { Component, OnInit } from '@angular/core';
import { RobotService } from '../../core/services/robot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [RobotService]
})
export class LoginComponent implements OnInit {

  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router, 
    private userService: UserService){
    
  }

  doLogin(phoneNo: string) {
    this.robotService.getInstanceStatus(phoneNo).subscribe((result)=>{
      if(result.data == 'ok'){
        this.userService.setLoggedIn(true, phoneNo);
        this.router.navigate(['/chatmain']);
      } else {
        this.userService.setLoggedIn(false, phoneNo);
        alert('Login failed');
      }
    });
  }

  ngOnInit() {
  }

}
