import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Signal } from '@angular/core';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,TemplateListComponent],
  templateUrl: './app.html',
})
export class AppComponent {
  title = signal('Evaluation System');
}