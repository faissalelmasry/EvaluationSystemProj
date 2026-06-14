import { Routes } from '@angular/router';
import { UserForm } from './features/admin/user-form/user-form';
import { UserManagement } from './features/admin/user-management/user-management';
import { NotFoundComponent } from './shared/layout/not-found/not-found';
import { roleGuard } from './core/guards/role-guard';
import { EvaluationFormComponent } from './features/evaluation/evaluation-form/evaluation-form';

export const routes: Routes = [
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { 
    path: 'evaluation/:assignmentId/submit', 
    component: EvaluationFormComponent
  }
  { path: 'admin', component: UserManagement },
  { path: 'admin/users/new', component: UserForm },
  { path: '404', component: NotFoundComponent },

  { path: '**', redirectTo: '404' },

];
