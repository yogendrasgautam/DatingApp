import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  constructor(private authService: AuthService) { }

  registerToggle() {
    this.registerMode = true;
  }

  cancelRegistration(registerMode: boolean) {
    this.registerMode = registerMode;
  }
  
  isLoggedIn(){
    return this.authService.loggedIn();
  }
  ngOnInit() {
  }

}
