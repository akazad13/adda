import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import {
  Validators,
  FormBuilder,
  FormGroup,
  AbstractControlOptions,
  AbstractControl,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { User } from '../models/user';
import { HasErrorPipe } from '../shared/pipes/has-error.pipe';
import { IsInvalidPipe } from '../shared/pipes/is-invalid.pipe';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styles: ``,
  imports: [HasErrorPipe, IsInvalidPipe, BsDatepickerModule, ReactiveFormsModule, NgIf],
  standalone: true,
})
export class RegisterComponent implements OnInit {
  user!: User;
  @Output() cancelRegister = new EventEmitter<boolean>();
  registerForm!: FormGroup;
  bsConfig!: Partial<BsDatepickerConfig>; // adding partial, make all the required field partial

  constructor(
    private readonly authService: AuthService,
    private readonly alertify: AlertifyService,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red',
    };
    this.createRegisterFrom();
  }

  createRegisterFrom() {
    const formOptions: AbstractControlOptions = {
      validators: this.passwordMatchValidator,
    };
    this.registerForm = this.formBuilder.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
        confirmPassword: ['', Validators.required],
      },
      formOptions
    );
  }

  passwordMatchValidator(g: AbstractControl) {
    return g.get('password')?.value === g.get('confirmPassword')?.value ? null : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = { ...this.registerForm.value };
      this.authService.register(this.user).subscribe(
        () => {
          this.alertify.success('registration successful');
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(
            () => {
              this.router.navigate(['/members']);
            },
            (error) => {
              this.alertify.error(error);
            }
          );
        }
      );
    }
  }

  get formData() {
    return this.registerForm.controls;
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
