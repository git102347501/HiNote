import { Component } from '@angular/core';
import { marked } from 'marked';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.prod';
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  id = '';
  content = '';
  constructor(private http : HttpClient, private route: ActivatedRoute) {
    this.route.queryParams.subscribe(queryParams => {
      console.log(queryParams['id']);
      this.id = queryParams['id'];
      this.getMdData();
    });
  }
  getMdData(){
    if (!this.id) {
      return;
    }
    this.http.get(environment.apis.default.url + '/api/app/hi-note/' + this.id + '/content').subscribe((res: any) =>{
      console.log(res.content);
      this.content = marked.parse(res.content);
    });
  }
}
