import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Signal } from '@angular/core';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { CreateTemplate } from "./features/evaluation-template/create-template/create-template";
import { TopBarComponent } from './shared/layout/navbar/navbar/navbar';
import { SidebarComponent } from './shared/layout/sidebar/sidebar/sidebar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopBarComponent, CreateTemplate],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = signal('Evaluation System');
}
