import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  constructor() { }

  registerToggle() {
    this.registerMode = true;
  }

  cancelRegistration(registerMode: boolean) {
    this.registerMode = registerMode;
  }
  ngOnInit() {
  }

}