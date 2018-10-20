import { Injectable } from '@angular/core';
import { RobotService } from './robot.service';
import { Router, ActivatedRoute } from '@angular/router';

@Injectable()
export class UserService {

  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router) { }

  public isLoggedIn():boolean{
    return Boolean(localStorage.getItem('isLoggedIn'));
  }

  public setLoggedIn(value:boolean, phoneno:string){
    localStorage.setItem('phoneno', phoneno);
    localStorage.setItem('isLoggedIn', String(value));
  }

  public logOut(){
    localStorage.removeItem('phoneno');
    localStorage.removeItem('isLoggedIn');
    localStorage.removeItem('robotConnId');
    this.router.navigate(['/login']);
  }
}
