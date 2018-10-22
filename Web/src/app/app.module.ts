import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from "@angular/flex-layout";

import { AppComponent } from './app.component';
import { MatFormFieldModule, MatCardModule, MatInputModule, MatButtonModule, MatProgressSpinnerModule, MatToolbarModule, MatMenuModule, MatIconModule, MatCheckboxModule } from '@angular/material';
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

const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'chatmain',      component: ChatmainComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChatmainComponent,
    ToolbarComponent,
    QrcodeComponent,
    ChatComponent
  ],
  imports: [
    BrowserModule,
    FlexLayoutModule,
    MatFormFieldModule,
    MatCardModule,
    MatInputModule,
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
    RouterModule.forRoot(
      appRoutes
    )
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
