import { Routes } from '@angular/router';
import { UserForm } from './features/admin/user-form/user-form';
import { UserManagement } from './features/admin/user-management/user-management';
import { NotFoundComponent } from './shared/layout/not-found/not-found';
import { DepartmentManagement } from './features/admin/department-management/department-management';

export const routes: Routes = [
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { path: 'admin', component: UserManagement },
  {
  path: 'departments',
  component: DepartmentManagement
},
  { path: 'admin/users/new', component: UserForm },
  { path: '404', component: NotFoundComponent },
  { path: '**', redirectTo: '404' },
];
