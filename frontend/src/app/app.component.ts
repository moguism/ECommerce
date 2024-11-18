import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {
  title = 'frontend';

  ngOnInit(): void {
    console.log(window.ethereum);
  }

}

declare global {
  interface Window {
  ethereum: any;
  }
}
  
