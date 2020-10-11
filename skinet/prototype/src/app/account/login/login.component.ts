import { Component, OnInit} from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';

import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  returnUrl: string;
 
  public reactiveForm: FormGroup = new FormGroup({
    recaptchaReactive: new FormControl(null, Validators.required),
    email: new FormControl('', [Validators.required,Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]),
    password: new FormControl('', [Validators.required,Validators.pattern(`^(?=.*[a-z])(?=.*[A-Z])((?=.*\\d)|(?=.*[!@#$%^&*()'"]))[A-Za-z\\d!@#$%^&*()'"](?!\\s).{5,21}$`)])
});

  constructor(private accountService : AccountService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
  }

  onSubmit() {
    this.accountService.login(this.reactiveForm.value).subscribe(()=>{this.router.navigateByUrl(this.returnUrl);},error=>{
      // console.log(error);
    })
  }

}
