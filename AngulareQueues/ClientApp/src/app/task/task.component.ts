import { Component, OnInit } from '@angular/core';
import { QueueServiceService } from '../queue-service.service';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['../app.component.css']
})
export class TaskComponent implements OnInit {

  public TaskTypeList: any[];
  public TaskTypeSelected: any;
  public TaskResult: any;
  public Error: any;

  constructor(private _queue: QueueServiceService) { }

  ngOnInit() {
    this._queue.GetTaskTypes().subscribe(
      result => {
        this.TaskTypeList = result;
        for (let i in this.TaskTypeList) {
          let elem = this.TaskTypeList[i];
          elem.factDuration = elem.duration
        }
      },
      error => {
        this.Error = error;
      }
    );
  }

  AddTask() {
    let send = {
      taskType: this.TaskTypeSelected,
      factDuration: this.TaskTypeSelected.factDuration
    };
    this._queue.AddTaskToAnyQueue(send).subscribe(
      result => {
        this.TaskResult = result;
      },
      error => {
        this.Error = error;
      }
    );
  }
}
