import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, of, ReplaySubject } from 'rxjs';
import { IUser } from "../shared/models/user";
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { IAddress } from '../shared/models/address';


@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();
  private isAdminSource = new ReplaySubject<boolean>(1);
  isAdmin$ = this.isAdminSource.asObservable();



  constructor( private http: HttpClient, private router:Router) { }

  getCurrentUserRoles(): string[] {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken = JSON.parse(atob(token.split('.')[1]));
      return decodedToken.role;
    }
  }

  isAdmin(token: string): boolean {
    if (token) {
      const decodedToken = JSON.parse(atob(token.split('.')[1]));
      if (decodedToken.role.indexOf('Admin') > -1) {
        return true;
      }
    }
  }

  
    

  loadCurrentUser(token: string) {
    if (token === null) {
      this.currentUserSource.next(null);
      return of(null);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);

    return this.http.get(this.baseUrl + 'account', {headers}).pipe(
      map((user: IUser) => {
        if (user) {
          //not need, already set the token, when user sign in
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
          this.isAdminSource.next(this.isAdmin(user.token));
        }
      })
    );
  }


  login(values:any){
    return this.http.post(this.baseUrl+'account/login', values).pipe(
      map((user:IUser)=>{
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
          this.isAdminSource.next(this.isAdmin(user.token));
        }
      })
    )
  }


  register(values:any){
    return this.http.post(this.baseUrl+'account/register', values).pipe(
      map((user:IUser)=>{
        if (user) {
          localStorage.setItem('token', user.token);
        }
      })
    )
  }

  logout(){
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  checkEmailExists(email: string) {
    return this.http.get(this.baseUrl + 'account/emailexists?email=' + email);
  }

  getUserAddress() {
    return this.http.get<IAddress>(this.baseUrl + 'account/address');
  }

  updateUserAddress(address: IAddress) {
    return this.http.put<IAddress>(this.baseUrl + 'account/address', address);
  }

}
