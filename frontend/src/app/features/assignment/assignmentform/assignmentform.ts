import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { Assignment } from '../../../core/services/assignment';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TemplateService } from '../../../core/services/template.service';
import { UserService } from '../../../core/services/user';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-assignmentform',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './assignmentform.html',
  styleUrl: './assignmentform.css',
})
export class Assignmentform implements OnInit {
  private assignmentService = inject(Assignment);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  templateService = inject(TemplateService);
  userService = inject(UserService);

  assignmentForm!: FormGroup;
  templatesLookup: any[] = [];
  
  // 👥 SPLIT LOOKUPS FOR CLEANER DROPDOWNS
  evaluatorsLookup: any[] = [];
  evaluateesLookup: any[] = [];
  
  isLoading: boolean = false;
  isSubmitted: boolean = false;
  errorMessage: string | null = null;
  assignmentId: number | null = null;

  ngOnInit(): void {
    this.initForm();
    this.loadInitialDataAndCheckMode();
  }

  private initForm() {
    this.assignmentForm = this.fb.group({
      templateId: ['', Validators.required],
      evaluatorId: ['', Validators.required],
      evaluateeId: ['', Validators.required],
      dueDate: ['', Validators.required]
    });
  }

  private loadInitialDataAndCheckMode() {
    this.isLoading = true;

    forkJoin({
      templates: this.templateService.GetTemplates(1, 100, ''),
      users: this.userService.getUsers(1, 100)
    }).subscribe({
      next: (res) => {
        this.templatesLookup = res.templates;
        const allUsers = res.users.items || [];
        
        // 🔎 FILTER: Evaluators Only
        this.evaluatorsLookup = allUsers.filter((u: any) => {
          const roleStr = u.role || '';
          const rolesArr = Array.isArray(u.roles) ? u.roles : [];
          return roleStr.toLowerCase() === 'evaluator' || 
                 rolesArr.some((r: any) => r.toString().toLowerCase() === 'evaluator');
        });

        // 🔎 FILTER: Evaluatees Only
        this.evaluateesLookup = allUsers.filter((u: any) => {
          const roleStr = u.role || '';
          const rolesArr = Array.isArray(u.roles) ? u.roles : [];
          return roleStr.toLowerCase() === 'evaluatee' || 
                 rolesArr.some((r: any) => r.toString().toLowerCase() === 'evaluatee');
        });
        
        // Fallback check: If the filters return absolutely nothing because of a role string mismatch,
        // we fill them with all users so the UI doesn't completely break.
        if (this.evaluatorsLookup.length === 0) this.evaluatorsLookup = allUsers;
        if (this.evaluateesLookup.length === 0) this.evaluateesLookup = allUsers;

        this.route.queryParams.subscribe(params => {
          if (params['id']) {
            this.assignmentId = Number(params['id']);
            this.loadAssignmentForEdit(this.assignmentId);
          } else {
            this.isLoading = false;
            this.cdr.detectChanges();
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load form data.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private loadAssignmentForEdit(id: number) {
    this.assignmentService.getAssignmentById(id).subscribe({
      next: (data) => {
        const res = data as any;

        let formattedDate = '';
        if (res.dueDate) {
          formattedDate = res.dueDate.split('T')[0];
        }
        
        let templateIdValue = res.templateId || res.template?.id || res.templateID;
        let evaluatorIdValue = res.evaluatorId || res.evaluator?.id || res.evaluatorID;
        let evaluateeIdValue = res.evaluateeId || res.evaluatee?.id || res.evaluateeID;

        // Combining lookups here to ensure edit mode can still search all active users
        const combinedUsers = [...this.evaluatorsLookup, ...this.evaluateesLookup];

        if (!evaluatorIdValue && res.evaluatorName) {
          const foundUser = combinedUsers.find(u => 
            u.name === res.evaluatorName || 
            u.fullName === res.evaluatorName || 
            u.username === res.evaluatorName
          );
          if (foundUser) {
            evaluatorIdValue = foundUser.id;
          }
        }

        if (!evaluateeIdValue && res.evaluateeName) {
          const foundUser = combinedUsers.find(u => 
            u.name === res.evaluateeName || 
            u.fullName === res.evaluateeName || 
            u.username === res.evaluateeName
          );
          if (foundUser) {
            evaluateeIdValue = foundUser.id;
          }
        }

        this.assignmentForm.patchValue({
          templateId: templateIdValue ? Number(templateIdValue) : '',
          evaluatorId: evaluatorIdValue ? Number(evaluatorIdValue) : '',
          evaluateeId: evaluateeIdValue ? Number(evaluateeIdValue) : '',
          dueDate: formattedDate
        });
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load assignment details for editing.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit() {
    this.isSubmitted = true;
    this.errorMessage = null;

    if (this.assignmentForm.invalid) {
      return;
    }

    this.isLoading = true;
    
    const formValues = this.assignmentForm.value;
    const dto = {
      templateId: Number(formValues.templateId),
      evaluatorId: Number(formValues.evaluatorId),
      evaluateeId: Number(formValues.evaluateeId),
      dueDate: new Date(formValues.dueDate).toISOString()
    };

    const request$ = this.assignmentId 
      ? this.assignmentService.updateAssignment(this.assignmentId, dto)
      : this.assignmentService.createAssignment(dto);

    request$.subscribe({
      next: () => {
        this.assignmentForm.reset(); 
        this.isSubmitted = false;
        this.isLoading = false;
        this.router.navigate(['/assignments']);
      },
      error: (err) => {
        console.log(err);
        this.isLoading = false;

        if (err.error?.errors) {
          const validationErrors = Object.values(err.error.errors).flat();
          this.errorMessage = validationErrors.join(' | ');
        } else if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else if (err.error?.error) {
          this.errorMessage = err.error.error;
        } else if (typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else {
          this.errorMessage = err.message || 'An unexpected server error occurred.';
        }

        this.cdr.detectChanges();
      }
    });
  }
}