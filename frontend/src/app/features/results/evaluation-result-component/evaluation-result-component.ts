import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EvaluationService } from '../../../core/services/evaluation'; // Adjust path

@Component({
  selector: 'app-evaluation-result',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './evaluation-result.html',
  styleUrl: './evaluation-result.css'
})
export class EvaluationResultComponent implements OnInit {
  private evaluationService = inject(EvaluationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  assignmentId!: number;
  resultData = signal<any>(null);
  isLoading = signal<boolean>(true);

  ngOnInit(): void {
    this.assignmentId = Number(this.route.snapshot.paramMap.get('assignmentId'));
    this.loadResults();
  }

  private loadResults(): void {
    this.evaluationService.getEvaluationResult(this.assignmentId).subscribe({
      next: (data) => {
        console.log('✅ Final Result Loaded:', data);
        this.resultData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('❌ Failed to fetch results:', err);
        alert('Could not load the final results. They might not be ready yet.');
        this.isLoading.set(false);
      }
    });
  }

  goBackToDashboard() {
    this.router.navigate(['/my-evaluations']);
  }
}