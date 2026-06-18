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
  // --- Pagination State ---
  currentPage = 1;
  itemsPerPage = 5; // Set how many rows you want per page

  get totalPages(): number {
    const total = this.reviewQueue?.length || 0;
    return total === 0 ? 1 : Math.ceil(total / this.itemsPerPage);
  }

  // Use this getter in your HTML instead of reviewQueue
  get paginatedQueue(): any[] {
    const allItems = this.reviewQueue || [];
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return allItems.slice(start, start + this.itemsPerPage);
  }

  // Display helpers for the table footer
  get startIndex(): number {
    return this.reviewQueue.length === 0 ? 0 : (this.currentPage - 1) * this.itemsPerPage + 1;
  }

  get endIndex(): number {
    const end = this.currentPage * this.itemsPerPage;
    return end > this.reviewQueue.length ? this.reviewQueue.length : end;
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
}