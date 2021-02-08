import { Component, OnInit } from '@angular/core';
import { QueueServiceService } from '../queue-service.service';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css', '../app.component.css']
})
export class QueueComponent implements OnInit {

  public QueueList: any[];
  public LoadDate: Date;
  public Error: any;

  constructor(private _queue: QueueServiceService) { }

  ngOnInit() {
    this.LoadQueues();
  }

  LoadQueues() {
    this._queue.GetQueues().subscribe(
      result => {
        this.QueueList = result;
        this.LoadDate = new Date();
      },
      error => {
        this.Error = error;
      }
    );
  }

  ClearAllTasks() {
    this._queue.ClearAllTasks().subscribe(
      result => {
        this.LoadQueues();
      },
      error => {
        this.Error = error;
      }
    );
  }

  private Imitator: any;
  private ImitatorTasks: any[];
  public ImitatorActive: boolean = false;

  StopImitation() {
    clearInterval(this.Imitator);
    this.ImitatorTasks = [];
    this.ImitatorActive = false;
  }

  StartImitation() {
    this.ImitatorActive = true;

    this._queue.GenerateRandomTasks().subscribe(
      result => {
        debugger;
        this.ImitatorTasks = result;
        this.LoadDate = new Date();

        var maxStep = Number.MAX_SAFE_INTEGER;
        for (let i in result) {
          if (result[i].taskType.duration < maxStep)
            maxStep = result[i].taskType.duration;
        }

        this.Imitator = setInterval(() => {
          this.LoadDate = this.addMinutes(this.LoadDate, maxStep);

          if (this.ImitatorTasks.length == 0) {
            this.StopImitation();
          }
          else {
            let task = this.ImitatorTasks.pop();
            this._queue.AddTaskToAnyQueue(task, this.LoadDate).subscribe(
              result => {
                console.log(JSON.stringify(result));

                this._queue.GetQueues(this.LoadDate).subscribe(
                  result => {
                    this.QueueList = result;
                  },
                  error => {
                    this.Error = error;
                  }
                );

              },
              error => {
                this.Error = error;
              }
            );
          }
        }, 1000);
      },
      error => {
        this.Error = error;
      }
    );
  }

  private addMinutes(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
  }

}
