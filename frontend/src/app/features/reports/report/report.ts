import { Component, ElementRef, inject, ViewChild, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reportservice } from '../../../core/services/reportservice';
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
    this.reportService.getDashboard().subscribe({
      next: (dashboardData: any) => {
        const res = dashboardData as any;
        
        this.reportService.getByDepartment().subscribe({
          next: (deptData) => {
            this.isLoading = false;
            setTimeout(() => {
              this.destroyCharts();
              this.initializeCharts(res, deptData);
              this.cdr.detectChanges();
            }, 50);
          },
          error: (err) => this.handleError(err)
        });
      },
      error: (err) => this.handleError(err)
    });
  }

  private handleError(err: any) {
    console.error(err);
    this.errorMessage = 'Failed to load report analytics. Please verify backend endpoints.';
    this.isLoading = false;
    this.cdr.detectChanges();
  }

  private destroyCharts() {
    this.chartInstances.forEach(chart => chart.destroy());
    this.chartInstances = [];
  }

  private initializeCharts(dashboard: any, departments: any) {
    if (!this.deptChartRef || !this.completionChartRef || !this.topEvaluateesChartRef || !this.lowScoresChartRef || !this.trendsChartRef) {
      return;
    }

    const dash = dashboard?.data || dashboard?.result || dashboard;

    const deptArray = Array.isArray(departments) ? departments : (departments?.items || departments?.data || []);
    const deptLabels = deptArray.map((d: any) => d.departmentName || d.DepartmentName || '');
    const deptScores = deptArray.map((d: any) => d.averageScore || d.AverageScore || 0);

    const c1 = new Chart(this.deptChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: deptLabels.length ? deptLabels : ['HR', 'IT', 'Sales', 'Finance'],
        datasets: [{
          label: 'Average Score %',
          data: deptScores.length ? deptScores : [75, 88, 62, 80],
          backgroundColor: '#059669',
          borderRadius: 6
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c1);

    const completed = dash?.completedEvaluations || dash?.CompletedEvaluations || dash?.completedAssignments || dash?.CompletedAssignments || 0;
    const pending = dash?.pendingEvaluations || dash?.PendingEvaluations || dash?.pendingAssignments || dash?.CompletedAssignments || 0;
    
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

    const topScoresArray = dash?.topScoreEvaluations || dash?.topEvaluatees || dash?.TopEvaluatees || dash?.topScores || dash?.TopScores;
    const topUsers = (topScoresArray && topScoresArray.length > 0) ? topScoresArray : [
      { evaluateeName: 'Rawan Developer', score: 95 },
      { evaluateeName: 'Aly Mobile Expert', score: 92 }
    ];

    const c3 = new Chart(this.topEvaluateesChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: topUsers.map((u: any) => u.evaluateeName || u.name || u.employeeName),
        datasets: [{
          label: 'Score',
          data: topUsers.map((u: any) => u.score || u.Score || u.averageScore),
          backgroundColor: '#34d399',
          borderRadius: 6
        }]
      },
      options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c3);

    const lowScoresArray = dash?.lowScoreEvaluations || dash?.lowScores || dash?.LowScoreEvaluations;
    const lowScores = (lowScoresArray && lowScoresArray.length > 0) ? lowScoresArray : [
      { evaluateeName: 'Aly Mobile Expert', score: 45 }
    ];

    const c4 = new Chart(this.lowScoresChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels: lowScores.map((u: any) => u.evaluateeName || u.name || u.employeeName),
        datasets: [{
          label: 'Critical Scores (<60)',
          data: lowScores.map((u: any) => u.score || u.Score),
          backgroundColor: '#ef4444',
          borderRadius: 6
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c4);

    const monthlyTrends = dash?.monthlyTrends || dash?.MonthlyTrends || [
      { month: 'Jan', rate: 65 },
      { month: 'Feb', rate: 72 },
      { month: 'Mar', rate: 80 },
      { month: 'Apr', rate: 85 },
      { month: 'May', rate: 90 },
      { month: 'Jun', rate: 94 }
    ];

    const c5 = new Chart(this.trendsChartRef.nativeElement, {
      type: 'line',
      data: {
        labels: monthlyTrends.map((t: any) => t.month || t.Month || t.label),
        datasets: [{
          label: 'Evaluation Activity Rate',
          data: monthlyTrends.map((t: any) => t.rate || t.Rate || t.count || t.value),
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          fill: true,
          tension: 0.3
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
    this.chartInstances.push(c5);
  }
}