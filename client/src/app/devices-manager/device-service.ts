import { inject, Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable, tap } from 'rxjs';
import { IResultBaseDto, Device, Commands } from './device';
import { JsonPipe } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {

  private http = inject(HttpClient);

  private baseUrl = 'http://localhost:5225/api/v1/telnetapi';
  private httpOptions = {headers: {'Content-Type': 'application/json'}};


  constructor() { }

  // Get all devices
  getDevices(): Observable<IResultBaseDto<Device[]>> {
    return this.http.get<IResultBaseDto<Device[]>>(this.baseUrl + '/devicesasync', this.httpOptions).pipe(
      tap((response) => {
        return response;
      })
    );
  }

  // Get commands by name
  getDevice(id: string): Observable<IResultBaseDto<Commands[]>> {
    return this.http.get<IResultBaseDto<Commands[]>>(this.baseUrl + '/devices/'+id+'/commands/', this.httpOptions)
      .pipe(
        tap((response) => {
          response.data?.map((command) => {
            command.format =  JSON.parse(command.format)
          }
          );

          return response;
        })
      );
  }


  // Send command to device
  sendCommand(id: string,commandIndex: number, command: string[]): Observable<IResultBaseDto<any>> {

    var payload = {
      commandIndex: commandIndex,
      command: command
    };

    return this.http.post<IResultBaseDto<any>>(this.baseUrl + '/devices/' + id + '/command / ' + commandIndex + '/execute', JSON.stringify(payload), this.httpOptions)
      .pipe(
        tap((response) => {
          return response;
        })
      );
  }


}
