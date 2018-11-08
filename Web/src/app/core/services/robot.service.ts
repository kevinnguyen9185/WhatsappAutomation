import { Component,Injectable } from '@angular/core';
import { Observable, fromEvent, Subject } from 'rxjs';
import { UserService } from './user.service';
import { delay, send } from 'q';
import { environment } from 'src/environments/environment.prod';

@Injectable({
  providedIn: 'root'
})
export class RobotService {
  private actionUrl: string;
  private headers: Headers;
  private static websocket: WebSocket;
  static isLoadedRecentContact:boolean = false;
  private receivedMsg: any;
  private static loginResultSubject = new Subject<string>();
  LoginResult$ = RobotService.loginResultSubject.asObservable();
  private static qrImageSubject = new Subject<string>();
  QrImageResult$ = RobotService.qrImageSubject.asObservable();
  private static pairRobotResultSubject = new Subject<string>();
  PairRobotResult$ = RobotService.pairRobotResultSubject.asObservable();
  private static isOrphanedSubject = new Subject<boolean>();
  IsOrphanedResult$ = RobotService.isOrphanedSubject.asObservable();
  private static isOpenSubject = new Subject<boolean>();
  IsOpenResult$ = RobotService.isOpenSubject.asObservable();
  private static robotLoginStatusSubject = new Subject<string>();
  RobotLoginStatusResult$ = RobotService.robotLoginStatusSubject.asObservable();
  private static isWsCloseSubject = new Subject<boolean>();
  IsWsCloseResult$ = RobotService.isWsCloseSubject.asObservable();
  private static contactListSubject = new Subject<string[]>();
  contactListResult$ = RobotService.contactListSubject.asObservable();

  constructor() {
    
  }

  public get LoadedRecentContact() : boolean {
    return RobotService.isLoadedRecentContact;
  }
  public set LoadedRecentContact(v : boolean) {
    RobotService.isLoadedRecentContact = v;
  }
  

  public getContactList(isGetall:boolean=false):Observable<any>{
    this.sendMessage(<any>{IsGetall:isGetall}, 'ContactListMessage');
    return this.contactListResult$;
  }

  public sendMessage(mess:any, messtype:string, sender:string=''){
    var text = JSON.stringify(<SendMessage>{
      Sender : sender==''?localStorage.getItem('phoneno'):sender,
      LoginToken: localStorage.getItem('logintoken'),
      MessageType : messtype,
      Message : JSON.stringify(mess)
    })
    RobotService.websocket.send(text);
  }

  public connectWs(uiId: string, securestring:string, type='password'){
    if (RobotService.websocket==null){
      RobotService.websocket = new WebSocket(`${environment.urlWsScheme}://${environment.baseWsurl}/ServerSocket?connId=${localStorage.getItem('phoneno')}&logintoken=${localStorage.getItem('logintoken')}`);
    } else {
      RobotService.isOpenSubject.next(true);
    }
    RobotService.websocket.onopen = ()=>{
      RobotService.isOpenSubject.next(true);
    };
    RobotService.websocket.onmessage = RobotService.processMessage;
    RobotService.websocket.onclose = ()=>{
      RobotService.isWsCloseSubject.next(true);
      alert('You are either disconnected or connecting from different browser. This will be disconnected');
    };
  }

  static processMessage(event:MessageEvent){
    var mess = JSON.parse(event.data);
    switch(mess.MessageType){
      case "LoginResponseMessage":
        var loginResponse = JSON.parse(mess.Message);
        if(loginResponse.LoginToken){
          RobotService.loginResultSubject.next(loginResponse.LoginToken);
        } else {
          RobotService.loginResultSubject.next("");          
        }
        break;
      case "LoginStatusResponseMessage":
        var status = JSON.parse(mess.Message).Status;
        RobotService.robotLoginStatusSubject.next(status);
        break;
      case "PairRobotUIResponseMessage":
        var robotConnId = JSON.parse(mess.Message).RobotConnId;
        localStorage.setItem('robotConnId', robotConnId);
        RobotService.pairRobotResultSubject.next(robotConnId);
        break;
      case "ErrorMessage":
        var errMess = JSON.parse(mess.Message);
        RobotService.processErrorMessage(errMess.Message);
        break;
      case "GetQRCodeResponseMessage":
        var qrObj = JSON.parse(mess.Message);
        RobotService.qrImageSubject.next(qrObj.QRCodeBase64);
        break;
      case "ContactListResponseMessage":
        var contactListObj = JSON.parse(mess.Message);
        RobotService.contactListSubject.next(contactListObj.Contacts);
        break;
      default:
        break;
    }
  }

  static processErrorMessage(mess:string){
    switch (mess){
      case "orphaned":
        //this.userService.logOut();
        RobotService.isOrphanedSubject.next(true);
        break;
      case "pair failed":
        RobotService.pairRobotResultSubject.next("");
        break;
      default:
        break;
    }
  }
}

export class SendMessage{
  constructor(
    public Sender:string,
    public MessageType:string,
    public Message:string,
    public LoginToken:string
  ) {}
}