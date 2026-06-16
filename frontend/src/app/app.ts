import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Signal } from '@angular/core';
import { TemplateListComponent } from './features/evaluation-template/template-list.component/template-list.component';
import { CreateTemplate } from "./features/evaluation-template/create-template/create-template";
import { TopBarComponent } from './shared/layout/navbar/navbar/navbar';
import { SidebarComponent } from './shared/layout/sidebar/sidebar/sidebar';
import { CreateSection } from "./features/evaluation-section/create-section/create-section";
import { CreateCriteria } from "./features/evaluation-criteria/create-criteria/create-criteria";
import { TemplateDetails } from "./features/evaluation-template/template-details/template-details";
import { UpdateTemplate } from "./features/evaluation-template/update-template/update-template";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopBarComponent, CreateCriteria, TemplateListComponent, TemplateDetails, UpdateTemplate],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = signal('Evaluation System');
}
