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
startEvaluation(assignmentId: number, templateId: number) {
  console.log('Navigating with Assignment ID:', assignmentId, 'and Template ID:', templateId);
  
  this.router.navigate(['/evaluation', assignmentId, 'submit'], {
    queryParams: { templateId: templateId }
  });
}
// --- Pagination State ---
  currentPage = 1;
  itemsPerPage = 5; 

  get totalPages(): number {
    const total = this.pendingAssignments?.length || 0;
    return total === 0 ? 1 : Math.ceil(total / this.itemsPerPage);
  }

  get paginatedPending(): any[] {
    const allItems = this.pendingAssignments || [];
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return allItems.slice(start, start + this.itemsPerPage);
  }

  get startIndex(): number {
    return (this.pendingAssignments?.length || 0) === 0 ? 0 : (this.currentPage - 1) * this.itemsPerPage + 1;
  }

  get endIndex(): number {
    const end = this.currentPage * this.itemsPerPage;
    const total = this.pendingAssignments?.length || 0;
    return end > total ? total : end;
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