import { Injectable } from '@angular/core';
import { RobotService } from './robot.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpwraperService } from './httpwraper.service';

@Injectable()
export class UserService {
  baseWebApiUrl:string = environment.baseApiUrl;
  urlScheme:string = environment.urlscheme;
  private _tempSchedule:Schedule = new Schedule(null, null, null, null, null, null, null);
  private subTempSchedule = new Subject<Schedule>();
  tempScheduleResult$ = this.subTempSchedule.asObservable();
  
  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router,
    private httpClient:HttpwraperService) { }

  public isLoggedIn():boolean{
    return (localStorage.getItem('logintoken')!=null && localStorage.getItem('phoneno')!=null);
  }

  public setLoggedIn(logintoken:string, phoneno:string){
    localStorage.setItem('phoneno', phoneno);
    localStorage.setItem('logintoken', logintoken);
  }

  public logOut(){
    localStorage.removeItem('phoneno');
    localStorage.removeItem('logintoken');
    localStorage.removeItem('robotConnId');
    this.router.navigate(['/login']);
  }

  public login(username:string, password:string):Observable<any>{
    return this.httpClient.post<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Login`,{
      "Username":username,
      "Password":password
    });

  }

  public listusers():Observable<any>{
    return this.httpClient.get<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/User/List`);
  }

  public changepassword(user:User):Observable<any>{
    return this.httpClient.post<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/User/Changepassword`, user);
  }

  public createuser(user:User):Observable<any>{
    return this.httpClient.post<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/User/Newuser`, user); 
  }

  public listschedule(username:string):Observable<any>{
    return this.httpClient.get<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/List?username=${username}`);
  }

  public upsertschedule(schedule:Schedule):Observable<boolean>{
    return this.httpClient.post<boolean>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/Upsert`, schedule);
  }

  public deleteschedule(id:string):Observable<boolean>{
    return this.httpClient.post<boolean>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/Delete?id=${id}`,{
    });
  }

  public setTempSchedule(schedule:Schedule){
    this._tempSchedule = schedule;
  }

  public getTempSchedule(){
    return this._tempSchedule;
  }

  public uploadFile(base64Image:string){
    return this.httpClient.post<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/Upload`,{
      base64Image
    });
  }

  public getPhoto(id:string){
    return this.httpClient.get<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/Photo?id=${id}`);
  }

  public getContacts(userid:string){
    return this.httpClient.get<any>(`${this.urlScheme}://${this.baseWebApiUrl}/api/Schedule/GetContacts?userid=${userid}`);
  }
}

export class Schedule{
  constructor(
    public _id:string,
    public username:string,
    public contacts:string[],
    public chatMessage:string,
    public pathImages:string[],
    public willSendDate:Date,
    public isSent:boolean
  ) {}
}

export class User{
  constructor(
    public _id:string,
    public userName:string,
    public password:string,
    public loginToken:string
  ){}
}
