import { Component, OnInit, ViewChild } from '@angular/core';
import { SelectcontactComponent } from './selectcontact/selectcontact.component';
import { UserService } from 'src/app/core/services/user.service';
import { MatSnackBar } from '@angular/material';
import { ChatsetupComponent } from './chatsetup/chatsetup.component';
import { Content } from '@angular/compiler/src/render3/r3_ast';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private step:number = 0;
  butProccedScheduleText:string = 'Create schedule';
  butProccedScheduleIcon:string = 'navigate_next';
  disableButtonProcess:boolean = false;
  private schedules:any[] = [];
  @ViewChild(SelectcontactComponent) selectcontactComponent:SelectcontactComponent;
  @ViewChild(ChatsetupComponent) chatsetupComponent:ChatsetupComponent;

  constructor(private userService: UserService,
    public snackBar:MatSnackBar) { 
    this.userService.getTempSchedule().Username = localStorage.getItem('phoneno');
    this.loadListSchedule();
  }

  private loadListSchedule(){
    this.schedules = [];
    this.userService.listschedule(localStorage.getItem('phoneno')).subscribe(result=>{
      result.schedules.forEach(s => {
        var schedule:any = {};
        schedule._id = s._id;
        schedule.username = s.username;
        schedule.contacts = s.contacts;
        schedule.chatMessage = s.chatMessage;
        schedule.issent = s.isSent;
        schedule.photos = [];
        if (s.pathImages){
          s.pathImages.forEach(p => {
            var photo = <Photo>{
              path : p,
              content : ''
            }
            schedule.photos.push(photo);
          });
        }
        schedule.willSendDate = new Date(s.willSendDate).toLocaleDateString('en-US', {weekday:'long', year:'numeric', month:'long', day:'numeric', hour:'numeric'});
        this.schedules.push(schedule);
      });
      if(this.schedules.length>=5){
        this.disableButtonProcess = true;
      } else {
        this.disableButtonProcess = false;
      }
    });
  }

  ngOnInit() {
  }

  proceedSchedule() {
    this.step++;
    switch(this.step){
      case 0:
        this.butProccedScheduleIcon = 'navigate_next';
        this.butProccedScheduleText = 'Create schedule';
        break;
      case 1:
        this.butProccedScheduleIcon = 'navigate_next';  
        this.butProccedScheduleText = 'Setup message and photos';
        break;
      case 2:
        var tempSchedule = this.userService.getTempSchedule();
        if(tempSchedule.Contacts && tempSchedule.Contacts.length>0){
          this.butProccedScheduleIcon = 'save';
          this.butProccedScheduleText = 'SAVE SCHEDULE'
        } else{
          alert('Please select some contacts');
          this.step--;
        }
        break;
      case 3:
        this.chatsetupComponent.saveSchedule().subscribe(result=>{
          if(result){
            this.step = 0;
            this.butProccedScheduleIcon = 'navigate_next';
            this.butProccedScheduleText = 'Create schedule';
            this.loadListSchedule();
          } else {
            this.step--;
          }
        });
        break;
    }
  }

  loadImages(schedule:any){
    if(schedule.photos){
      schedule.photos.forEach(photo=>{
        if(!photo.content){
          this.userService.getPhoto(photo.path).subscribe(data=>{
            photo.content = `data:image/png;base64,${data.imageBase64}`;
          });
        }
      });
    }
  }

  proceedScheduleBack(){
    this.step--;
  }

  runSchedule(id:string){
    if(confirm(`This will run ${id}`)){
      //do something
    }
  }

  deleteSchedule(id:string){
    if(confirm(`This will delete ${id}`)){
      //do something
      this.userService.deleteschedule(id).subscribe(result=>{
        if(result){
          this.snackBar.open('Delete successfully', '', {
            duration:5000
          });
          this.loadListSchedule();
        }
      });
    }
  }
}

export class Photo {
  path:string;
  content:string;
}