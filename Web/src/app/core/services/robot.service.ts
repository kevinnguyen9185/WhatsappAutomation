import { Component,Injectable } from '@angular/core';
import { Observable, fromEvent } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RobotService {
  private actionUrl: string;
  private headers: Headers;
  private static websocket: WebSocket;
  private receivedMsg: any;
  
  public sendMessage(text:string){
    RobotService.websocket.send(text);
  }

  public connectWs(uiId: string){
    RobotService.websocket = new WebSocket(`ws://localhost:5000/UISocket?UIId=${uiId}`);
  }

  public getInstanceStatus(uiId: string): Observable<MessageEvent>{
    if (!RobotService.websocket){
      this.connectWs(uiId);
    }
    return fromEvent<MessageEvent>(RobotService.websocket, 'message');
  }
}
