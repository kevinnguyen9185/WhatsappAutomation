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
  imageToUploads:ImageUpload[];
  imageFileUploaded: string[];
  imageToProcess:ImageUpload[];
  date: Date = new Date();
  settings = {
      bigBanner: true,
      timePicker: true,
      format: 'dd-MMM-yyyy hh:mm a',
      defaultOpen: false
  }
  isSaving = false;
  beforeChangeSchedule:Schedule = null;
  private subjectSavingStatus:Subject<boolean> = new Subject<boolean>();
  $resultSavingStatus:Observable<boolean> = this.subjectSavingStatus.asObservable();

  constructor(private robotService: RobotService,
    private userService: UserService,
    private ng2ImgMax: Ng2ImgMaxService,
    public snackBar: MatSnackBar) {
      this.imageToUploads = [];
      this.imageFileUploaded = [];
      this.imageToProcess = [];
  }

  ngOnInit() {
  }

  ngAfterViewInit() {

  }

  public loadInfoWhenEdit(){
    var schedule = this.userService.getTempSchedule();
    if(schedule._id){
      this.beforeChangeSchedule = JSON.parse(JSON.stringify(schedule));
      this.loadImages(schedule);
      this.chatMessage = schedule.chatMessage;
      this.date = new Date(schedule.willSendDate);
    }
  }

  public saveSchedule(){
    var subj = new Subject<boolean>();
    if(this.chatMessage.trim().length==0) {
      alert('You must type something to send');
      return;
    }
    if(this.date <= new Date()) {
      alert('The date must be in future');
      return;
    }
    this.isSaving=true;
    this.imageToProcess = JSON.parse(JSON.stringify(this.imageToUploads));
    this.uploadImage();
  }

  loadImages(schedule:any){
    this.imageToUploads = [];
    if(schedule.photos){
      schedule.photos.forEach(photo=>{
        //this.imageUploads.push(photo.content);
        if(!photo.content){
          this.userService.getPhoto(photo.path).subscribe(data=>{
            photo.content = `data:image/png;base64,${data.imageBase64}`;
            this.imageToUploads.push(new ImageUpload(photo.content, photo.path));
          });
        } else {
          this.imageToUploads.push(new ImageUpload(photo.content, photo.path));
        }
      });
    }
  }

  private uploadImage(){
    var img = this.imageToProcess.pop();
    if(img){
      if(!img.Path){
        var imgContent = img.Content.split(',').pop();
        this.userService.uploadFile(imgContent).subscribe(result=>{
          this.insertUniqueImageUploaded(result.imagePath);
          this.uploadImage();
        },
        (err:HttpErrorResponse)=>{
          alert('error while uploading. Please try again later');
        });
      } else {
        this.insertUniqueImageUploaded(img.Path);
        this.uploadImage();
      }
    } else {
      this.userService.getTempSchedule().chatMessage = this.chatMessage;
      this.userService.getTempSchedule().willSendDate = new Date(this.date.toString());
      this.userService.getTempSchedule().pathImages = this.imageFileUploaded;
      this.userService.getTempSchedule().isSent = false;
      this.userService.upsertschedule(this.userService.getTempSchedule()).subscribe(result=>{
        if(result){
          this.isSaving=false;
          this.subjectSavingStatus.next(true);
        }
      }, error=>{
        this.isSaving=false;
        this.subjectSavingStatus.next(false);
      });
    }
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
              var imgUpload = new ImageUpload(base64data, null);
              if (self.imageToUploads.length>=10){
                alert('Cannot upload more than 10 files');
                return;
              }
              self.imageToUploads.push(imgUpload);
            }
          });
        });
      }
    }
  }

  removeImage(path:string){
    this.imageFileUploaded.splice(this.imageFileUploaded.findIndex(i=>i==path),1);
    this.imageToUploads.splice(this.imageToUploads.findIndex(i=>i.Path == path),1);
    // this.imageToUploads.forEach((item,index)=>{
    //   if(item.Path.toLowerCase()==path.toLocaleLowerCase()){
    //     this.imageToUploads.splice(index,1);
    //   }
    // });
  }

  insertUniqueImageUploaded(value:string){
    if(!this.imageFileUploaded.find(r=>r==value)){
      this.imageFileUploaded.push(value);
    }
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