import { Component,Injectable } from '@angular/core';
import { Observable, fromEvent, Subject } from 'rxjs';
import { UserService } from './user.service';
import { delay } from 'q';

@Injectable({
  providedIn: 'root'
})
export class RobotService {
  private actionUrl: string;
  private headers: Headers;
  private static websocket: WebSocket;
  private receivedMsg: any;
  private static loginResultSubject = new Subject<boolean>();
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

  public sendMessage(text:string){
    RobotService.websocket.send(text);
  }

  public connectWs(uiId: string){
    if (RobotService.websocket==null){
      RobotService.websocket = new WebSocket(`ws://localhost:5000/ServerSocket?connId=${uiId}`);
    } else {
      RobotService.isOpenSubject.next(true);
    }
    RobotService.websocket.onopen = ()=>{
      RobotService.isOpenSubject.next(true);
    };
    RobotService.websocket.onmessage = RobotService.processMessage;
    RobotService.websocket.onclose = ()=>{
      RobotService.isWsCloseSubject.next(true);
      alert('You are connecting from different browser. This will be disconnected');
    };
  }

  public getInstanceStatus(uiId: string): Observable<MessageEvent>{
    if (!RobotService.websocket){
      this.connectWs(uiId);
    }
    return fromEvent<MessageEvent>(RobotService.websocket, 'message');
  }

  static processMessage(event:MessageEvent){
    var mess = JSON.parse(event.data);
    switch(mess.MessageType){
      case "LoginResponseMessage":
        if(mess.Message == '"ok"'){
          RobotService.loginResultSubject.next(true);
        } else {
          RobotService.loginResultSubject.next(false);          
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

  public doLogin(phoneNo:string){
    this.connectWs(phoneNo);
  }
}
