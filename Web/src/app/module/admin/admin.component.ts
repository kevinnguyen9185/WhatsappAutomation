import { Component, OnInit } from '@angular/core';
import { UserService, User } from 'src/app/core/services/user.service';
import { Subscription } from 'rxjs';
import { RobotService } from 'src/app/core/services/robot.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  users:any;
  pairlist = [];
  hideCreateUserDialog:boolean = true;
  private pairListSub:Subscription;
  constructor(private userService:UserService,
    private robotServide:RobotService) {

  }

  ngOnInit() {
    this.pairListSub = this.robotServide.pairListResult$.subscribe(result=>{
      this.pairlist = [];
      //console.log(result);
      JSON.parse(result).forEach(pairItem => {
        var userName = pairItem.SourceId;
        var robotId = pairItem.DestinationId;
        this.pairlist.push(<any>{ username: userName, robotid: robotId});
      });
    });
  }

  loadUsers():any{
    this.userService.listusers().subscribe(result=>{
      this.users = result.users;
      if(this.users){
        this.users.forEach(user => {
          var pairItem = this.pairlist.find(p=>p.username == user.username);
          if(pairItem){
            user.robotid = pairItem.robotid;
          }
        });
      }
    }, error=>{
      alert('You are not authorized to view this');
    });
  }

  openCreateUser(){
    this.hideCreateUserDialog = false;
  }

  createUser(username:string, password:string, confirmpassword:string){
    if(password==confirmpassword && password){
      var user = new User(null, username, password, null);
      this.userService.createuser(user).subscribe(result=>{
        alert('Create user successfully');
        this.loadUsers();
      }, error=>{
        alert("Can't create user");
      });
    } else {
      alert('Password must be confirmed');
    }
  }

  changePassword(user:any){
    var newpwd =(<HTMLInputElement>document.getElementById(`password${user.id}`)).value;
    var confirmpwd = (<HTMLInputElement>document.getElementById(`confirmpassword${user.id}`)).value;
    if(newpwd==confirmpwd && newpwd){
      user.password = newpwd;
      var updatedUser = new User(user.id, user.username, newpwd, user.loginToken);
      this.userService.changepassword(updatedUser).subscribe(result=>{
        alert('Password updated');
      }, error=>{
        alert("Can't change password");
      });
    } else {
      alert('Password must be confirmed');
    }
  }
}
