import { Component, HostListener } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
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
      const animatedSections = document.querySelectorAll('.animated-section-class');

      animatedSections.forEach((animatedSection) => {
        animatedSection.classList.add("animation-class");
      });
      
      this.reproducido = true
    }

    console.log(currentScroll)
  }


}
