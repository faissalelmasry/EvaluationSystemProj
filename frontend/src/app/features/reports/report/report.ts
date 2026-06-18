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
        setTimeout(() => {
          this.destroyCharts();
          this.initializeCharts(res);
          this.cdr.detectChanges();
        }, 50);
      },
      error: (err) => {
        console.error(err);
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
    
    const deptArray = Array.isArray(res.departments) ? res.departments : (res.departments?.items || res.departments?.data || []);
    const deptLabels = deptArray.map((d: any) => d.departmentName || d.DepartmentName || '');
    const deptScores = deptArray.map((d: any) => d.averageScore || d.AverageScore || 0);

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

    const compData = res.completion?.data || res.completion?.result || res.completion;
    const completed = compData?.completedEvaluations || compData?.completedAssignments || dash?.completedEvaluations || 0;
    const pending = compData?.pendingEvaluations || compData?.pendingAssignments || dash?.pendingEvaluations || 0;
    
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

    const topScoresArray = Array.isArray(res.topScores) ? res.topScores : (res.topScores?.data || res.topScores?.items || dash?.topScoreEvaluations || []);
    const topLabels = topScoresArray.map((u: any) => u.evaluateeName || u.name || u.employeeName || '');
    const topValues = topScoresArray.map((u: any) => u.score || u.Score || u.averageScore || 0);

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

    const lowScoresArray = dash?.lowScoreEvaluations || dash?.lowScores || [];
    const lowLabels = lowScoresArray.map((u: any) => u.evaluateeName || u.name || u.employeeName || '');
    const lowValues = lowScoresArray.map((u: any) => u.score || u.Score || 0);

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

    const monthlyTrends = dash?.monthlyTrends || dash?.MonthlyTrends || [];
    const trendLabels = monthlyTrends.map((t: any) => t.month || t.Month || t.label || '');
    const trendValues = monthlyTrends.map((t: any) => t.rate || t.Rate || t.count || t.value || 0);

    const c5 = new Chart(this.trendsChartRef.nativeElement, {
      type: 'line',
      data: {
        labels: trendLabels,
        datasets: [{
          label: 'Evaluation Activity Rate',
          data: trendValues,
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