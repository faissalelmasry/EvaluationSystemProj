import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Assignment } from '../../../core/services/assignment';

@Component({
  selector: 'app-pendingassignment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pendingassignment.html',
  styleUrl: './pendingassignment.css',
})
export class Pendingassignment implements OnInit {
 private assignmentService = inject(Assignment);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  pendingAssignments: any[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadPendingAssignments();
  }

  private loadPendingAssignments() {
    this.isLoading = true;
    this.errorMessage = null;

    const savedUser = localStorage.getItem('user'); 
    const currentUserId = savedUser ? JSON.parse(savedUser).id : 1; 

    this.assignmentService.getPendingAssignmentsForUser(currentUserId).subscribe({
      next: (res: any) => {
        this.pendingAssignments = Array.isArray(res) 
          ? res 
          : (res?.items || res?.data || []);
        this.isLoading = false;
        
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load pending evaluations. Please check backend connection.';
        this.isLoading = false;
        
        this.cdr.detectChanges();
      }
    });
  }
// change this method to navigate to the evaluation page
  startEvaluation(id: number) {
    this.router.navigate(['/evaluations/take', id]);
  }
}