import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('1s ease-in', style({ opacity: 1 }))
      ]),
      transition(':leave', [
        animate('1s ease-out', style({ opacity: 0 }))
      ])
    ])
  ]
})
export class HomeComponent {
  showText = false;
  private scrollPosition = 800; // Cambia este valor a la posición deseada

  prueba()
  {
    console.log("hur hur hur hur hur")
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    console.log("Estoy bajando bobolón");
    
    const currentScroll = window.scrollY;
    
    if (currentScroll >= this.scrollPosition) {
      this.showText = true;
    } else {
      this.showText = false;
    }
  }
}
