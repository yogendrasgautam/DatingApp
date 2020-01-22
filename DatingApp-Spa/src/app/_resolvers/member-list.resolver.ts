import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';

@Injectable()
export class MemberListResolver implements Resolve<PaginatedResult<User[]>> {
    pageNumber = 1;
    itemsPerPage = 5;

    constructor(private userService: UserService,
                private router: Router, private alertify: AlertifyService) {
    }
     resolve(route: ActivatedRouteSnapshot): Observable<PaginatedResult<User[]>> {
         return this.userService.getUsers(this.pageNumber, this.itemsPerPage).pipe(
             catchError(error => {
                 this.alertify.error('Problem Retrieving Data');
                 this.router.navigate(['/home']);
                 return of(null);
             })
         );
 
     }
 
 }

