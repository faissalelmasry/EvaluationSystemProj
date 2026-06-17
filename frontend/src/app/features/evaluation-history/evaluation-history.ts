import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Assignment } from '../../core/services/assignment';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-evaluation-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './evaluation-history.html',
  styleUrl: './evaluation-history.css'
})
export class EvaluationHistoryComponent implements OnInit {
  private assignmentService = inject(Assignment);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  historyList = signal<any[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadHistory();
  }

  private getCurrentUserId(): number {
    const token = localStorage.getItem('jwt_token');
    if (!token) return 0;
    try {
      const decoded: any = jwtDecode(token);
      const userId = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 
                     decoded['id'] || 
                     decoded['sub'];
      return userId ? Number(userId) : 0;
    } catch {
      return 0;
    }
  }

  private loadHistory() {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const currentUserId = this.getCurrentUserId();

    this.assignmentService.getAssignments().subscribe({
      next: (res: any) => {
        const allAssignments = Array.isArray(res) ? res : (res?.items || res?.data || []);
        
        const filtered = allAssignments.filter((a: any) => {
          const isUserMatch = Number(a.evaluateeId) === currentUserId;
          const isCompleted = a.status === 'Completed' || a.status === 5 || a.status === '5';
          return isUserMatch && isCompleted;
        });
        
        this.historyList.set(filtered);
        this.isLoading.set(false);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load history:', err);
        this.errorMessage.set('Failed to load evaluation history.');
        this.isLoading.set(false);
        this.cdr.detectChanges();
      }
    });
  }

  viewResults(assignmentId: number) {
    this.router.navigate(['/evaluation', assignmentId, 'result']);
  }
}