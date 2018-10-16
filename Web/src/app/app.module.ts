import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { MatFormFieldModule, MatSelectModule, MatInputModule, MatButtonModule } from '@angular/material';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './module/login/login.component';
import { ChatmainComponent } from './module/chatmain/chatmain.component';

const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'chatmain',      component: ChatmainComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChatmainComponent
  ],
  imports: [
    BrowserModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    BrowserAnimationsModule,
    MatButtonModule,
    RouterModule.forRoot(
      appRoutes
    )
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
