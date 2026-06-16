import { Routes } from '@angular/router';
import { UserForm } from './features/admin/user-form/user-form';
import { UserManagement } from './features/admin/user-management/user-management';
import { NotFoundComponent } from './shared/layout/not-found/not-found';
import { roleGuard } from './core/guards/role-guard';
import { EvaluationFormComponent } from './features/evaluation/evaluation-form/evaluation-form';
import { MyEvaluationsComponent } from './features/my-evaluationdashboard/my-evaluations/my-evaluations';
import { ManagerReviewComponent } from './features/Review/manager-review/manager-review';
import { EvaluationResultComponent } from './features/results/evaluation-result-component/evaluation-result-component';

export const routes: Routes = [
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { 
    path: 'evaluation/:assignmentId/submit', 
    component: EvaluationFormComponent
  },
  { path: 'admin', component: UserManagement },
  { 
    path: 'my-evaluations', 
    component: MyEvaluationsComponent 
  },
  { 
    path: 'evaluation/:assignmentId/review', 
    component: ManagerReviewComponent 
  },
{ 
    path: 'evaluation/:assignmentId/result', 
    component: EvaluationResultComponent 
  },
  
  { path: 'admin/users/new', component: UserForm },
  { path: '404', component: NotFoundComponent },

  { path: '**', redirectTo: '404' },

];
