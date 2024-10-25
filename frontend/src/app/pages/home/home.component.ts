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
  private scrollPosition = 2550;
  private reproducido = false;

  @HostListener('window:scroll', [])
  checkScroll()
  {
    const currentScroll = window.scrollY;

    if(currentScroll >= this.scrollPosition && !this.reproducido)
    {
      const animatedSection = document.getElementById("animated-section")
      console.log(animatedSection == null)
      document.getElementById("animated-section")?.classList.add("animation-class");
      this.reproducido = true
    }

    console.log(currentScroll)
  }
}