import { Component, OnInit } from '@angular/core';
import { Subscription, Observable, Subject } from 'rxjs';
import { Ng2ImgMaxService } from 'ng2-img-max';
import { RobotService } from 'src/app/core/services/robot.service';
import { UploadFile, UploadEvent, FileSystemFileEntry } from 'ngx-file-drop';
import { UserService, Schedule } from 'src/app/core/services/user.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material';
import { stringify } from 'querystring';
import { AngularDateTimePickerModule } from 'angular2-datetimepicker';

@Component({
  selector: 'app-chatsetup',
  templateUrl: './chatsetup.component.html',
  styleUrls: ['./chatsetup.component.css']
})
export class ChatsetupComponent implements OnInit {
  contacts = [];
  contactSub:Subscription;
  contactCheckedAll = false;
  chatMessage = '';
  imageUploads:ImageUpload[];
  imageFileUploaded: string[];
  date: Date = new Date();
  settings = {
      bigBanner: true,
      timePicker: true,
      format: 'dd-MMM-yyyy hh:mm a',
      defaultOpen: false
  }
  isSaving = false;

  constructor(private robotService: RobotService,
    private userService: UserService,
    private ng2ImgMax: Ng2ImgMaxService,
    public snackBar: MatSnackBar) {
      this.imageUploads = [];
      this.imageFileUploaded = [];
  }

  ngOnInit() {
  }

  ngAfterViewInit() {

  }

  public loadInfoWhenEdit(){
    var schedule = this.userService.getTempSchedule();
    this.loadImages(schedule);
  }

  public saveSchedule():Observable<boolean>{
    var subj = new Subject<boolean>();
    if(this.chatMessage.trim().length==0) {
      alert('You must type something to send');
      subj.next(false);
      return subj.asObservable();
    }
    if(this.date <= new Date()) {
      alert('The date must be in future');
      subj.next(false);
      return subj.asObservable();
    }
    this.isSaving=true;
    return this.uploadImage();
  }

  loadImages(schedule:any){
    this.imageUploads = [];
    if(schedule.photos){
      schedule.photos.forEach(photo=>{
        this.imageUploads.push(photo.content);
      });
    }
  }

  private uploadImage():Observable<boolean>{
    var subj = new Subject<boolean>();
    var img = this.imageUploads.pop();
    if(img){
      var imgContent = img.Content.split(',').pop();
      this.userService.uploadFile(imgContent).subscribe(result=>{
        this.imageFileUploaded.push(result.imagePath);
        this.uploadImage();
      },
      (err:HttpErrorResponse)=>{
        subj.next(false);
        return subj.asObservable();
      });
    } else {
      this.userService.getTempSchedule().chatMessage = this.chatMessage;
      this.userService.getTempSchedule().willSendDate = new Date(this.date.toLocaleString());
      this.userService.getTempSchedule().pathImages = this.imageFileUploaded;
      this.userService.getTempSchedule().isSent = false;
      this.userService.upsertschedule(this.userService.getTempSchedule()).subscribe(result=>{
        if(result){
          this.isSaving=false;
          subj.next(true);
          return subj.asObservable();
        }
      });
    }
    return subj.asObservable();
  }

  public files: UploadFile[] = [];
 
  public dropped(event: UploadEvent) {
    this.files = event.files;
    var self = this;
    for (const droppedFile of event.files) {
      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          //Compress the file first
          var compressedImage:File;
          this.ng2ImgMax.compressImage(file, 0.075).subscribe(result =>{
            let reader = new FileReader();

            reader.readAsDataURL(result);
            
            reader.onloadend = function() {
              var base64data = reader.result.toString();
              var imgUpload = new ImageUpload(base64data, file.name);
              if (self.imageUploads.length>=10){
                alert('Cannot upload more than 10 files');
                return;
              }
              self.imageUploads.push(imgUpload);
            }
          });
        });
      }
    }
  }

  removeImage(path:string){
    this.imageUploads.forEach((item,index)=>{
      if(item.Path.toLowerCase()==path.toLocaleLowerCase()){
        this.imageUploads.splice(index,1);
      }
    });
  }
 
  public fileOver(event){
    //console.log(event);
  }
 
  public fileLeave(event){
    //console.log(event);
  }
}

export class Contact{
  constructor(
    public Contact:string,
    public Checked:boolean
  ){}
}

export class SendChatMessage{
  constructor(
    public ContactName:string,
    public ChatMessage:string,
    public ImagePaths:string[]
  ){}
}

export class ImageUpload{
  constructor(
    public Content:string,
    public Path:string
  ){}
}