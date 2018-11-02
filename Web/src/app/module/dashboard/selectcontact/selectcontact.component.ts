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
  allcontacts = [];
  selectedcontacts = [];
  contactSub:Subscription;
  tempScheduleSub:Subscription;

  constructor(private robotService:RobotService,
    private userService:UserService) { 
  }

  ngOnInit() {
    this.userService.getContacts(localStorage.getItem('phoneno')).subscribe(result=>{
      //console.log(result);
      result.map((v)=>{
        var foundcontact = this.allcontacts.find(c=>c.contact == v.contactName);
        if(!foundcontact){
          this.allcontacts.push(<Contact>{
            contact : v.contactName,
            isrecent : v.isRecent
          });
        } else {
          foundcontact.isrecent = v.isRecent;
        }
      });
    });
  }

  public getContactList(){
    this.contactSub = this.robotService.getContactList().subscribe(result=>{
      if(result.isGetall){
        result.map((v)=>{
          this.allcontacts.push(v);
        });
      }
    });
  }

  selectAllRecentContacts(){
    var arr = [];
    this.allcontacts.forEach(c=>{
      if(c.isrecent){
        arr.push(c.contact);
      }
    });
    arr.forEach(c=>{
      this.selectcontact(c);
    });
  }

  refreshContactList(){
    this.robotService.getContactList(true);
  }

  removeselected(contactName){
    var idx = this.selectedcontacts.findIndex(c=>c.contact==contactName);
    var contact = this.selectedcontacts.find(c=>c.contact==contactName);
    this.selectedcontacts.splice(idx,1);
    this.allcontacts.push(contact);
    this.userService.getTempSchedule().Contacts = [];
    this.selectedcontacts.forEach(c=>{
      this.userService.getTempSchedule().Contacts.push(c.contact);
    });
    
  }

  selectcontact(contactName){
    var idx = this.allcontacts.findIndex(c=>c.contact==contactName);
    var contact = this.allcontacts.find(c=>c.contact==contactName);
    this.allcontacts.splice(idx,1);
    this.selectedcontacts.push(contact);
    this.userService.getTempSchedule().Contacts = [];
    this.selectedcontacts.forEach(c=>{
      this.userService.getTempSchedule().Contacts.push(c.contact);
    });
  }

  ngAfterViewInit() {
    if(!this.robotService.LoadedRecentContact){
      this.getContactList();
      this.robotService.LoadedRecentContact = true;
    }
  }

  ngOnDestroy(){
    if(this.contactSub){
      this.contactSub.unsubscribe(); 
    }
  }
}

export class Contact {
  contact:string;
  isrecent:boolean;
}
