import { Component, OnInit, NgModule } from '@angular/core';
import { RobotService } from 'src/app/core/services/robot.service';
import { Subscription } from 'rxjs';
import { MatCheckboxChange } from '@angular/material';
import { delay } from 'q';
import { UploadEvent, UploadFile, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Ng2ImgMaxService } from 'ng2-img-max';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  contacts = [];
  contactSub:Subscription;
  contactCheckedAll = false;
  chatMessage = '';
  imageUploads:ImageUpload[];

  constructor(private robotService:RobotService,
    private ng2ImgMax: Ng2ImgMaxService) { 
    this.imageUploads = [];
  }

  ngOnInit() {
  }

  public getContactList(){
    this.contactSub = this.robotService.getContactList().subscribe(result=>{
      result.map((v)=>{
        this.contacts.push(new Contact(v, true));
      });
    })
  }

  public selectAllChange(evt:MatCheckboxChange){
    this.contacts.forEach(c=>c.Checked = evt.checked);
  }

  public sendMessage(){
    this.contacts.forEach(c=>{
      if(c.Checked){
        delay(1000).then(()=>{
          var imgs = [];
          this.imageUploads.forEach((v,i)=>{
            imgs.push(v.Content.split(',').pop());
          });
          var sendChatMess = new SendChatMessage(c.Contact, this.chatMessage, imgs);
          this.robotService.sendMessage(sendChatMess, 'SendChatMessage')
        });
      }
    });
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
    console.log(event);
  }
 
  public fileLeave(event){
    console.log(event);
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