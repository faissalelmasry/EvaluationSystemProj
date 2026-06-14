import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Signal } from '@angular/core';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { CreateTemplate } from "./features/evaluation-template/create-template/create-template";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, TemplateListComponent, CreateTemplate],
  templateUrl: './app.html',
})
export class AppComponent {
  title = signal('Evaluation System');
}