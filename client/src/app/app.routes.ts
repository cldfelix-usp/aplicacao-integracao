import { Routes } from '@angular/router';
import {NotFoundComponent} from "./shared/not-found/not-found.component";
import {DevicesManagerModule} from "./devices-manager/devices-manager.module";

export const routes: Routes = [
  { path: '', loadChildren: () => DevicesManagerModule },
  { path: '**', component: NotFoundComponent }

];
