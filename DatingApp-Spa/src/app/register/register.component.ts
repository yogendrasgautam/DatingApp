import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { Router } from '@angular/router';
import { User } from '../_models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: User;
  registerForm: FormGroup;
  bsDateConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private alertify: AlertifyService, 
              private router: Router, private fb: FormBuilder) { }

  register() {
    // this.authService.register(this.model).subscribe(() => {
    //   this.alertify.success('Registration Successfull');
    // }, error => {
    //   this.alertify.error(error);
    // });
    //console.log(this.registerForm.value);
    if(this.registerForm.valid) {
      this.model = Object.assign({}, this.registerForm.value);
      this.authService.register(this.model).subscribe(() => {
      this.alertify.success('Registration Successfull');
      }, error => {
      this.alertify.error(error);
      }, () => this.authService.login(this.model).subscribe( () => {
        this.router.navigate(['/members']);
      }));
    }

  }

  cancel() {
    this.cancelRegister.emit(false);
    this.alertify.message('Registration Cancelled');
  }
  ngOnInit() {
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMismatchValidator);
    this.bsDateConfig = {
      containerClass: 'theme-red',
    };
    this.createRegisterForm();
  }

  createRegisterForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required ],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {validators: this.passwordMismatchValidator});
  }

  passwordMismatchValidator(g: FormGroup){
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

}
