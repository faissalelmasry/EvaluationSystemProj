import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { Department } from '../../../core/models/department.model';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-department-management',
  imports: [],
  templateUrl: './department-management.html',
  styleUrl: './department-management.scss',
})
export class DepartmentManagement implements OnInit {
  private readonly http=inject(HttpClient);
  private readonly apiUrl=`${environment.apiUrl}/departments`;
  departments=signal<Department[]>([]);
  searchControl=new FormControl('',{
    nonNullable:true
  })
  
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

}
