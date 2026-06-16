import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Assignment } from '../../core/services/assignment';

@Component({
  selector: 'app-review-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-dashboard.html',
  styleUrls: ['./review-dashboard.css'] 
})
export class ReviewDashboardComponent implements OnInit {
  private assignmentService = inject(Assignment);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  reviewQueue: any[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadReviewQueue();
  }

  private loadReviewQueue() {
    this.isLoading = true;
    this.errorMessage = null;

    this.assignmentService.getAssignments().subscribe({
      next: (res: any) => {
        const allAssignments = Array.isArray(res) ? res : (res?.items || res?.data || []);
        
        this.reviewQueue = allAssignments.filter((a: any) => 
          a.status === 'Submitted' || a.status === 3 || a.status === '3'
        );
        
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load assignments:', err);
        this.errorMessage = 'Failed to load the review queue. Please check backend connection.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goToReview(assignmentId: number) {
    this.router.navigate(['/evaluation', assignmentId, 'review']);
  }
}