import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from "@angular/flex-layout";

import { AppComponent } from './app.component';
import { MatFormFieldModule, MatCardModule, MatInputModule, MatButtonModule, MatProgressSpinnerModule, MatToolbarModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressBarModule, MatDividerModule, MatExpansionModule, MatChipsModule, MatSnackBarModule } from '@angular/material';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './module/login/login.component';
import { ToolbarComponent } from './module/toolbar/toolbar.component';
import { ChatmainComponent } from './module/chatmain/chatmain.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { QrcodeComponent } from './module/qrcode/qrcode.component';
import { ChatComponent } from './module/chat/chat.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileDropModule } from 'ngx-file-drop';
import { Ng2ImgMaxModule } from 'ng2-img-max';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { DashboardComponent } from './module/dashboard/dashboard.component';
import { SelectcontactComponent } from './module/dashboard/selectcontact/selectcontact.component';
import { ChatsetupComponent } from './module/dashboard/chatsetup/chatsetup.component';
import { AngularDateTimePickerModule } from 'angular2-datetimepicker';
import { AdminComponent } from './module/admin/admin.component';

const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'chatmain',      component: ChatmainComponent },
  { path: 'admin',  component: AdminComponent}
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChatmainComponent,
    ToolbarComponent,
    QrcodeComponent,
    ChatComponent,
    DashboardComponent,
    SelectcontactComponent,
    ChatsetupComponent,
    AdminComponent
  ],
  imports: [
    BrowserModule,
    FlexLayoutModule,
    MatFormFieldModule,
    MatCardModule,
    MatChipsModule,
    MatSnackBarModule,
    MatInputModule,
    MatExpansionModule,
    MatDividerModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    NgxSpinnerModule,
    MatMenuModule,
    MatIconModule,
    MatCheckboxModule,
    FormsModule,
    ReactiveFormsModule,
    FileDropModule,
    Ng2ImgMaxModule,
    HttpClientModule,
    AngularDateTimePickerModule,
    MatProgressBarModule,
    RouterModule.forRoot(
      appRoutes, {useHash: true}
    )
  ],
  providers: [HttpClient],
  bootstrap: [AppComponent]
})
export class AppModule { }
