import { Routes } from '@angular/router';
import { UserForm } from './features/admin/user-form/user-form';
import { UserManagement } from './features/admin/user-management/user-management';
import { NotFoundComponent } from './shared/layout/not-found/not-found';
import { CreateTemplate } from './features/evaluation-template/create-template/create-template';
import { UpdateTemplate } from './features/evaluation-template/update-template/update-template';
import { CreateSection } from './features/evaluation-section/create-section/create-section';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { TemplateDetails } from './features/evaluation-template/template-details/template-details';
import { CreateCriteria } from './features/evaluation-criteria/create-criteria/create-criteria';

export const routes: Routes = [
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { path: 'admin', component: UserManagement },
  { path: 'admin/users/new', component: UserForm },
  { path:'templates',component:TemplateListComponent},
  { path:'template/add',component:CreateTemplate},
  { path:'template/:id',component:TemplateDetails},
  { path:'template/update/:id',component:UpdateTemplate},
  { path:'section/add/:templateid',component:CreateSection},
  { path:'section/update/:sectionid',component:CreateSection},
  { path:'criteria/add/:sectionid',component:CreateCriteria},
  { path:'criteria/update/:criteriaid',component:CreateCriteria},
  { path: '404', component: NotFoundComponent },
  { path: '**', redirectTo: '404' },
];
