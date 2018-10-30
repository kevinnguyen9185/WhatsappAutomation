import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { RobotService } from 'src/app/core/services/robot.service';
import { Subscription } from 'rxjs';
import { UserService, Schedule } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-selectcontact',
  templateUrl: './selectcontact.component.html',
  styleUrls: ['./selectcontact.component.css']
})
export class SelectcontactComponent implements OnInit, OnDestroy {
  recentcontacts = [];
  allcontacts = [];
  selectedcontacts = [];
  contactSub:Subscription;
  tempScheduleSub:Subscription;

  constructor(private robotService:RobotService,
    private userService:UserService) { 
  }

  ngOnInit() {

  }

  public getContactList(){
    this.contactSub = this.robotService.getContactList().subscribe(result=>{
      if(result.isGetall){
        result.map((v)=>{
          this.allcontacts.push(v);
        });
      } else {
        result.map((v)=>{
          this.recentcontacts.push(v);
        });
      }
    })
  }

  selectAllRecentContacts(){
    
  }

  refreshContactList(){
    this.robotService.getContactList(true);
  }

  removeselected(contact){
    var idx = this.selectedcontacts.indexOf(contact);
    this.selectedcontacts.splice(idx,1);
    this.recentcontacts.push(contact);
    this.userService.getTempSchedule().Contacts = this.selectedcontacts;
  }

  selectcontact(contact){
    var idx = this.recentcontacts.indexOf(contact);
    this.recentcontacts.splice(idx,1);
    this.selectedcontacts.push(contact);
    this.userService.getTempSchedule().Contacts = this.selectedcontacts;
  }

  ngAfterViewInit() {
    this.getContactList();
  }

  ngOnDestroy(){
   this.contactSub.unsubscribe(); 
  }
}

export class Contact{
  constructor(
    public Contact:string
  ){}
}
