import { Component, inject } from '@angular/core';
import { DeviceService } from '../device-service';
import { Commands, Device } from '../device';
import {JsonPipe} from "@angular/common";

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [
    JsonPipe
  ],
  templateUrl: './device-list.component.html',
  styleUrl: './device-list.component.scss'
})
export class DeviceListComponent {
private devicesService =  inject(DeviceService);

  devices?: Device[];
  commands?: Commands[];
  selectedDevice?: Device;
  selectedCommand?: Commands;
  commandResult: string = '';
  errorMessage: string = '';
  loading: boolean = false;

  constructor() { }

  ngOnInit(): void {
    this.getDevices();
  }

  getDevices() {
    // Simulate an API call to get devices
    this.devicesService.getDevices().subscribe(
      (response) => {
        console.log('Devices:', response);
        this.devices = response.data;

        this.getCommands('device002');


      },
      (error) => {
        console.error('Error fetching devices:', error);
      }
    );
  }

  selectDevice(device: Device, command: Commands) {
    this.selectedDevice = device;
    this.selectedCommand = command;
    this.commandResult = '';
    this.errorMessage = '';
    this.getCommands(device.id);
  }

  getCommands(deviceId: string) {
    // Simulate an API call to get commands for the selected device
    this.devicesService.getDevice(deviceId).subscribe(
      (response) => {
        console.log('Commands:', response.data);
        this.commands = response.data;

        this.commands?.forEach(com => {
          console.log('Format:',com.format);

        });
      },
      (error) => {
        console.error('Error fetching commands:', error);
      }
    );
  }

  sendCommand(commandName: string) {
    // Simulate sending a command to the device
    this.loading = true;
    setTimeout(() => {
      this.loading = false;
      this.commandResult = `Result of ${commandName}`;
      this.errorMessage = '';
    }, 2000);
  }

}
