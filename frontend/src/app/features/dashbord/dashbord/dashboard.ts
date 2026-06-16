import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { Reportservice } from '../../../core/services/reportservice';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private reportService = inject(Reportservice);
  private cdr = inject(ChangeDetectorRef);

  stats: any = null;
  recentActivity: any[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData() {
    this.isLoading = true;
    this.errorMessage = null;

    this.reportService.getDashboard().subscribe({
      next: (data: any) => {
        const res = data?.data || data?.result || data;
        
        this.stats = res;
        this.recentActivity = res?.recentActivity || res?.RecentActivity || [];
        this.isLoading = false;
        
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        
        if (err.status === 403) {
          this.errorMessage = 'Access Denied: Your account role (Evaluatee) does not have permission to view the admin dashboard.';
        } else {
          this.errorMessage = err.error?.message || 'Failed to load dashboard data. Please check backend connection.';
        }
        
        this.cdr.detectChanges();
      }
    });
  }
}