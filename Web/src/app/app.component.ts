import { Component } from '@angular/core';
import { UserService } from './core/services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [UserService]
})
export class AppComponent {
  title = 'Whatsapp Automation';

  constructor(private userService:UserService,
    private router: Router) {
    if (!userService.isLoggedIn()){
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/chatmain']);
    }
  }
}