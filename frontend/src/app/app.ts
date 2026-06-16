import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { CreateTemplate } from "./features/evaluation-template/create-template/create-template";
import { TopBarComponent } from './shared/layout/navbar/navbar/navbar';
import { SidebarComponent } from './shared/layout/sidebar/sidebar/sidebar';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, TopBarComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  title = signal('Evaluation System');
  showLayout = signal(true);
  private router = inject(Router);

  ngOnInit() {
    this.updateLayout(this.router.url);
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => this.updateLayout(event.urlAfterRedirects));
  }

  private updateLayout(url: string) {
    this.showLayout.set(!url.startsWith('/login') && !url.startsWith('/register'));
  }
}
