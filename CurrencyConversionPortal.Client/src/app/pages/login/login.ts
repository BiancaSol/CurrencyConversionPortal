import { Component } from '@angular/core';
import { AuthService } from '../../services/auth';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})

export class Login {
  loginForm: FormGroup;
  errorMessage: string | null = null;
  submitted = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

 async onSubmit() {
  this.submitted = true;
  this.errorMessage = null;
  if (this.loginForm.invalid) return;

  const { username, password } = this.loginForm.value;
  const success = await this.auth.login(username, password);
  if (success) {
    this.router.navigate(['/conversion']);
  } else {
    this.errorMessage = 'Invalid username or password.';
  }
}
}
