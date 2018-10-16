import { Injectable } from '@angular/core';
import { RobotService } from './robot.service';

@Injectable()
export class UserService {

  constructor(private robotService: RobotService) { }

  public isLoggedIn():boolean{
    return Boolean(localStorage.getItem('isLoggedIn'));
  }

  public setLoggedIn(value:boolean, phoneno:string){
    localStorage.setItem('phoneno', phoneno);
    localStorage.setItem('isLoggedIn', String(value));
  }
}
