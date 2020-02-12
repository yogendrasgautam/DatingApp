import { Component, OnInit } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Pagination, PaginatedResult } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  users: User[];
  pagination: Pagination;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'Male'}, {value: 'female', display: 'Female'}];
  likeParams: string;
  constructor(private userService: UserService, private route: ActivatedRoute, private alertify: AlertifyService ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;

      this.likeParams = 'Likers';
    });
   
  }

resetFilters() {
  
    this.loadUsers();
  }

pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, 
      null, this.likeParams).subscribe((user: PaginatedResult<User[]>) => {
      this.users = user.result;
      this.pagination = user.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }
}
