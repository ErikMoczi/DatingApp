import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {HttpClientModule} from '@angular/common/http';
import {NavComponent} from './nav/nav.component';
import {FormsModule} from '@angular/forms';
import {AuthService} from './_services/auth.service';
import {HomeComponent} from './home/home.component';
import {RegisterComponent} from './register/register.component';
import {ErrorInterceptorProvider} from './_services/error.interceptor';
import {AlertifyjsService} from './_services/alertifyjs.service';
import {BsDropdownModule} from 'ngx-bootstrap';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot()
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyjsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
