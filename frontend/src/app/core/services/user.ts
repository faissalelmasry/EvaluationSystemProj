import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable, map } from 'rxjs';
import { PagesResult } from '../models/pagination.model';
import { CreateUserDto, User, UserCreatePayload } from '../models/user.model';

function normalizePagesResult<T>(raw: any): PagesResult<T> {
  return {
    items: raw.Items ?? raw.items ?? [],
    totalCount: raw.TotalCount ?? raw.totalCount ?? 0,
    pageNumber: raw.PageNumber ?? raw.pageNumber ?? 1,
    pageSize: raw.PageSize ?? raw.pageSize ?? 10,
  };
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http=inject(HttpClient) ;
  private readonly apiUrl=`${environment.apiUrl}/users`;
  getUsers(pageNumber=1,pageSize=10,search?:string,sortBy?:string,descending?:boolean,email?:string):Observable<PagesResult<User>>{
    let params=new HttpParams().set('pageNumber',pageNumber).set('pageSize',pageSize);
    if(search){
      params=params.set('search',search);
    }
    if(sortBy){
      params=params.set('sortBy',sortBy);
    }
    if(descending){
      params=params.set('descending',descending);
    }
    if(email){
      params=params.set('email',email);
    }
    return this.http.get<PagesResult<User>>(`${this.apiUrl}`,{
      params
    }).pipe(map(res => normalizePagesResult<User>(res)));
  }
  getById(id:number):Observable<User>{
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }
  create(payLoad:CreateUserDto):Observable<User>{
    return this.http.post<User>(`${this.apiUrl}`,payLoad);
  }
  update(id:number,payLoad:UserCreatePayload):Observable<void>{
    return this.http.put<void>(`${this.apiUrl}/${id}`,payLoad);
  }
  delete(id:number):Observable<void>{
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  activate(id:number):Observable<void>{
    return this.http.put<void>(`${this.apiUrl}/${id}/activate`,null);
  }
  deactivate(id:number):Observable<void>{
    return this.http.put<void>(`${this.apiUrl}/${id}/deactivate`,null);
  }
  assignRole(id:number,role:string):Observable<void>{
    return this.http.put<void>(`${this.apiUrl}/${id}/role`,{role});
  }
  assignDepartment(id:number,departmentId:number):Observable<void>{
    return this.http.put<void>(`${this.apiUrl}/${id}/department`,{ departmentId }
  );
  }
}
