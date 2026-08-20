import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NonConformitesComponent } from './components/non-conformites/non-conformites';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NonConformitesComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('suivi-non-conformites-app');
}
