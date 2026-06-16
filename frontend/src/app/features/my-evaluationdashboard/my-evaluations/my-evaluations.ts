import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { EvaluationService } from '../../../core/services/evaluation'; // Adjust path if needed

@Component({
  selector: 'app-my-evaluations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-evaluations.html',
  styleUrl: './my-evaluations.css'
})
export class MyEvaluationsComponent implements OnInit {
  private evaluationService = inject(EvaluationService);
  private router = inject(Router);

  assignments = signal<any[]>([]);
  isLoading = signal<boolean>(true);

  ngOnInit(): void {
    this.loadAssignments();
  }

loadAssignments(): void {
    const currentUserId = 6; 
    console.log('🚀 Fetching assignments for User ID:', currentUserId);

    this.evaluationService.getMyPendingAssignments(currentUserId).subscribe({
      next: (data: any) => {
        console.log('✅ Backend returned data:', data);
        
        let assignmentsArray = [];

        if (Array.isArray(data)) {
          assignmentsArray = data;
        } else if (data && data.message) {
          console.log('💬 Backend says:', data.message);
          assignmentsArray = []; 
        } else if (data && Array.isArray(data.data)) {
          assignmentsArray = data.data;
        } else if (data && typeof data === 'object' && data.id) {
          assignmentsArray = [data]; 
        }

        const pendingOnly = assignmentsArray.filter((a: any) => a.status !== 'Submitted');
        
        this.assignments.set(pendingOnly);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('❌ API Error fetching assignments:', err);
        this.isLoading.set(false);
      }
    });
  }

  goToEvaluation(assignmentId: number, templateId: number): void {
    this.router.navigate([`/evaluation/${assignmentId}/submit`], {
      queryParams: { templateId: templateId }
    });
  }
}