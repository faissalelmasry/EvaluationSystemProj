import { Component, ElementRef, inject, ViewChild, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reportservice } from '../../../core/services/reportservice';
import { forkJoin } from 'rxjs';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-report',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report.html',
  styleUrl: './report.css',
})
export class Report implements OnInit {
  private reportService = inject(Reportservice);
  private cdr = inject(ChangeDetectorRef);

  isLoading: boolean = true;
  errorMessage: string | null = null;
  private chartInstances: Chart[] = [];

  @ViewChild('deptChart') deptChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('completionChart') completionChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('topEvaluateesChart') topEvaluateesChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('lowScoresChart') lowScoresChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('trendsChart') trendsChartRef!: ElementRef<HTMLCanvasElement>;

  ngOnInit(): void {
    this.loadAllReportsData();
  }

  private loadAllReportsData() {
    this.isLoading = true;

    forkJoin({
      dashboard: this.reportService.getDashboard(),
      departments: this.reportService.getByDepartment(),
      completion: this.reportService.getCompletionRate(),
      topScores: this.reportService.getTopScores()
    }).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        
        console.log('=== RAW BACKEND RESPONSE ===', res);

        setTimeout(() => {
          this.destroyCharts();
          this.initializeCharts(res);
          this.cdr.detectChanges();
        }, 50);
      },
      error: (err) => {
        console.error('API Error:', err);
        this.errorMessage = 'Failed to load report analytics. Please verify backend endpoints.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private destroyCharts() {
    this.chartInstances.forEach(chart => chart.destroy());
    this.chartInstances = [];
  }

  private initializeCharts(res: any) {
    if (!this.deptChartRef || !this.completionChartRef || !this.topEvaluateesChartRef || !this.lowScoresChartRef || !this.trendsChartRef) {
      return;
    }

    const dash = res.dashboard?.data || res.dashboard?.result || res.dashboard;
    const compData = res.completion?.data || res.completion?.result || res.completion;
    
    const deptArray = Array.isArray(res.departments) ? res.departments : (res.departments?.data || []);
    const deptLabels = deptArray.map((d: any) => d.departmentName || d.name || d.Name || '');
    const deptScores = deptArray.map((d: any) => d.averageScore || d.totalScore || d.score || 0);

    const c1 = new Chart(this.deptChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: deptLabels,
        datasets: [{
          label: 'Average Score %',
          data: deptScores,
          backgroundColor: '#059669',
          borderRadius: 6
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c1);

    const completed = compData?.completedAssignments || dash?.completedEvaluations || 0;
    const total = compData?.totalAssignments || 0;
    const pending = total > 0 ? (total - completed) : (compData?.pendingEvaluations || dash?.pendingEvaluations || 0);
    
    const c2 = new Chart(this.completionChartRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels: ['Completed', 'Pending'],
        datasets: [{
          data: [completed, pending],
          backgroundColor: ['#10b981', '#f59e0b']
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c2);

    const topScoresArray = res.topScores?.topEvaluatees || [];
    const topLabels = topScoresArray.map((u: any) => u.evaluateeName || u.fullName || u.name || '');
    const topValues = topScoresArray.map((u: any) => u.score || u.totalScore || u.percentage || u.averageScore || 0);

    const c3 = new Chart(this.topEvaluateesChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: topLabels,
        datasets: [{
          label: 'Score',
          data: topValues,
          backgroundColor: '#34d399',
          borderRadius: 6
        }]
      },
      options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c3);

    const lowScoresArray = res.topScores?.lowScoreEvaluations || [];
    const lowLabels = lowScoresArray.map((u: any) => u.evaluateeName || u.fullName || u.name || '');
    const lowValues = lowScoresArray.map((u: any) => u.score || u.totalScore || u.percentage || 0);

    const c4 = new Chart(this.lowScoresChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: lowLabels,
        datasets: [{
          label: 'Critical Scores (<60)',
          data: lowValues,
          backgroundColor: '#ef4444',
          borderRadius: 6
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c4);

    const monthlyTrends = compData?.monthlyTrends || dash?.monthlyTrends || [];
    const trendLabels = monthlyTrends.map((t: any) => t.month || '');
    const createdValues = monthlyTrends.map((t: any) => t.totalCreated || 0);
    const completedValues = monthlyTrends.map((t: any) => t.totalCompleted || 0);

    const c5 = new Chart(this.trendsChartRef.nativeElement, {
      type: 'line',
      data: {
        labels: trendLabels,
        datasets: [
          {
            label: 'Assignments Created',
            data: createdValues,
            borderColor: '#3b82f6',
            backgroundColor: 'rgba(59, 130, 246, 0.05)',
            fill: true,
            tension: 0.3
          },
          {
            label: 'Assignments Completed',
            data: completedValues,
            borderColor: '#10b981',
            backgroundColor: 'rgba(16, 185, 129, 0.1)',
            fill: true,
            tension: 0.3
          }
        ]
      },
      options: { 
        responsive: true, 
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 1
            }
          }
        }
      }
    });
    this.chartInstances.push(c5);
  }
}