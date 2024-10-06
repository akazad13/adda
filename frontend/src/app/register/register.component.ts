import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup, AbstractControlOptions, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { User } from '../models/user';
import { HasErrorPipe } from '../pipes/has-error.pipe';
import { IsInvalidPipe } from '../pipes/is-invalid.pipe';
import { NgIf } from '@angular/common';
import { UserService } from '../services/user.service';
import { firstValueFrom } from 'rxjs';

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
    private readonly userService: UserService,
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

  async register(): Promise<void> {
    if (this.registerForm.valid) {
      this.user = { ...this.registerForm.value };

      try {
        await firstValueFrom(this.userService.register(this.user));
        this.alertify.success('registration successful');
        try {
          await firstValueFrom(this.authService.login(this.user));
          this.router.navigate(['/members']);
        } catch (e: any) {
          this.alertify.error(e.error.title);
        }
      } catch (e: any) {
        this.alertify.error(e.error);
      }
    }
  }

  get formData() {
    return this.registerForm.controls;
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
