import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from "@angular/router";
import {DeviceListComponent} from "./device-list/device-list.component";
import {DeviceCommandsComponent} from "./device-commands/device-commands.component";

export const routes: Routes = [
  { path: '', component: DeviceListComponent },
  { path: 'list', component: DeviceListComponent },
  { path: 'commands}', component: DeviceCommandsComponent },
  { path: 'commands/result}', component: DeviceCommandsComponent },

];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ]
})
export class DevicesManagerModule { }
