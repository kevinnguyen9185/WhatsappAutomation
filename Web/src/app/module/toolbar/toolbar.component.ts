import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent implements OnInit {
  phoneNo = '';

  constructor(private userService:UserService) { }

  ngOnInit() {
    if(this.userService.isLoggedIn()){
      this.phoneNo = localStorage.getItem('phoneno');
    }
  }

  logout(){
    this.userService.logOut();
  }

}
