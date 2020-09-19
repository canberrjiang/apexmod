import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';

import { Router, ActivatedRoute } from '@angular/router';

// export interface FormModel {
//   captcha?: string;
// }

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
   loginForm: FormGroup;
   returnUrl: string;
  //  title = 'recaptchaapp';
  //  stieKey = "6Lc4Cs4ZAAAAAH4K4HFJ_q_kA99UyGIMT56xK5an"
  //  recaptcha:any[];

  // public formModel: FormModel = {};


  constructor(private accountService : AccountService, private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
    this.creatLoginFrom();

  }


//   resolved(captchaResponse: any[]) {
//     this.recaptcha = captchaResponse;
//     console.log(this.recaptcha);
// }

  creatLoginFrom() {
    this.loginForm = new FormGroup({
      //change to userName/email
      // userName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required,Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]),
      password: new FormControl('', Validators.required)
    })
  }

  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe(()=>{this.router.navigateByUrl(this.returnUrl);},error=>{
      console.log(error);
    })
  }

}
