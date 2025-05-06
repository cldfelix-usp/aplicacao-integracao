import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {MatSlideToggleModule} from "@angular/material/slide-toggle";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,  MatSlideToggleModule,],
  template: `
    <router-outlet />
  `
})
export class AppComponent {
  title = 'client';
}
