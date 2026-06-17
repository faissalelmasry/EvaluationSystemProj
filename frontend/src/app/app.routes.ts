import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Register } from './features/register/register';
import { ForgotPassword } from './features/forgot-password/forgot-password';
import { ResetPassword } from './features/reset-password/reset-password';
import { ChangePassword } from './features/change-password/change-password';
import { Profile } from './features/profile/profile';
import { UserManagement } from './features/admin/user-management/user-management';
import { DepartmentManagement } from './features/admin/department-management/department-management';
import { UserForm } from './features/admin/user-form/user-form';
import { NotFoundComponent } from './shared/layout/not-found/not-found';
import { CreateTemplate } from './features/evaluation-template/create-template/create-template';
import { UpdateTemplate } from './features/evaluation-template/update-template/update-template';
import { CreateSection } from './features/evaluation-section/create-section/create-section';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { TemplateDetails } from './features/evaluation-template/template-details/template-details';
import { CreateCriteria } from './features/evaluation-criteria/create-criteria/create-criteria';
import { authGuard } from './core/guards/auth-guard';
import { Assignmentlist } from './features/assignment/assignmentlist/assignmentlist';
import { Assignmentform } from './features/assignment/assignmentform/assignmentform';
import { roleGuard } from './core/guards/role-guard';
import { EvaluationFormComponent } from './features/evaluation/evaluation-form/evaluation-form';
import { ManagerReviewComponent } from './features/Review/manager-review/manager-review';
import { EvaluationResultComponent } from './features/results/evaluation-result-component/evaluation-result-component';
import { Dashboard } from './features/dashbord/dashbord/dashboard';
import { Report } from './features/reports/report/report';
import { Pendingassignment } from './features/assignment/pendingassignment/pendingassignment';
import { ReviewDashboardComponent } from './features/review-dashboard/review-dashboard';
import { EvaluationHistoryComponent } from './features/evaluation-history/evaluation-history';
export const routes: Routes = [
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { 
    path: 'evaluation/:assignmentId/submit', 
    component: EvaluationFormComponent
  },
  { 
  path: 'reviews', 
  component: ReviewDashboardComponent 
  },
  { path: 'admin', component: UserManagement },
  { 
    path: 'evaluation/:assignmentId/review', 
    component: ManagerReviewComponent 
  },
  { path: 'history', component: EvaluationHistoryComponent },
 { 
    path: 'evaluation/:assignmentId/result', 
    component: EvaluationResultComponent 
  },
  
  { path: 'admin/users/new', component: UserForm },
  { path:'templates',component:TemplateListComponent},
  { path:'template/add',component:CreateTemplate},
  { path:'template/:id',component:TemplateDetails},
  { path:'template/update/:id',component:UpdateTemplate},
  { path:'section/add/:templateid',component:CreateSection},
  { path:'section/update/:sectionid',component:CreateSection},
  { path:'criteria/add/:sectionid',component:CreateCriteria},
  { path:'criteria/update/:criteriaid',component:CreateCriteria},
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'reset-password', component: ResetPassword },
  { path: 'profile', component: Profile, canActivate: [authGuard] },
  { path: 'change-password', component: ChangePassword, canActivate: [authGuard] },
  { path: 'admin', component: UserManagement, canActivate: [authGuard] },
  { path: 'departments', component: DepartmentManagement, canActivate: [authGuard] },
  { path: 'admin/users/new', component: UserForm, canActivate: [authGuard] },
  { path: 'assignments', component: Assignmentlist, canActivate: [authGuard] },
  { path: 'create-assignment', component: Assignmentform , canActivate: [authGuard] },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'reports', component: Report, canActivate: [authGuard] },
  { path: 'pending', component: Pendingassignment, canActivate: [authGuard] },
  { path: '404', component: NotFoundComponent },
  { path: '**', redirectTo: '404' },
];