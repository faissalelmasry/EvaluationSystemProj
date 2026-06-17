import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { TopBarComponent } from './shared/layout/navbar/navbar/navbar';
import { SidebarComponent } from './shared/layout/sidebar/sidebar/sidebar';
import { CreateSection } from "./features/evaluation-section/create-section/create-section";
import { CreateCriteria } from "./features/evaluation-criteria/create-criteria/create-criteria";
import { TemplateDetails } from "./features/evaluation-template/template-details/template-details";
import { UpdateTemplate } from "./features/evaluation-template/update-template/update-template";
import { filter } from 'rxjs';
import { AuthService } from './core/services/auth';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopBarComponent,CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  title = signal('Evaluation System');
  showLayout = signal(true);
  private router = inject(Router);
  private authService = inject(AuthService);

  ngOnInit() {
    
    this.updateLayout(this.router.url);
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => this.updateLayout(event.urlAfterRedirects));
  }

  private updateLayout(url: string) {
    this.showLayout.set(!url.startsWith('/login') && !url.startsWith('/register') && !url.startsWith('/forgot-password') && !url.startsWith('/reset-password'));
  }
}
