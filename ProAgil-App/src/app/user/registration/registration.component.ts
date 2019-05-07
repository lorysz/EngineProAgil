import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/User';
import { AuthService } from 'src/app/_services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  registerForm: FormGroup;
  user: User;

  constructor(private fb: FormBuilder,
              private toastr: ToastrService,
              private authService: AuthService,
              private router: Router
              ) { }

  ngOnInit() {
    this.validation();
  }

  validation() {
    this.registerForm = this.fb.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', [Validators.required]],
      passwords: this.fb.group({
        password: ['', [Validators.required, Validators.minLength(4)]],
        confirmPassword: ['', [Validators.required]]
      }, {validator: this.compararSenhas})
    });
  }

  compararSenhas(fb: FormGroup) {
    const confirmSenhaCtrl = fb.get('confirmPassword');

    if (confirmSenhaCtrl.errors == null || 'mismatch' in confirmSenhaCtrl.errors) {
      if (fb.get('password').value !== confirmSenhaCtrl.value) {
        fb.get('password').setErrors({ mismatch: true });
      } else {
        confirmSenhaCtrl.setErrors(null);
      }
    }
  }

  cadastrarUsuario() {
    if (this.registerForm.valid) {
      this.user = Object.assign(
        {password: this.registerForm.get('passwords.password').value},
        this.registerForm.value
        );

      this.authService.register(this.user).subscribe(
          () => {
            this.router.navigate(['/user/login']);
            this.toastr.success('Cadastro Realizado');
          }, error => {
            const erro = error.error;
            console.log(erro);
            if (erro === null) {
              this.toastr.error('Problemas para Cadastrar.');
            } else {
              erro.forEach(err => {
                switch (err.code) {
                  case 'DuplicateUserName':
                    this.toastr.error('Cadastro Duplicado');
                    break;
                  default:
                    this.toastr.error(`Erro no Cadastro! CODE: ${err.code}`);
                    break;
                }
              });
            }

          }
        );
    }
  }
}
