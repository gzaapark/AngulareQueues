import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class QueueServiceService {

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  GetTaskTypes() {
    return this.http.get<any>(this.baseUrl + 'api/Queue/GetTaskTypes', this.httpOptions);
  }

  GetQueues(now: Date = new Date()) {
    return this.http.get<any>(this.baseUrl + 'api/Queue/GetQueues?now=' + now.toISOString(), this.httpOptions);
  }

  AddTaskToAnyQueue(task: any, now: Date = new Date()) {
    return this.http.post<any>(this.baseUrl + 'api/Queue/AddTaskToAnyQueue?now=' + now.toISOString(), task, this.httpOptions);
  }

  GenerateRandomTasks(now: Date = new Date()) {
    return this.http.get<any>(this.baseUrl + 'api/Queue/GenerateRandomTasks?now=' + now.toISOString(), this.httpOptions);
  }

  ClearAllTasks() {
    return this.http.post<any>(this.baseUrl + 'api/Queue/ClearAllTasks', { }, this.httpOptions);
  }

}
