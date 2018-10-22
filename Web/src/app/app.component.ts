import { Component, OnDestroy } from '@angular/core';
import { UserService } from './core/services/user.service';
import { Router } from '@angular/router';
import { RobotService } from './core/services/robot.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [UserService]
})
export class AppComponent implements OnDestroy {
  title = 'Whatsapp Automation';
  private isWsCloseSub: Subscription;

  constructor(private userService:UserService,
    private robotService:RobotService,
    private router: Router) {
    this.isWsCloseSub = this.robotService.IsWsCloseResult$.subscribe(result => {
      if(result){
        userService.logOut();
      }
    });  
    if (!userService.isLoggedIn()){
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/chatmain']);
    }
  }

  ngOnDestroy(){
    this.isWsCloseSub.unsubscribe();
  }
}