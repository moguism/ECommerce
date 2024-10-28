import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  @HostListener('window:scroll', [])
  checkScroll()
  {
    const currentScroll = window.scrollY;

    if(currentScroll >= 100 && currentScroll <= 700)
    {
      document.getElementById("animated-section")?.classList.add("animation-class");
    }
    else if(currentScroll >= 2800 && currentScroll <= 3800)
    {
      document.getElementById("second-animated-section")?.classList.add("animation-class");
    }
    else if(currentScroll >= 5400 && currentScroll <= 6500)
    {
      document.getElementById("third-animated-section")?.classList.add("animation-class");
    }

    console.log(currentScroll)
  }
}