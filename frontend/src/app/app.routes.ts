import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role-guard';
import { EvaluationFormComponent } from './features/evaluation/evaluation-form/evaluation-form';

export const routes: Routes = [
{ 
    path: 'evaluation/:assignmentId/submit', 
    component: EvaluationFormComponent
  }];
