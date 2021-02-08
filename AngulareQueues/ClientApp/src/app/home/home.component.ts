import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  template: ''
})
export class HomeComponent {

  constructor(private router: Router) { }

  ngOnInit() {
    this.router.navigate(['./queue']);
  }

}
