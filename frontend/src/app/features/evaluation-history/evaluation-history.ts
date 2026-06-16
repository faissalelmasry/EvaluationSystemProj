import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Assignment } from '../../core/services/assignment'; // Adjust path if needed

@Component({
  selector: 'app-evaluation-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './evaluation-history.html',
  styleUrls: ['./evaluation-history.css'] 
})
export class EvaluationHistoryComponent implements OnInit {
  private assignmentService = inject(Assignment);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  historyList: any[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadHistory();
  }

  private loadHistory() {
    this.isLoading = true;
    this.errorMessage = null;

    const savedUser = localStorage.getItem('user'); 
    const currentUser = savedUser ? JSON.parse(savedUser) : null;
    
    // Grab the name for our temporary fix
    const currentUserName = currentUser ? currentUser.username : 'toff';

    this.assignmentService.getAssignments().subscribe({
      next: (res: any) => {
        const allAssignments = Array.isArray(res) ? res : (res?.items || res?.data || []);
        
        this.historyList = allAssignments.filter((a: any) => {
          // ⚠️ TEMPORARY FIX: Match by Name since evaluateeId is missing from backend
          const isUserMatch = a.evaluateeName === currentUserName || a.evaluateeName === 'toff'; 
          
          const isCompleted = a.status === 'Completed' || a.status === 5 || a.status === '5';
          
          return isUserMatch && isCompleted;
        });
        
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load history:', err);
        this.errorMessage = 'Failed to load evaluation history.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  viewResults(assignmentId: number) {
    this.router.navigate(['/evaluation', assignmentId, 'result']);
  }
}