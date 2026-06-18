import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EvaluationService } from '../../../core/services/evaluation'; // Adjust path

@Component({
  selector: 'app-manager-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manager-review.html',
  styleUrl: './manager-review.scss'
})
export class ManagerReviewComponent implements OnInit {
  private evaluationService = inject(EvaluationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  assignmentId!: number;
  
  responses = signal<any[]>([]); 
  isLoading = signal<boolean>(true);
  managerNotes = '';

  ngOnInit(): void {
    this.assignmentId = Number(this.route.snapshot.paramMap.get('assignmentId'));
    this.loadSubmission();
  }

  private loadSubmission(): void {
    this.evaluationService.getEvaluationForReview(this.assignmentId).subscribe({
      next: (data) => {
        console.log('✅ Flat Responses Loaded:', data);
        
        const responsesArray = Array.isArray(data) ? data : (data.data || []);
        
        this.responses.set(responsesArray);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('❌ Failed to fetch submission details:', err);
        alert('Could not load the submission details for review.');
        this.isLoading.set(false);
      }
    });
  }

  onApprove(): void {
    if (!confirm('Are you sure you want to approve this evaluation?')) return;

    this.evaluationService.approveEvaluation(this.assignmentId, this.managerNotes).subscribe({
      next: () => {
        alert('Evaluation successfully approved!');
        this.router.navigate(['/reviews']);
      },
      error: () => alert('Failed to approve evaluation. Check console.')
    });
  }

  onReject(): void {
    if (!this.managerNotes.trim()) {
      alert('Please provide a reason in the manager notes before rejecting.');
      return;
    }
    if (!confirm('Are you sure you want to reject this evaluation?')) return;

    this.evaluationService.rejectEvaluation(this.assignmentId, this.managerNotes).subscribe({
      next: () => {
        alert('Evaluation rejected and sent back for updates.');
        this.router.navigate(['/reviews']);
      },
      error: () => alert('Failed to reject evaluation.')
    });
  }
  // --- Pagination State ---
  currentPage = 1;
  itemsPerPage = 3; // Adjust this number to show more/fewer answers per page

  get totalPages(): number {
    const total = this.responses()?.length || 0;
    return total === 0 ? 1 : Math.ceil(total / this.itemsPerPage);
  }

  get paginatedResponses(): any[] {
    const allResponses = this.responses() || [];
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return allResponses.slice(startIndex, startIndex + this.itemsPerPage);
  }

  get progressPercentage(): number {
    if (this.totalPages === 0) return 0;
    return (this.currentPage / this.totalPages) * 100;
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}