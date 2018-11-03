import { Injectable } from '@angular/core';
import { RobotService } from './robot.service';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class UserService {
  baseWebApiUrl:string = 'localhost:5551';
  private _tempSchedule:Schedule = new Schedule(null, null, null, null, null, null, null);
  private subTempSchedule = new Subject<Schedule>();
  tempScheduleResult$ = this.subTempSchedule.asObservable();
  
  constructor(private robotService: RobotService,
    private route: ActivatedRoute, 
    private router: Router,
    private httpClient:HttpClient) { }

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
    return this.httpClient.post<any>(`http://${this.baseWebApiUrl}/api/Login`,{
      "Username":username,
      "Password":password
    });
  }

  public listschedule(username:string):Observable<any>{
    return this.httpClient.get<any>(`http://${this.baseWebApiUrl}/api/Schedule/List?username=${username}`);
  }

  public upsertschedule(schedule:Schedule):Observable<boolean>{
    return this.httpClient.post<boolean>(`http://${this.baseWebApiUrl}/api/Schedule/Upsert`, schedule);
  }

  public deleteschedule(id:string):Observable<boolean>{
    return this.httpClient.post<boolean>(`http://${this.baseWebApiUrl}/api/Schedule/Delete?id=${id}`,{
    });
  }

  public setTempSchedule(schedule:Schedule){
    this._tempSchedule = schedule;
  }

  public getTempSchedule(){
    return this._tempSchedule;
  }

  public uploadFile(base64Image:string){
    return this.httpClient.post<any>(`http://${this.baseWebApiUrl}/api/Schedule/Upload`,{
      base64Image
    });
  }

  public getPhoto(id:string){
    return this.httpClient.get<any>(`http://${this.baseWebApiUrl}/api/Schedule/Photo?id=${id}`);
  }

  public getContacts(userid:string){
    return this.httpClient.get<any>(`http://${this.baseWebApiUrl}/api/Schedule/GetContacts?userid=${userid}`);
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
